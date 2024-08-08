namespace TestCode.Graphs;

public class LowResolutionTile {
    public LowResolutionTileType tileType = LowResolutionTileType.None;
    public Vector2 position;

    public LowResolutionTile(Node t_node, Vector2 t_position) {
        tileType = convertNodeTypeToLowResolutionTileType(t_node.NodeType);
        position = t_position;
    }

    public LowResolutionTile(LowResolutionTileType t_tileType, Vector2 t_position) {
        tileType = t_tileType;
        position = t_position;
    }

    private LowResolutionTileType convertNodeTypeToLowResolutionTileType(NodeType t_nodeType) {
        switch (t_nodeType) {
            case NodeType.None:
                return LowResolutionTileType.Empty;
            case NodeType.Entrance:
                return LowResolutionTileType.Room;
            case NodeType.Goal:
                return LowResolutionTileType.Room;
            case NodeType.Room:
                return LowResolutionTileType.Room;
            case NodeType.CycleEntrance:
                return LowResolutionTileType.Room;
            case NodeType.Cycle:
                return LowResolutionTileType.Room;
            case NodeType.CycleEnd:
                return LowResolutionTileType.Room;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public override string ToString() {
        switch (tileType) {
            case LowResolutionTileType.None:
                return "'";
            case LowResolutionTileType.Room:
                return "X";
            case LowResolutionTileType.Door:
                return "D";
            case LowResolutionTileType.Empty:
                return ".";
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

public enum LowResolutionTileType {
    None,
    Room,
    Door,
    Empty,
}