namespace TestCode.Graphs;

public class LowResolutionTilemap {
    public int Width { get; private set; }
    public int Height { get; private set; }
    
    private const int SIZE_MULTIPLIER = 2;
    private LowResolutionTile[,] m_tilemap;

    public LowResolutionTilemap(Graph t_graph) {
        Width = t_graph.Width * SIZE_MULTIPLIER + 1;
        Height = t_graph.Height * SIZE_MULTIPLIER + 1;
        m_tilemap = new LowResolutionTile[Width, Height];
        for (int y = 0; y < m_tilemap.GetLength(1); y++) {
            for (int x = 0; x < m_tilemap.GetLength(0); x++) {
                m_tilemap[x, y] = new LowResolutionTile(LowResolutionTileType.Empty, new Vector2(x, y));
            }
        }
    }

    public void generateLowResolutionTileMap(Graph t_graph) {
        for (int graphY = 0, tileY = 1; graphY < t_graph.Height; graphY++, tileY += 2) {
            for (int graphX = 0, tileX = 1; graphX < t_graph.Width; graphX++, tileX += 2) {
                addTileToTileMap(t_graph.getNodeInPosition(new Vector2(graphX, graphY)), new Vector2(tileX, tileY));
                addDoorToRightNeighbour(graphX, graphY, t_graph, tileX, tileY);
                addDoorToLowerNeighbour(graphX, graphY, t_graph, tileX, tileY);
            }
        }
    }

    public List<LowResolutionTile> getNeighbourTiles(LowResolutionTile t_lowResolutionTile) {
        int xPosition = t_lowResolutionTile.position.X;
        int yPosition = t_lowResolutionTile.position.Y;
        List<LowResolutionTile> returnList = new List<LowResolutionTile>();
        // Define the relative positions for von Neumann neighborhood
        int[][] directions = new int[][] {
            new int[] { 0, -1 }, // Up
            new int[] { 0, 1 }, // Down
            new int[] { -1, 0 }, // Left
            new int[] { 1, 0 } // Right
        };
        foreach (int[] dir in directions) {
            int newX = xPosition + dir[0];
            int newY = yPosition + dir[1];
            if (!isNodeOutsideGrid(newX, newY)) {
                continue;
            }
            if (m_tilemap[newX, newY].tileType != LowResolutionTileType.None) {
                continue;
            }
            returnList.Add(m_tilemap[newX, newY]);
        }
        return returnList;
    }

    public LowResolutionTile getTileInPosition(Vector2 t_position) {
        if (t_position.X > Width || t_position.Y > Height) {
            throw new Exception("Position out of bounds of the tilemap");
        }
        return m_tilemap[t_position.X, t_position.Y];
    }

    private void addTileToTileMap(Node t_node, Vector2 t_position) {
        LowResolutionTile newTile = new LowResolutionTile(t_node, t_position);
        m_tilemap[t_position.X, t_position.Y] = newTile;
    }

    private void addDoorToRightNeighbour(int t_graphX, int t_graphY, Graph t_graph, int t_tileMapX, int t_tileMapY) {
        if (t_graphX >= t_graph.Width - 1 || !t_graph.areNodesConnected(
                t_graph.getNodeInPosition(new Vector2(t_graphX, t_graphY)),
                t_graph.getNodeInPosition(new Vector2(t_graphX + 1, t_graphY)))) {
            return;
        }
        LowResolutionTile doorTile =
            new LowResolutionTile(LowResolutionTileType.Door, new Vector2(t_tileMapX + 1, t_tileMapY));
        m_tilemap[t_tileMapX + 1, t_tileMapY] = doorTile;
    }

    private void addDoorToLowerNeighbour(int t_graphX, int t_graphY, Graph t_graph, int t_tileMapX, int t_tileMapY) {
        if (t_graphY >= t_graph.Height - 1 || !t_graph.areNodesConnected(
                t_graph.getNodeInPosition(new Vector2(t_graphX, t_graphY)),
                t_graph.getNodeInPosition(new Vector2(t_graphX, t_graphY + 1)))) {
            return;
        }
        LowResolutionTile doorTile =
            new LowResolutionTile(LowResolutionTileType.Door, new Vector2(t_tileMapX, t_tileMapY + 1));
        m_tilemap[t_tileMapX, t_tileMapY + 1] = doorTile;
    }
    
    private bool isNodeOutsideGrid(int t_xPosition, int t_yPosition) {
        return t_xPosition >= 0 && t_xPosition < Width && t_yPosition >= 0 && t_yPosition < Height;
    }

    public override string ToString() {
        string graphString = "";
        for (int y = 0; y < m_tilemap.GetLength(1); y++) {
            for (int x = 0; x < m_tilemap.GetLength(0); x++) {
                graphString += m_tilemap[x, y].ToString();
            }
            graphString += "\n";
        }
        return graphString;
    }
}