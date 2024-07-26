public class Edge {
    public Node From { get; set; }
    public Node To { get; set; }

    public Edge(Node t_from, Node t_to) {
        From = t_from;
        To = t_to;
    }
    
    public static bool operator ==(Edge t_left, Edge t_right) {
        if (ReferenceEquals(t_left, t_right)) {
            return true;
        } // Checks if both objects are the same instance.
        if (ReferenceEquals(t_left, null) || ReferenceEquals(t_right, null)) {
            return false;
        } // Checks if either object is null.
        return (t_left.To.RoomType == t_right.To.RoomType) && (t_left.From.RoomType == t_right.From.RoomType);
    }

    public static bool operator !=(Edge t_left, Edge t_right) {
        return !(t_left == t_right);
    }
}