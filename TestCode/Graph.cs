using System.Reflection;
using TestCode;

/// <summary>
/// Represents a graph with nodes arranged in a grid.
/// </summary>
public class Graph {
    private readonly Node[,] m_grid;
    private readonly Node[] m_nodeArray;
    public int Width { get; private set; }
    public int Height { get; private set; }
    private readonly Random m_random = new Random();

    /// <summary>
    /// Initializes a new instance of the <see cref="Graph"/> class with the specified width and height.
    /// </summary>
    /// <param name="t_width">Width of the grid.</param>
    /// <param name="t_height">Height of the grid.</param>
    public Graph(int t_width, int t_height) {
        Console.WriteLine($"Random seed: {m_random.Next()}");
        Width = t_width;
        Height = t_height;
        m_grid = new Node[t_width, t_height];
        m_nodeArray = new Node[Width * Height];
        int i = 0;
        for (int y = 0; y < t_height; y++) {
            for (int x = 0; x < t_width; x++) {
                m_grid[x, y] = new Node.Builder()
                    .withPosition(x, y)
                    .build();
                m_nodeArray[i++] = m_grid[x, y];
            }
        }
        // Setup neighbors
        for (int y = 0; y < t_height; y++) {
            for (int x = 0; x < t_width; x++) {
                m_grid[x, y].neighbourNodes = getNeighbourNodes(m_grid[x, y]);
            }
        }
    }

    /// <summary>
    /// Retrieves the node at the specified position.
    /// </summary>
    /// <param name="t_position">Position in the grid.</param>
    /// <returns>The node at the given position.</returns>
    public Node getNodeInPosition(Vector2 t_position) {
        return m_grid[t_position.X, t_position.Y];
    }

    /// <summary>
    /// Checks if two nodes are connected.
    /// </summary>
    /// <param name="t_node1">The first node.</param>
    /// <param name="t_node2">The second node.</param>
    /// <returns>True if the nodes are connected, otherwise false.</returns>
    public bool areNodesConnected(Node t_node1, Node t_node2) {
        return t_node1.adjacentNodes.Contains(t_node2);
    }

    /// <summary>
    /// Sets a random node in the graph as the dungeon entrance.
    /// </summary>
    public void setDungeonEntrance() {
        int randomXPosition = m_random.Next(0, Width);
        int randomYPosition = m_random.Next(0, Height);
        m_grid[randomXPosition, randomYPosition].setRoomType(NodeType.Entrance);
    }

    /// <summary>
    /// Sets a random neighbor of the dungeon entrance as the cycle entrance.
    /// </summary>
    /// <exception cref="NullReferenceException">Thrown if no entrance is found in the graph.</exception>
    public void setCycleEntrance() {
        Node? startNode = getFirstNodeOfType(NodeType.Entrance);
        if (startNode is null) {
            throw new NullReferenceException("There is no entrance in the graph");
        }
        List<Node> neighbourNodes = getNeighbourNodes(startNode);
        Node randomNeighbour = neighbourNodes[m_random.Next(0, neighbourNodes.Count)];
        randomNeighbour.setRoomType(NodeType.CycleEntrance);
        addEdge(startNode, randomNeighbour);
    }

    /// <summary>
    /// Generates a path from the cycle entrance through the graph.
    /// </summary>
    /// <param name="t_maxIterations">Maximum number of iterations to generate the path.</param>
    /// <exception cref="NullReferenceException">Thrown if no cycle entrance is found.</exception>
    public void generatePath(int t_maxIterations) {
        Node? cycleStartNode = getFirstNodeOfType(NodeType.CycleEntrance);
        if (cycleStartNode is null) {
            throw new NullReferenceException("There is no cycle entrance in the graph");
        }
        Node lastNode = cycleStartNode;
        // This walks in a random direction trying to get the furthest away from the cycle start node.
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

    /// <summary>
    /// Connects the generated cycle path to the cycle entrance.
    /// </summary>
    /// <param name="t_startNode">The node from which to start connecting the cycle.</param>
    private void connectCycle(Node t_startNode) {
        Node? cycleStartNode = getFirstNodeOfType(NodeType.CycleEntrance);
        if (cycleStartNode is null) {
            throw new NullReferenceException("There is no cycle entrance in the graph");
        }
        List<Node> nodesToAdd = PathFinding.findPath(m_nodeArray, t_startNode, cycleStartNode);
        for (int i = 0; i < nodesToAdd.Count; i++) {
            Node node = nodesToAdd[i];
            node.setRoomType(NodeType.Cycle);
            if (i == nodesToAdd.Count - 1) {
                addEdge(node, cycleStartNode);
            }
            if (i == 0) {
                addEdge(node, t_startNode);
                continue;
            }
            addEdge(node, nodesToAdd[i - 1]);
        }
    }

    /// <summary>
    /// Generates the goal of the graph by setting the goal node and the end cycle node.
    /// </summary>
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
        while (endCycleNodeNeighbours.Count == 0) {
            endCycle.setRoomType(NodeType.Cycle);
            allCycleNodes = getAllNodesOfType(NodeType.Cycle);
            endCycle = allCycleNodes[m_random.Next(0, allCycleNodes.Count)];
            while (endCycle.hasNeighbourOfType(NodeType.Entrance)) {
                allCycleNodes.Remove(endCycle);
                allCycleNodes.TrimExcess();
                endCycle = allCycleNodes[m_random.Next(0, allCycleNodes.Count)];
            }
            endCycle.setRoomType(NodeType.CycleEnd);
            endCycleNodeNeighbours = endCycle.getNeighboursOfType(NodeType.None);
        } // Case where exit cannot be placed
        Node goal = endCycleNodeNeighbours[m_random.Next(0, endCycleNodeNeighbours.Count)];
        goal.setRoomType(NodeType.Goal);
        addEdge(endCycle, goal);
    }

    /// <summary>
    /// Retrieves the furthest node from the given list of nodes.
    /// </summary>
    /// <param name="t_candidateNodes">List of candidate nodes.</param>
    /// <param name="t_referenceNode">Reference node to measure distance from.</param>
    /// <returns>The furthest node from the list.</returns>
    /// <exception cref="NullReferenceException">Thrown if the node list is empty.</exception>
    private Node? getFurthestNodeFromList(List<Node> t_candidateNodes, Node t_referenceNode) {
        if (t_candidateNodes.Count == 0) {
            throw new NullReferenceException("Node list can't be empty :(");
        }
        Node? furthestNode = null;
        float biggestDistance = 0;
        foreach (Node? node in t_candidateNodes) {
            float distance = Vector2.distance(t_referenceNode.Position, node.Position);
            if (distance <= biggestDistance) {
                continue;
            }
            furthestNode = node;
            biggestDistance = distance;
        }
        return furthestNode;
    }

    /// <summary>
    /// Retrieves all valid neighbors of the given node.
    /// </summary>
    /// <param name="t_node">The node whose neighbors are to be retrieved.</param>
    /// <returns>A list of neighboring nodes.</returns>
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

    /// <summary>
    /// Checks if the specified position is outside the grid.
    /// </summary>
    /// <param name="t_xPosition">X position of the node.</param>
    /// <param name="t_yPosition">Y position of the node.</param>
    /// <returns>True if the position is within the grid boundaries; otherwise, false.</returns>
    private bool isNodeOutsideGrid(int t_xPosition, int t_yPosition) {
        return t_xPosition >= 0 && t_xPosition < Width && t_yPosition >= 0 && t_yPosition < Height;
    }

    /// <summary>
    /// Adds an edge (connection) between two nodes.
    /// </summary>
    /// <param name="t_firstNode">The first node.</param>
    /// <param name="t_secondNode">The second node.</param>
    private void addEdge(Node t_firstNode, Node t_secondNode) {
        if (!t_firstNode.adjacentNodes.Contains(t_secondNode)) {
            t_firstNode.adjacentNodes.Add(t_secondNode);
        }
        if (!t_secondNode.adjacentNodes.Contains(t_firstNode)) {
            t_secondNode.adjacentNodes.Add(t_firstNode);
        }
    }

    /// <summary>
    /// Retrieves the first node of the specified type.
    /// </summary>
    /// <param name="t_nodeType">Type of node to find.</param>
    /// <returns>The first node of the given type, or null if not found.</returns>
    private Node? getFirstNodeOfType(NodeType t_nodeType) {
        for (int y = 0; y < Height; y++) {
            for (int x = 0; x < Width; x++) {
                if (m_grid[x, y]!.NodeType == t_nodeType) {
                    return m_grid[x, y];
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Retrieves all nodes of the specified type.
    /// </summary>
    /// <param name="t_nodeType">Type of node to find.</param>
    /// <returns>A list of all nodes of the given type.</returns>
    private List<Node> getAllNodesOfType(NodeType t_nodeType) {
        List<Node> nodesToReturn = new List<Node>();
        for (int y = 0; y < Height; y++) {
            for (int x = 0; x < Width; x++) {
                if (m_grid[x, y].NodeType == t_nodeType) {
                    nodesToReturn.Add(m_grid[x, y]);
                }
            }
        }
        return nodesToReturn;
    }

    /// <summary>
    /// Returns a string representation of the graph.
    /// </summary>
    /// <returns>A string representing the graph structure.</returns>
    public override string ToString() {
        string graphString = "";
        for (int y = 0; y < Height; y++) {
            for (int x = 0; x < Width; x++) {
                graphString += m_grid[x, y].ToString();
            }
            graphString += "\n";
        }
        return graphString;
    }

    private int GetRandomSeed() {
        var fieldInfo = typeof(Random).GetField("_seed", BindingFlags.NonPublic | BindingFlags.Instance);
        if (fieldInfo != null) {
            return (int)fieldInfo.GetValue(m_random);
        }
        else {
            // Handle the case where the field is not found
            throw new Exception("Could not access random seed.");
        }
    }
}