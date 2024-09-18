namespace TestCode;

/// <summary>
/// Represents a 2D vector with basic vector operations.
/// </summary>
public class Vector2 {
    public int X { get; set; }
    public int Y { get; set; }

    public Vector2(int t_xPosition, int t_yPosition) {
        X = t_xPosition;
        Y = t_yPosition;
    }

    /// <summary>
    /// Returns a Vector2 at position (0, 0).
    /// </summary>
    public static Vector2 zero() {
        return new Vector2(0, 0);
    }

    /// <summary>
    /// Calculates the Euclidean distance between two vectors.
    /// </summary>
    public static float distance(Vector2 t_v1, Vector2 t_v2) {
        double x = t_v2.X - t_v1.X;
        double y = t_v2.Y - t_v1.Y;
        return (float)Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
    }

    #region Operator Overloads

    // Comparison operators
    public static bool operator <(Vector2 t_v1, Vector2 t_v2) {
        return (t_v1.X < t_v2.X) && (t_v1.Y < t_v2.Y);
    }

    public static bool operator >(Vector2 t_v1, Vector2 t_v2) {
        return (t_v1.X > t_v2.X) && (t_v1.Y > t_v2.Y);
    }

    public static bool operator <=(Vector2 t_v1, Vector2 t_v2) {
        return (t_v1.X <= t_v2.X) && (t_v1.Y <= t_v2.Y);
    }

    public static bool operator >=(Vector2 t_v1, Vector2 t_v2) {
        return (t_v1.X >= t_v2.X) && (t_v1.Y >= t_v2.Y);
    }

    // Arithmetic operators
    public static Vector2 operator +(Vector2 t_v1, Vector2 t_v2) {
        return new Vector2(t_v1.X + t_v2.X, t_v1.Y + t_v2.Y);
    }

    public static Vector2 operator -(Vector2 t_v1, Vector2 t_v2) {
        return new Vector2(t_v1.X - t_v2.X, t_v1.Y - t_v2.Y);
    }

    public static Vector2 operator *(Vector2 t_v1, Vector2 t_v2) {
        return new Vector2(t_v1.X * t_v2.X, t_v1.Y * t_v2.Y);
    }

    public static Vector2 operator /(Vector2 t_v1, Vector2 t_v2) {
        if (t_v2.X == 0 || t_v2.Y == 0)
            throw new DivideByZeroException("Cannot divide by zero.");
        return new Vector2(t_v1.X / t_v2.X, t_v1.Y / t_v2.Y);
    }

    // Equality operators
    public static bool operator ==(Vector2 t_v1, Vector2 t_v2) {
        if (ReferenceEquals(t_v1, t_v2)) {
            return true;
        }
        if (ReferenceEquals(t_v1, null) || ReferenceEquals(t_v2, null)) {
            return false;
        }
        return t_v1.X == t_v2.X && t_v1.Y == t_v2.Y;
    }

    public static bool operator !=(Vector2 t_v1, Vector2 t_v2) {
        return !(t_v1 == t_v2);
    }

    #endregion

    #region Overrides
    /// <summary>
    /// Overrides Equals to compare vector values.
    /// </summary>
    public override bool Equals(object obj) {
        if (obj is Vector2 other) {
            return X == other.X && Y == other.Y;
        }
        return false;
    }

    /// <summary>
    /// Overrides GetHashCode to return a hash based on vector values.
    /// </summary>
    public override int GetHashCode() {
        return HashCode.Combine(X, Y);
    }

    /// <summary>
    /// Returns a string representation of the vector.
    /// </summary>
    public override string ToString() {
        return $"X: {X}, Y: {Y}";
    }

    #endregion
}
