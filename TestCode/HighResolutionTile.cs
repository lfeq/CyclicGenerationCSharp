namespace TestCode;

public class HighResolutionTile {
    public readonly Vector2 position;
    public HighResolutionTileType tileType = HighResolutionTileType.None;

    public HighResolutionTile(Vector2 t_position) {
        position = t_position;
    }

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

public enum HighResolutionTileType {
    None,
    Room,
    Door,
}