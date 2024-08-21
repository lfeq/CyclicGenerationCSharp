namespace TestCode;

public class Vector2(int t_xPosition, int t_yPosition) {
    public int X { get; set; } = t_xPosition;
    public int Y { get; set; } = t_yPosition;

    public static Vector2 zero() {
        return new Vector2(0, 0);
    }

    public static float distance(Vector2 t_v1, Vector2 t_v2) {
        double x = t_v2.X - t_v1.X;
        double y = t_v2.Y - t_v1.Y;
        x = Math.Pow(x, 2);
        y = Math.Pow(y, 2);
        double result = x + y;
        return (float)Math.Sqrt(result);
    }

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

    public static bool operator ==(Vector2 t_v1, Vector2 t_v2) {
        if (ReferenceEquals(t_v1, t_v2)) {
            return true;
        }
        if (ReferenceEquals(t_v1, null)) {
            return false;
        }
        if (ReferenceEquals(t_v2, null)) {
            return false;
        }
        return t_v1.X == t_v2.X && t_v1.Y == t_v2.Y;
    }

    public static bool operator !=(Vector2 t_v1, Vector2 t_v2) {
        return !(t_v1 == t_v2);
    }
    
    public override string ToString() {
        return $"X: {X}, Y:{Y}";
    }
}