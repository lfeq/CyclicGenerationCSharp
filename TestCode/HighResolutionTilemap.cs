using TestCode.Graphs;

namespace TestCode;

public class HighResolutionTilemap {
    public int Width { get; private set; }
    public int Height { get; private set; }

    private const int SIZE_MULTIPLIER = 7;
    private readonly LowResolutionTilemap m_lowResolutionTilemap;
    private readonly HighResolutionTile[,] m_highResolutionTilemap;
    private readonly List<Area> m_areas;

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

    public void generateTilemap() {
        turnLowResolutionTilemapToHighResolutionTilemap();
        connectAreas();
        fixDoorSpaces();
    }

    // Reduces the size of a door to a single tile instead of a 7x7 room
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

    private List<HighResolutionTile> fillSpaceWithElements(int t_tileX, int t_tileY,
        LowResolutionTile t_lowResolutionTile) {
        List<HighResolutionTile> tileList = new List<HighResolutionTile>();
        for (int Y = 0; Y < SIZE_MULTIPLIER; Y++) {
            for (int X = 0; X < SIZE_MULTIPLIER; X++) {
                Vector2 position = new Vector2(t_tileX + X, t_tileY + Y);
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

public class Area {
    public AreaType AreaType { get; private set; }
    public LowResolutionTile LowResolutionTile { get; private set; }
    public Vector2 position;

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

    public void connect(Area t_area) {
        m_connectedAreas.Add(t_area);
    }

    public List<HighResolutionTile> getTiles() {
        return m_tiles;
    }

    public List<Area> getConnectedAreas() {
        return m_connectedAreas;
    }
}

public enum AreaType {
    None,
    Room,
    Door
}