using TestCode;

public class Node {
    public Vector2 Position { get; private set; }
    public NodeType NodeType { get; private set; }
    public readonly List<Node> adjacentNodes = new List<Node>();

    // Pathfinding variables
    public float distanceToStartNode;
    public float distanceToTargetNode;
    public float totalCost;
    public List<Node> neighbourNodes = new List<Node>();
    public Node previousNode;

    public void setRoomType(NodeType t_nodeType) {
        NodeType = t_nodeType;
    }

    public bool hasNeighbourOfType(NodeType t_nodeType) {
        foreach (Node neighbourNode in neighbourNodes) {
            if (neighbourNode.NodeType == t_nodeType) {
                return true;
            }
        }
        return false;
    }

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

    public static bool operator ==(Node t_left, Node t_right) {
        if (ReferenceEquals(t_left, t_right)) {
            return true;
        } // Checks if both objects are the same instance.
        if (ReferenceEquals(t_left, null) || ReferenceEquals(t_right, null)) {
            return false;
        } // Checks if either object is null.
        return t_left.NodeType == t_right.NodeType;
    }

    public static bool operator !=(Node t_left, Node t_right) {
        return !(t_left == t_right);
    }

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

    public class Builder {
        private int m_xPosition = 0;
        private int m_yPosition = 0;
        private NodeType m_nodeType = NodeType.None;


        public Builder withPosition(int t_xPosition, int t_yPosition) {
            m_xPosition = t_xPosition;
            m_yPosition = t_yPosition;
            return this;
        }

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
                    break;
            }
            return this;
        }

        public Node? build() {
            return new Node {
                Position = new Vector2(m_xPosition, m_yPosition),
                NodeType = m_nodeType
            };
            ;
        }
    }
}

public enum NodeType {
    None = 0,
    Entrance = 1,
    Goal = 2,
    Room = 3,
    CycleEntrance = 4,
    Cycle = 5,
    CycleEnd = 6,
}