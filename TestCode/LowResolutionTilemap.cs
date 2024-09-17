namespace TestCode.Graphs;

/// <summary>
/// Represents a tilemap with low-resolution tiles, used to map nodes from a graph to a 2D grid.
/// </summary>
public class LowResolutionTilemap {
    /// <summary>
    /// The width of the tilemap.
    /// </summary>
    public int Width { get; private set; }
    
    /// <summary>
    /// The height of the tilemap.
    /// </summary>
    public int Height { get; private set; }
    
    private const int SIZE_MULTIPLIER = 2;
    private LowResolutionTile[,] m_tilemap;

    /// <summary>
    /// Initializes a new instance of the <see cref="LowResolutionTilemap"/> class using the graph provided.
    /// </summary>
    /// <param name="t_graph">The graph to be mapped to a tilemap.</param>
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

    /// <summary>
    /// Generates the low-resolution tilemap based on the provided graph.
    /// </summary>
    /// <param name="t_graph">The graph used to generate the tilemap.</param>
    public void generateLowResolutionTileMap(Graph t_graph) {
        for (int graphY = 0, tileY = 1; graphY < t_graph.Height; graphY++, tileY += 2) {
            for (int graphX = 0, tileX = 1; graphX < t_graph.Width; graphX++, tileX += 2) {
                addTileToTileMap(t_graph.getNodeInPosition(new Vector2(graphX, graphY)), new Vector2(tileX, tileY));
                addDoorToRightNeighbour(graphX, graphY, t_graph, tileX, tileY);
                addDoorToLowerNeighbour(graphX, graphY, t_graph, tileX, tileY);
            }
        }
    }

    /// <summary>
    /// Retrieves the neighbor tiles for a given tile using von Neumann neighborhood rules.
    /// </summary>
    /// <param name="t_lowResolutionTile">The tile whose neighbors are to be found.</param>
    /// <returns>A list of neighboring tiles.</returns>
    public List<LowResolutionTile> getNeighbourTiles(LowResolutionTile t_lowResolutionTile) {
        int xPosition = t_lowResolutionTile.position.X;
        int yPosition = t_lowResolutionTile.position.Y;
        List<LowResolutionTile> returnList = new List<LowResolutionTile>();

        // Define the relative positions for von Neumann neighborhood
        int[][] directions = new int[][] {
            new int[] { 0, -1 }, // Up
            new int[] { 0, 1 },  // Down
            new int[] { -1, 0 }, // Left
            new int[] { 1, 0 }   // Right
        };

        foreach (int[] dir in directions) {
            int newX = xPosition + dir[0];
            int newY = yPosition + dir[1];
            if (!isNodeOutsideGrid(newX, newY)) {
                continue;
            }
            LowResolutionTile tile = m_tilemap[newX, newY];
            if (tile.tileType == LowResolutionTileType.Empty) {
                continue;
            }
            returnList.Add(tile);
        }

        return returnList;
    }

    /// <summary>
    /// Gets the tile at a specific position in the tilemap.
    /// </summary>
    /// <param name="t_position">The position of the tile.</param>
    /// <returns>The tile at the specified position.</returns>
    /// <exception cref="Exception">Thrown if the position is outside the bounds of the tilemap.</exception>
    public LowResolutionTile getTileInPosition(Vector2 t_position) {
        if (t_position.X > Width || t_position.Y > Height) {
            throw new Exception("Position out of bounds of the tilemap");
        }
        return m_tilemap[t_position.X, t_position.Y];
    }

    /// <summary>
    /// Adds a tile to the tilemap based on the node and position provided.
    /// </summary>
    /// <param name="t_node">The node to map to the tile.</param>
    /// <param name="t_position">The position in the tilemap.</param>
    private void addTileToTileMap(Node t_node, Vector2 t_position) {
        LowResolutionTile newTile = new LowResolutionTile(t_node, t_position);
        m_tilemap[t_position.X, t_position.Y] = newTile;
    }

    /// <summary>
    /// Adds a door to the right neighbor of the current tile, if connected.
    /// </summary>
    /// <param name="t_graphX">The X position in the graph.</param>
    /// <param name="t_graphY">The Y position in the graph.</param>
    /// <param name="t_graph">The graph to check for connections.</param>
    /// <param name="t_tileMapX">The X position in the tilemap.</param>
    /// <param name="t_tileMapY">The Y position in the tilemap.</param>
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

    /// <summary>
    /// Adds a door to the lower neighbor of the current tile, if connected.
    /// </summary>
    /// <param name="t_graphX">The X position in the graph.</param>
    /// <param name="t_graphY">The Y position in the graph.</param>
    /// <param name="t_graph">The graph to check for connections.</param>
    /// <param name="t_tileMapX">The X position in the tilemap.</param>
    /// <param name="t_tileMapY">The Y position in the tilemap.</param>
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
    
    /// <summary>
    /// Checks if a node is outside the grid bounds.
    /// </summary>
    /// <param name="t_xPosition">The X position in the tilemap.</param>
    /// <param name="t_yPosition">The Y position in the tilemap.</param>
    /// <returns>True if the node is within the grid, false otherwise.</returns>
    private bool isNodeOutsideGrid(int t_xPosition, int t_yPosition) {
        return t_xPosition >= 0 && t_xPosition < Width && t_yPosition >= 0 && t_yPosition < Height;
    }

    /// <summary>
    /// Returns a string representation of the tilemap.
    /// </summary>
    /// <returns>A string representing the entire tilemap.</returns>
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
