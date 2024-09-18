namespace TestCode;

/// <summary>
/// Provides methods for finding paths using the A* algorithm.
/// </summary>
public class PathFinding {
    /// <summary>
    /// Finds a path from the start node to the target node using the A* algorithm.
    /// </summary>
    /// <param name="t_nodes">The list of all nodes in the graph.</param>
    /// <param name="t_startNode">The node to start the path from.</param>
    /// <param name="t_targetNode">The node to find the path to.</param>
    /// <returns>A list of nodes representing the found path, or an empty list if no path exists.</returns>
    public static List<Node> findPath(Node[] t_nodes, Node t_startNode, Node t_targetNode) {
        // Set up the initial distances for pathfinding
        setUpNodeCosts(t_nodes, t_startNode, t_targetNode);

        // Nodes yet to visit and already visited
        List<Node> nodesToVisit = new List<Node> { t_startNode };
        List<Node> visited = new List<Node>();

        Node visitingNode;

        while (nodesToVisit.Count != 0) {
            visitingNode = nodesToVisit[0]; // Get the node with the lowest cost

            // If we have reached the target node, we can return the path
            if (visitingNode == t_targetNode) {
                return reversePath(visited);
            }

            // Process each neighbor node
            foreach (Node t in visitingNode.neighbourNodes) {
                // Skip cycle or entrance nodes, or if the node has already been visited
                if (t.NodeType == NodeType.Cycle || t.NodeType == NodeType.Entrance || visited.Contains(t)) {
                    continue;
                }

                // Add to visit list and mark the current node as the previous node
                nodesToVisit.Add(t);
                t.previousNode = visitingNode;
            }

            // Move the current node to visited list and remove from visit list
            visited.Add(visitingNode);
            nodesToVisit.Remove(visitingNode);

            // Re-sort the list by the total cost (distance + heuristic)
            nodesToVisit = sortList(nodesToVisit);
        }

        // Return an empty list if no path is found
        return new List<Node>();
    }

    /// <summary>
    /// Reverses the path based on the visited nodes.
    /// </summary>
    /// <param name="t_visitedNodes">The list of visited nodes in order.</param>
    /// <returns>The reconstructed path, reversed from the target node to the start node.</returns>
    private static List<Node> reversePath(List<Node> t_visitedNodes) {
        List<Node> path = new List<Node>();
        Node tempNode = t_visitedNodes[^1]; // Start from the last node

        // Backtrack through the previous nodes to reconstruct the path
        while (tempNode.previousNode != null) {
            path.Add(tempNode);
            tempNode = tempNode.previousNode;
        }

        path.Reverse(); // Reverse to get the correct order from start to target
        return path;
    }

    /// <summary>
    /// Sets up the costs for all nodes based on Manhattan distances from the start and target nodes.
    /// </summary>
    /// <param name="t_nodes">The list of nodes to set up costs for.</param>
    /// <param name="t_startNode">The start node.</param>
    /// <param name="t_targetNode">The target node.</param>
    private static void setUpNodeCosts(Node[] t_nodes, Node t_startNode, Node t_targetNode) {
        foreach (Node tempNode in t_nodes) {
            // Calculate Manhattan distance to start and target nodes
            tempNode.distanceToStartNode = Math.Abs(tempNode.Position.X - t_startNode.Position.X) +
                                           Math.Abs(tempNode.Position.Y - t_startNode.Position.Y);
            tempNode.distanceToTargetNode = Math.Abs(tempNode.Position.X - t_targetNode.Position.X) +
                                            Math.Abs(tempNode.Position.Y - t_targetNode.Position.Y);
            // The total cost is the sum of both distances
            tempNode.totalCost = tempNode.distanceToStartNode + tempNode.distanceToTargetNode;
        }
    }

    /// <summary>
    /// Sorts the list of nodes based on their total cost (distance + heuristic).
    /// </summary>
    /// <param name="t_listToSort">The list of nodes to sort.</param>
    /// <returns>The sorted list of nodes.</returns>
    private static List<Node> sortList(List<Node> t_listToSort) {
        // Sort nodes in ascending order based on their total cost
        t_listToSort.Sort((t_xNode, t_yNode) => t_xNode.totalCost.CompareTo(t_yNode.totalCost));
        return t_listToSort;
    }
}
