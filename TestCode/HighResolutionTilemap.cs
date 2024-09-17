using TestCode.Graphs;

namespace TestCode;

/// <summary>
/// Represents a high-resolution tilemap generated from a low-resolution tilemap.
/// </summary>
public class HighResolutionTilemap {
    /// <summary>
    /// Gets the width of the high-resolution tilemap.
    /// </summary>
    private int Width { get; }

    /// <summary>
    /// Gets the height of the high-resolution tilemap.
    /// </summary>
    private int Height { get; }

    private const int SIZE_MULTIPLIER = 7;
    private readonly LowResolutionTilemap m_lowResolutionTilemap;
    private readonly HighResolutionTile[,] m_highResolutionTilemap;
    private readonly List<Area> m_areas;

    /// <summary>
    /// Initializes a new instance of the <see cref="HighResolutionTilemap"/> class.
    /// </summary>
    /// <param name="t_lowResolutionTilemap">The low-resolution tilemap to generate the high-resolution tilemap from.</param>
    public HighResolutionTilemap(LowResolutionTilemap t_lowResolutionTilemap) {
        m_lowResolutionTilemap = t_lowResolutionTilemap;
        Width = m_lowResolutionTilemap.Width * SIZE_MULTIPLIER;
        Height = m_lowResolutionTilemap.Height * SIZE_MULTIPLIER;
        m_highResolutionTilemap = new HighResolutionTile[Width, Height];
        m_areas = new List<Area>();
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                m_highResolutionTilemap[x, y] = new HighResolutionTile(new Vector2(x, y));
            }
        }
    }

    /// <summary>
    /// Generates the high-resolution tilemap by converting the low-resolution tilemap, connecting areas, and fixing door spaces.
    /// </summary>
    public void generateTilemap() {
        turnLowResolutionTilemapToHighResolutionTilemap();
        connectAreas();
        fixDoorSpaces();
    }

    /// <summary>
    /// Reduces the size of door areas to a single tile in the high-resolution tilemap.
    /// </summary>
    private void fixDoorSpaces() {
        foreach (Area area in m_areas) {
            if (area.AreaType != AreaType.Door) {
                // Skip non door rooms
                continue;
            }
            List<HighResolutionTile> deletedTiles = shrinkDoorSpaceToOne(area);
            addDeletedTilesToNeigbourAreas(area, deletedTiles);
        }
    }

    /// <summary>
    /// Adds deleted tiles (shrunk doors) to neighboring areas.
    /// </summary>
    /// <param name="t_door">The door area whose tiles were shrunk.</param>
    /// <param name="t_deletedTiles">The list of tiles removed from the door area.</param>
    private void addDeletedTilesToNeigbourAreas(Area t_door, List<HighResolutionTile> t_deletedTiles) {
        List<Area> connectedAreas = t_door.getConnectedAreas();
        if (connectedAreas == null || connectedAreas.Count == 0) {
            return;
        } // Early exit just in case
        Dictionary<string, Func<Area?, bool>> directions = new Dictionary<string, Func<Area?, bool>> {
            { "Above", t_r => t_r.position.Y < t_door.position.Y },
            { "Below", t_r => t_r.position.Y > t_door.position.Y },
            { "Left", t_r => t_r.position.X < t_door.position.X },
            { "Right", t_r => t_r.position.X > t_door.position.X },
        };
        Dictionary<string, Area?> connectedRoomMap = directions.ToDictionary(
            t_direction => t_direction.Key,
            t_direction => connectedAreas.Find(t_r => t_direction.Value(t_r))
        );
        foreach (HighResolutionTile tile in t_deletedTiles) {
            foreach (KeyValuePair<string, Area> direction in connectedRoomMap) {
                if (direction.Value == null) {
                    continue;
                }
                if ((direction.Key == "Above" && tile.position.Y < t_door.position.Y) ||
                    (direction.Key == "Below" && tile.position.Y > t_door.position.Y) ||
                    (direction.Key == "Left" && tile.position.X < t_door.position.X) ||
                    (direction.Key == "Right" && tile.position.X > t_door.position.X)) {
                    tile.tileType = HighResolutionTileType.Room;
                    direction.Value.getTiles().Add(tile);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Shrinks a door area to a single tile and returns the removed tiles.
    /// </summary>
    /// <param name="t_area">The area to shrink.</param>
    /// <returns>A list of tiles removed from the area.</returns>
    private List<HighResolutionTile> shrinkDoorSpaceToOne(Area t_area) {
        List<HighResolutionTile> listTilesToRemove = new List<HighResolutionTile>();
        foreach (HighResolutionTile tile in t_area.getTiles()) {
            if (tile.position == t_area.position) {
                continue;
            }
            tile.tileType = HighResolutionTileType.None;
            listTilesToRemove.Add(tile);
        }
        return listTilesToRemove;
    }

    /// <summary>
    /// Connects neighboring areas in the high-resolution tilemap.
    /// </summary>
    private void connectAreas() {
        foreach (Area area in m_areas) {
            List<LowResolutionTile> lowResolutionTilesNeighbours =
                m_lowResolutionTilemap.getNeighbourTiles(area.LowResolutionTile);
            foreach (LowResolutionTile lowResolutionTile in lowResolutionTilesNeighbours) {
                Area neighbourArea = m_areas.Find(a => a.LowResolutionTile == lowResolutionTile);
                if (neighbourArea != null) {
                    area.connect(neighbourArea);
                }
            }
        }
    }

    /// <summary>
    /// Converts the low-resolution tilemap to a high-resolution tilemap by expanding each tile.
    /// </summary>
    private void turnLowResolutionTilemapToHighResolutionTilemap() {
        for (int lrTilemapY = 1, tileY = 1;
             lrTilemapY < m_lowResolutionTilemap.Height;
             lrTilemapY++, tileY += SIZE_MULTIPLIER) {
            for (int lrTilemapX = 1, tileX = 1;
                 lrTilemapX < m_lowResolutionTilemap.Width;
                 lrTilemapX++, tileX += SIZE_MULTIPLIER) {
                addRoomFromLowResolutionTilemap(lrTilemapX, lrTilemapY, tileX, tileY);
            }
        }
    }

    /// <summary>
    /// Adds a room or door from the low-resolution tilemap to the high-resolution tilemap.
    /// </summary>
    /// <param name="t_lrTilemapX">The X position in the low-resolution tilemap.</param>
    /// <param name="t_lrTilemapY">The Y position in the low-resolution tilemap.</param>
    /// <param name="t_tileX">The X position in the high-resolution tilemap.</param>
    /// <param name="t_tileY">The Y position in the high-resolution tilemap.</param>
    private void addRoomFromLowResolutionTilemap(int t_lrTilemapX, int t_lrTilemapY, int t_tileX, int t_tileY) {
        // Get tile from low resolution tilemap
        LowResolutionTile lowResolutionTile =
            m_lowResolutionTilemap.getTileInPosition(new Vector2(t_lrTilemapX, t_lrTilemapY));
        if (lowResolutionTile.tileType == LowResolutionTileType.Empty) {
            return;
        }
        // Fill tiles with type
        List<HighResolutionTile> tilesInSpace = fillSpaceWithElements(t_tileX, t_tileY, lowResolutionTile);
        Vector2 middlePosition = new Vector2(t_tileX + SIZE_MULTIPLIER / 2, t_tileY + SIZE_MULTIPLIER / 2);
        if (lowResolutionTile.tileType == LowResolutionTileType.Room) {
            m_areas.Add(new Area(middlePosition, lowResolutionTile, AreaType.Room, tilesInSpace));
        }
        else if (lowResolutionTile.tileType == LowResolutionTileType.Door) {
            m_areas.Add(new Area(middlePosition, lowResolutionTile, AreaType.Door, tilesInSpace));
        }
    }

    /// <summary>
    /// Fills a specified area of the high-resolution tilemap with elements from the low-resolution tilemap.
    /// </summary>
    /// <param name="t_tileX">The starting X position in the high-resolution tilemap.</param>
    /// <param name="t_tileY">The starting Y position in the high-resolution tilemap.</param>
    /// <param name="t_lowResolutionTile">The low-resolution tile data to apply.</param>
    /// <returns>A list of high-resolution tiles that were filled.</returns>
    private List<HighResolutionTile> fillSpaceWithElements(int t_tileX, int t_tileY,
        LowResolutionTile t_lowResolutionTile) {
        List<HighResolutionTile> tileList = [];
        for (int y = 0; y < SIZE_MULTIPLIER; y++) {
            for (int x = 0; x < SIZE_MULTIPLIER; x++) {
                Vector2 position = new Vector2(t_tileX + x, t_tileY + y);
                // Set high resolution tile type at given position to door if low resolution tile is type door,
                // else set high resolution tile type to room.
                m_highResolutionTilemap[position.X, position.Y].tileType =
                    t_lowResolutionTile.tileType == LowResolutionTileType.Door
                        ? HighResolutionTileType.Door
                        : HighResolutionTileType.Room;
                tileList.Add(m_highResolutionTilemap[position.X, position.Y]);
            }
        }
        return tileList;
    }

    /// <summary>
    /// Returns a string representation of the high-resolution tilemap.
    /// </summary>
    /// <returns>A string representing the high-resolution tilemap.</returns>
    public override string ToString() {
        string graphString = "";
        for (int y = 0; y < m_highResolutionTilemap.GetLength(1); y++) {
            for (int x = 0; x < m_highResolutionTilemap.GetLength(0); x++) {
                graphString += m_highResolutionTilemap[x, y].ToString();
            }
            graphString += "\n";
        }
        return graphString;
    }
}

/// <summary>
/// Represents an area (room or door) in the tilemap.
/// </summary>
public class Area {
    public AreaType AreaType { get; }
    public LowResolutionTile LowResolutionTile { get; }
    public readonly Vector2 position;

    private readonly List<Area> m_connectedAreas;
    private readonly List<HighResolutionTile> m_tiles;

    public Area(Vector2 t_position, LowResolutionTile t_lowResolutionTile, AreaType t_areaType,
        List<HighResolutionTile> t_tilesInRoom) {
        position = t_position;
        LowResolutionTile = t_lowResolutionTile;
        m_tiles = t_tilesInRoom;
        AreaType = t_areaType;
        m_connectedAreas = new List<Area>();
    }

    /// <summary>
    /// Connects this area to another area.
    /// </summary>
    /// <param name="t_area">The area to connect to.</param>
    public void connect(Area t_area) {
        m_connectedAreas.Add(t_area);
    }

    /// <summary>
    /// Gets the list of tiles in this area.
    /// </summary>
    /// <returns>The list of tiles in the area.</returns>
    public List<HighResolutionTile> getTiles() {
        return m_tiles;
    }

    /// <summary>
    /// Gets the list of areas connected to this area.
    /// </summary>
    /// <returns>The list of connected areas.</returns>
    public List<Area> getConnectedAreas() {
        return m_connectedAreas;
    }
}

/// <summary>
/// Enumeration representing the type of area (Room or Door).
/// </summary>
public enum AreaType {
    None,
    Room,
    Door
}