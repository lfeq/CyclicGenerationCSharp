public class Node {

    public int XPosition { get; private set; }
    public int YPosition { get; private set; }
    public NodeType NodeType { get; private set; }
    public List<Node> adjacentNodes = new List<Node>();

    public void setRoomType(NodeType t_nodeType) {
        NodeType = t_nodeType;
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
                XPosition = m_xPosition,
                YPosition = m_yPosition,
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
}

public class NodeData {
    public string Label { get; set; }
    public string Type { get; set; }
}