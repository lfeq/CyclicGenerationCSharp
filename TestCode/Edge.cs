/// <summary>
/// Represents an edge connecting two nodes in a graph.
/// </summary>
public class Edge {
    /// <summary>
    /// The starting node of the edge.
    /// </summary>
    public Node From { get; set; }

    /// <summary>
    /// The ending node of the edge.
    /// </summary>
    public Node To { get; set; }

    /// <summary>
    /// Constructs an edge between two nodes.
    /// </summary>
    /// <param name="t_from">The starting node of the edge.</param>
    /// <param name="t_to">The ending node of the edge.</param>
    public Edge(Node t_from, Node t_to) {
        From = t_from;
        To = t_to;
    }

    /// <summary>
    /// Checks if two edges are equal based on the types of their start and end nodes.
    /// </summary>
    /// <param name="t_left">The first edge to compare.</param>
    /// <param name="t_right">The second edge to compare.</param>
    /// <returns>True if the edges are equal, false otherwise.</returns>
    public static bool operator ==(Edge t_left, Edge t_right) {
        if (ReferenceEquals(t_left, t_right)) {
            return true; // Checks if both objects are the same instance.
        }
        if (ReferenceEquals(t_left, null) || ReferenceEquals(t_right, null)) {
            return false; // Checks if either object is null.
        }
        return (t_left.To.NodeType == t_right.To.NodeType) && (t_left.From.NodeType == t_right.From.NodeType);
    }

    /// <summary>
    /// Checks if two edges are not equal.
    /// </summary>
    /// <param name="t_left">The first edge to compare.</param>
    /// <param name="t_right">The second edge to compare.</param>
    /// <returns>True if the edges are not equal, false otherwise.</returns>
    public static bool operator !=(Edge t_left, Edge t_right) {
        return !(t_left == t_right);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current edge.
    /// </summary>
    /// <param name="obj">The object to compare with the current edge.</param>
    /// <returns>True if the object is an Edge and is equal to the current edge; otherwise, false.</returns>
    public override bool Equals(object? obj) {
        if (obj is Edge edge) {
            return this == edge;
        }
        return false;
    }

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current edge, based on its From and To nodes.</returns>
    public override int GetHashCode() {
        return HashCode.Combine(From, To);
    }
}

/// <summary>
/// Represents the data for an edge, where the nodes are represented by string identifiers.
/// </summary>
public class EdgeData {
    /// <summary>
    /// The identifier for the starting node of the edge.
    /// </summary>
    public string From { get; set; }

    /// <summary>
    /// The identifier for the ending node of the edge.
    /// </summary>
    public string To { get; set; }
}
