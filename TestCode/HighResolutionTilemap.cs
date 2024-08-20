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
                m_highResolutionTilemap[x, y] = new HighResolutionTile();
            }
        }
    }

    public void generateTilemap() {
        turnLowResolutionTilemapToHighResolutionTilemap();
        connectAreas();
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
        }else if (lowResolutionTile.tileType == LowResolutionTileType.Door) {
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

    private readonly List<Area> m_connectedAreas;
    private readonly List<HighResolutionTile> m_tiles;
    private Vector2 m_position;

    public Area(Vector2 t_position, LowResolutionTile t_lowResolutionTile, AreaType t_areaType,
        List<HighResolutionTile> t_tilesInRoom) {
        m_position = t_position;
        LowResolutionTile = t_lowResolutionTile;
        m_tiles = t_tilesInRoom;
        AreaType = t_areaType;
        m_connectedAreas = new List<Area>();
    }

    public void connect(Area t_area) {
        m_connectedAreas.Add(t_area);
    }
}

public enum AreaType {
    None,
    Room,
    Door
}