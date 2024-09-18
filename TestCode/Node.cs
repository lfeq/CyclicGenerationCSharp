using TestCode;

/// <summary>
/// Represents a node within a graph, which can have various types such as rooms or entrances.
/// </summary>
public class Node {
    /// <summary>
    /// Gets the position of the node in the graph.
    /// </summary>
    public Vector2 Position { get; private set; }

    /// <summary>
    /// Gets the type of the node (e.g., room, entrance, goal).
    /// </summary>
    public NodeType NodeType { get; private set; }

    /// <summary>
    /// A list of nodes that are adjacent to this node.
    /// </summary>
    public readonly List<Node> adjacentNodes = new List<Node>();

    // Pathfinding variables
    /// <summary>
    /// The distance from the start node during pathfinding.
    /// </summary>
    public float distanceToStartNode;

    /// <summary>
    /// The distance to the target node during pathfinding.
    /// </summary>
    public float distanceToTargetNode;

    /// <summary>
    /// The total cost calculated during pathfinding.
    /// </summary>
    public float totalCost;

    /// <summary>
    /// The list of neighboring nodes.
    /// </summary>
    public List<Node> neighbourNodes = new List<Node>();

    /// <summary>
    /// The node visited prior to reaching this node during pathfinding.
    /// </summary>
    public Node previousNode;

    /// <summary>
    /// Sets the room type (NodeType) of the node.
    /// </summary>
    /// <param name="t_nodeType">The type of the node (e.g., Room, Entrance, Goal).</param>
    public void setRoomType(NodeType t_nodeType) {
        NodeType = t_nodeType;
    }

    /// <summary>
    /// Determines whether the node has a neighboring node of a specific type.
    /// </summary>
    /// <param name="t_nodeType">The type of node to check for.</param>
    /// <returns>True if a neighboring node of the specified type exists; otherwise, false.</returns>
    public bool hasNeighbourOfType(NodeType t_nodeType) {
        foreach (Node neighbourNode in neighbourNodes) {
            if (neighbourNode.NodeType == t_nodeType) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Gets all neighbors of the node that are of a specific type.
    /// </summary>
    /// <param name="t_nodeType">The type of node to search for.</param>
    /// <returns>A list of neighboring nodes that match the specified type.</returns>
    public List<Node> getNeighboursOfType(NodeType t_nodeType) {
        List<Node> returnList = new List<Node>();
        foreach (Node neighbourNode in neighbourNodes) {
            if (neighbourNode.NodeType == t_nodeType) {
                returnList.Add(neighbourNode);
            }
        }
        return returnList;
    }

    #region Overrides

    /// <summary>
    /// Checks whether two nodes are equal based on their NodeType.
    /// </summary>
    public static bool operator ==(Node t_left, Node t_right) {
        if (ReferenceEquals(t_left, t_right)) {
            return true;
        }
        if (ReferenceEquals(t_left, null) || ReferenceEquals(t_right, null)) {
            return false;
        }
        return t_left.NodeType == t_right.NodeType;
    }

    /// <summary>
    /// Checks whether two nodes are not equal based on their NodeType.
    /// </summary>
    public static bool operator !=(Node t_left, Node t_right) {
        return !(t_left == t_right);
    }

    /// <summary>
    /// Returns a string representation of the node, based on its NodeType.
    /// </summary>
    public override string ToString() {
        switch (NodeType) {
            case NodeType.None:
                return ".";
            case NodeType.Entrance:
                return "e";
            case NodeType.Goal:
                return "g";
            case NodeType.Room:
                return "R";
            case NodeType.CycleEntrance:
                return "S";
            case NodeType.Cycle:
                return "C";
            case NodeType.CycleEnd:
                return "F";
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    #endregion

    /// <summary>
    /// Builder class for constructing instances of the Node class with custom properties.
    /// </summary>
    public class Builder {
        private int m_xPosition = 0;
        private int m_yPosition = 0;
        private NodeType m_nodeType = NodeType.None;

        /// <summary>
        /// Sets the position of the node being constructed.
        /// </summary>
        /// <param name="t_xPosition">The X position of the node.</param>
        /// <param name="t_yPosition">The Y position of the node.</param>
        /// <returns>The current instance of the builder.</returns>
        public Builder withPosition(int t_xPosition, int t_yPosition) {
            m_xPosition = t_xPosition;
            m_yPosition = t_yPosition;
            return this;
        }

        /// <summary>
        /// Sets the room type (NodeType) of the node being constructed.
        /// </summary>
        /// <param name="t_nodeType">The type of the node (e.g., Room, Entrance, Goal).</param>
        /// <returns>The current instance of the builder.</returns>
        public Builder withRoomType(NodeType t_nodeType) {
            switch (t_nodeType) {
                case NodeType.Goal:
                    m_nodeType = NodeType.Goal;
                    break;
                case NodeType.Entrance:
                    m_nodeType = NodeType.Entrance;
                    break;
                case NodeType.Room:
                    m_nodeType = NodeType.Room;
                    break;
                default:
                    throw new Exception($"{t_nodeType} is not recognized as a room type");
            }
            return this;
        }

        /// <summary>
        /// Builds the node using the specified properties.
        /// </summary>
        /// <returns>The constructed Node object.</returns>
        public Node? build() {
            return new Node {
                Position = new Vector2(m_xPosition, m_yPosition),
                NodeType = m_nodeType
            };
        }
    }
}

/// <summary>
/// Enum representing the types of nodes that can exist within a graph.
/// </summary>
public enum NodeType {
    None = 0,
    Entrance = 1,
    Goal = 2,
    Room = 3,
    CycleEntrance = 4,
    Cycle = 5,
    CycleEnd = 6,
}
