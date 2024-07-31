namespace TestCode;

public class PathFinding {
    // A star pathfinding algorithm
    public static List<Node> findPath(Node[] t_nodes, Node t_startNode, Node t_targetNode) {
        setUpNodeCosts(t_nodes, t_startNode, t_targetNode);
        List<Node> nodesToVisit = new List<Node>();
        List<Node> visited = new List<Node>();
        nodesToVisit.Add(t_startNode);
        Node visitingNode;
        while (nodesToVisit.Count != 0) {
            visitingNode = nodesToVisit[0];
            if (visitingNode == t_targetNode) {
                break;
            }
            List<Node> neighbourNodes = visitingNode.neighbourNodes;
            foreach (Node t in neighbourNodes) {
                if (t.NodeType == NodeType.Cycle || t.NodeType == NodeType.Entrance) {
                    continue;
                }
                if (visited.Contains(t)) {
                    continue;
                }
                nodesToVisit.Add(t);
                t.previousNode = visitingNode;
            }
            visited.Add(visitingNode);
            nodesToVisit.Remove(visitingNode);
            nodesToVisit.TrimExcess();
            nodesToVisit = sortList(nodesToVisit);
        }
        return reversePath(visited);
    }

    private static List<Node> reversePath(List<Node> t_visitedNodes) {
        List<Node> path = new List<Node>();
        Node tempNode = t_visitedNodes[^1]; // Last value from list
        while (tempNode.previousNode != null) {
            path.Add(tempNode);
            tempNode = tempNode.previousNode;
        }
        path.Reverse();
        return path;
    }

    private static void setUpNodeCosts(Node[] t_nodes, Node t_startNode, Node t_targetNode) {
        // Sets up the distances to do pathfinding using manhattan distances
        foreach (Node tempNode in t_nodes) {
            tempNode.distanceToStartNode = Math.Abs(tempNode.Position.X - t_startNode.Position.X) +
                                           Math.Abs(tempNode.Position.Y - t_startNode.Position.Y);
            tempNode.distanceToTargetNode = Math.Abs(tempNode.Position.X - t_targetNode.Position.X) +
                                            Math.Abs(tempNode.Position.Y - t_targetNode.Position.Y);
            tempNode.totalCost = tempNode.distanceToStartNode + tempNode.distanceToTargetNode;
        }
    }

    private static List<Node> sortList(List<Node> t_listToSort) {
        t_listToSort.Sort(delegate(Node t_xNode, Node t_yNode) {
            return t_xNode.totalCost.CompareTo(t_yNode.totalCost);
        });
        return t_listToSort;
    }
}