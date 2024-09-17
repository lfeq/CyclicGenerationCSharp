namespace TestCode.Graphs;

/// <summary>
/// Represents a tile in a low-resolution tilemap.
/// </summary>
public class LowResolutionTile {
    /// <summary>
    /// The type of the tile (e.g., Room, Door, Empty).
    /// </summary>
    public LowResolutionTileType tileType = LowResolutionTileType.None;
    
    /// <summary>
    /// The position of the tile in the tilemap.
    /// </summary>
    public Vector2 position;

    /// <summary>
    /// Initializes a new instance of the <see cref="LowResolutionTile"/> class based on a node and its position.
    /// </summary>
    /// <param name="t_node">The node from which the tile type is derived.</param>
    /// <param name="t_position">The position of the tile in the tilemap.</param>
    public LowResolutionTile(Node t_node, Vector2 t_position) {
        tileType = convertNodeTypeToLowResolutionTileType(t_node.NodeType);
        position = t_position;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LowResolutionTile"/> class based on a tile type and its position.
    /// </summary>
    /// <param name="t_tileType">The type of the tile.</param>
    /// <param name="t_position">The position of the tile in the tilemap.</param>
    public LowResolutionTile(LowResolutionTileType t_tileType, Vector2 t_position) {
        tileType = t_tileType;
        position = t_position;
    }

    /// <summary>
    /// Converts a node type to a corresponding low-resolution tile type.
    /// </summary>
    /// <param name="t_nodeType">The node type to convert.</param>
    /// <returns>The corresponding <see cref="LowResolutionTileType"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when an unsupported node type is provided.</exception>
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
    
    /// <summary>
    /// Returns a string representation of the tile.
    /// </summary>
    /// <returns>A string that represents the tile type.</returns>
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

/// <summary>
/// Enumeration representing the type of a low-resolution tile.
/// </summary>
public enum LowResolutionTileType {
    None,
    Room,
    Door,
    Empty,
}