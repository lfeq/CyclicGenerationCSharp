public class Graph {
    private readonly Node[,] m_grid;
    private readonly int m_width;
    private readonly int m_height;
    private Random m_random = new Random();

    public Graph(int t_width, int t_height) {
        m_width = t_width;
        m_height = t_height;
        m_grid = new Node[t_width, t_height];
        for (int y = 0; y < t_height; y++) {
            for (int x = 0; x < t_width; x++) {
                m_grid[x, y] = new Node.Builder()
                    .withPosition(x, y)
                    .build();
            }
        }
    }

    public void setDungeonEntrance() {
        Random random = new Random();
        int randomXPosition = random.Next(0, m_width);
        int randomYPosition = random.Next(0, m_height);
        m_grid[randomXPosition, randomYPosition].setRoomType(NodeType.Entrance);
    }

    public void setCycleEntrance() {
        Node? startNode = getFirstNodeOfType(NodeType.Entrance);
        if (startNode == null) {
            throw new NullReferenceException("There is no entrance in the graph");
        }
        List<Node> neighbourNodes = getNeighbourNodes(startNode);
        neighbourNodes[m_random.Next(0, neighbourNodes.Count)].setRoomType(NodeType.CycleEntrance);
    }

    private List<Node> getNeighbourNodes(Node t_node) {
        int xPosition = t_node.XPosition;
        int yPosition = t_node.YPosition;
        List<Node> returnList = new List<Node>();
        // Define the relative positions for von Neumann neighborhood
        int[][] directions = new int[][] {
            new int[] {0, -1},  // Up
            new int[] {0, 1},   // Down
            new int[] {-1, 0},  // Left
            new int[] {1, 0}    // Right
        };
        foreach (int[] dir in directions) {
            int newX = xPosition + dir[0];
            int newY = yPosition + dir[1];
            if (!isNodeOutsideGrid(newX, newY)) {
                continue;
            }
            returnList.Add(m_grid[newX, newY]);
        }
        return returnList;
    }

    // Check if node is outside the grid.
    private bool isNodeOutsideGrid(int t_xPosition, int t_yPosition) {
        return t_xPosition >= 0 && t_xPosition < m_width && t_yPosition >= 0 && t_yPosition < m_height;
    }

    private void addEdge(Node t_firstNode, Node t_secondNode) {
        if (!t_firstNode.adjacentNodes.Contains(t_secondNode)) {
            t_firstNode.adjacentNodes.Add(t_secondNode);
        }
        if (!t_secondNode.adjacentNodes.Contains(t_firstNode)) {
            t_secondNode.adjacentNodes.Add(t_firstNode);
        }
    }

    private Node? getFirstNodeOfType(NodeType t_nodeType) {
        for (int y = 0; y < m_height; y++) {
            for (int x = 0; x < m_width; x++) {
                if (m_grid[x, y]!.NodeType == t_nodeType) {
                    return m_grid[x, y];
                }
            }
        }
        return null;
    }

    public override string ToString() {
        string graphString = "";
        for (int y = 0; y < m_height; y++) {
            for (int x = 0; x < m_width; x++) {
                graphString += m_grid[x, y].ToString();
            }
            graphString += "\n";
        }
        return graphString;
    }
}