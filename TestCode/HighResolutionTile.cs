namespace TestCode;

/// <summary>
/// Represents a high-resolution tile in a grid with a specific position and type.
/// </summary>
public class HighResolutionTile {
    /// <summary>
    /// The position of the tile in the grid.
    /// </summary>
    public readonly Vector2 position;

    /// <summary>
    /// The type of the tile, determining its role (e.g., None, Room, or Door).
    /// </summary>
    public HighResolutionTileType tileType = HighResolutionTileType.None;

    /// <summary>
    /// Initializes a new instance of the <see cref="HighResolutionTile"/> class with the specified position.
    /// </summary>
    /// <param name="t_position">The position of the tile in the grid.</param>
    public HighResolutionTile(Vector2 t_position) {
        position = t_position;
    }

    /// <summary>
    /// Returns a string representation of the tile, based on its type.
    /// </summary>
    /// <returns>A string representing the tile type (e.g., "." for None, "X" for Room, "D" for Door).</returns>
    public override string ToString() {
        switch (tileType) {
            case HighResolutionTileType.None:
                return ".";
            case HighResolutionTileType.Room:
                return "X";
            case HighResolutionTileType.Door:
                return "D";
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

/// <summary>
/// Defines the possible types of high-resolution tiles.
/// </summary>
public enum HighResolutionTileType {
    /// <summary>
    /// The tile has no special characteristics.
    /// </summary>
    None,

    /// <summary>
    /// The tile represents a room.
    /// </summary>
    Room,

    /// <summary>
    /// The tile represents a door.
    /// </summary>
    Door,
}