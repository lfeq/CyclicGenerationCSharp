using TestCode;

public class Graph {
    private readonly Node[,] m_grid;
    private readonly Node[] m_nodeArray;
    private readonly int m_width;
    private readonly int m_height;
    private Random m_random = new Random();

    public Graph(int t_width, int t_height) {
        m_width = t_width;
        m_height = t_height;
        m_grid = new Node[t_width, t_height];
        m_nodeArray = new Node[m_width * m_height];
        int i = 0;
        for (int y = 0; y < t_height; y++) {
            for (int x = 0; x < t_width; x++) {
                m_grid[x, y] = new Node.Builder()
                    .withPosition(x, y)
                    .build();
                m_nodeArray[i++] = m_grid[x, y];
            }
        }
        // Setup weights
        for (int y = 0; y < t_height; y++) {
            for (int x = 0; x < t_width; x++) {
                m_grid[x, y].neighbourNodes = getNeighbourNodes(m_grid[x, y]);
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
        if (startNode is null) {
            throw new NullReferenceException("There is no entrance in the graph");
        }
        List<Node> neighbourNodes = getNeighbourNodes(startNode);
        neighbourNodes[m_random.Next(0, neighbourNodes.Count)].setRoomType(NodeType.CycleEntrance);
    }

    public void generatePath(int t_maxIterations) {
        Node? cycleStartNode = getFirstNodeOfType(NodeType.CycleEntrance);
        if (cycleStartNode is null) {
            throw new NullReferenceException("There is no cycle entrance in the graph");
        }
        Node lastNode = cycleStartNode;
        for (int i = 0; i < t_maxIterations; i++) {
            List<Node> lastNodeNeighbours = getNeighbourNodes(lastNode);
            Node? nextNode = getFurthestNodeFromList(lastNodeNeighbours, cycleStartNode);
            nextNode.setRoomType(NodeType.Cycle);
            addEdge(nextNode, lastNode);
            lastNode = nextNode;
        }
        Console.WriteLine("Before connecting");
        Console.WriteLine(this.ToString());
        connectCycle(lastNode);
    }

    private void connectCycle(Node t_startNode) {
        Node? cycleStartNode = getFirstNodeOfType(NodeType.CycleEntrance);
        if (cycleStartNode is null) {
            throw new NullReferenceException("There is no cycle entrance in the graph");
        }
        List<Node> nodesToAdd = PathFinding.findPath(m_nodeArray, t_startNode, cycleStartNode);
        foreach (Node node in nodesToAdd) {
            node.setRoomType(NodeType.Cycle);
        }
    }

    public void generateGoal() {
        List<Node> allCycleNodes = getAllNodesOfType(NodeType.Cycle);
        Node endCycle = allCycleNodes[m_random.Next(0, allCycleNodes.Count)];
        while (endCycle.hasNeighbourOfType(NodeType.CycleEntrance)) {
            allCycleNodes.Remove(endCycle);
            allCycleNodes.TrimExcess();
            endCycle = allCycleNodes[m_random.Next(0, allCycleNodes.Count)];
        }
        endCycle.setRoomType(NodeType.CycleEnd);
        List<Node> endCycleNodeNeighbours = endCycle.getNeighboursOfType(NodeType.None);
        Node goal = endCycleNodeNeighbours[m_random.Next(0, endCycleNodeNeighbours.Count)];
        goal.setRoomType(NodeType.Goal);
        addEdge(endCycle, goal);
    }

    private Node? getFurthestNodeFromList(List<Node> t_nodesList, Node t_node) {
        if (t_nodesList.Count == 0) {
            throw new NullReferenceException("Nodes list can't be empty :(");
        }
        Node? furthestNode = null;
        Vector2 biggestDistance = Vector2.zero();
        foreach (Node? node in t_nodesList) {
            Vector2 distance = t_node.Position - node.Position;
            if (distance <= biggestDistance) {
                continue;
            }
            furthestNode = node;
            biggestDistance = distance;
        }
        return furthestNode;
    }

    private List<Node> getNeighbourNodes(Node t_node) {
        int xPosition = t_node.Position.X;
        int yPosition = t_node.Position.Y;
        List<Node> returnList = new List<Node>();
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
            if (m_grid[newX, newY].NodeType != NodeType.None) {
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

    private List<Node> getAllNodesOfType(NodeType t_nodeType) {
        List<Node> nodesToReturn = new List<Node>();
        for (int y = 0; y < m_height; y++) {
            for (int x = 0; x < m_width; x++) {
                if (m_grid[x, y].NodeType == t_nodeType) {
                    nodesToReturn.Add(m_grid[x, y]);
                }
            }
        }
        return nodesToReturn;
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