namespace TestCode;

public class PathFinding {
    // TODO: Probably redo AStar from scratch.
    public static List<Node> AStar(Node[] t_nodes, Node t_startNode, Node t_targetNode, Graph t_graph) {
        List<Node> nodesToVisit = new List<Node>();
        List<Node> visited = new List<Node>();
        Node tempNode;
        // Set distances to infinity except for the starter node and sets the distance to the target
        for (int i = 0; i < t_nodes.Length; i++) {
            t_nodes[i].distanceToTarget =
                Vector2.distance(t_nodes[i].Position, t_targetNode.Position);
            t_nodes[i].shortestDistance = float.MaxValue;
            t_nodes[i].previousNode = null;
            if (t_nodes[i] == t_startNode) {
                t_nodes[i].shortestDistance = 0;
            }
        }
        nodesToVisit.Add(t_startNode);
        while (nodesToVisit.Count != 0) {
            tempNode = nodesToVisit[0];
            // Finish while loop because we found the target node
            if (tempNode == t_targetNode) {
                visited.Add(tempNode);
                break;
            }
            float[] edgesWeights = [1, 1, 1, 1];
            List<Node> neighbourNodes = tempNode.neighbourNodes;
            // Set shortest distance, heuristic and previous node if new distance is smaller than the current shortest distance
            for (int i = 0; i < neighbourNodes.Count; i++) {
                if (visited.Contains(tempNode)) {
                    break;
                }
                if (neighbourNodes[i].NodeType != NodeType.None) {
                    continue;
                }
                float newDistance = tempNode.shortestDistance + edgesWeights[i];
                if (newDistance < neighbourNodes[i].shortestDistance) {
                    neighbourNodes[i].shortestDistance = newDistance;
                    neighbourNodes[i].heuristic = edgesWeights[i] + neighbourNodes[i].distanceToTarget;
                    neighbourNodes[i].previousNode = tempNode;
                }
                nodesToVisit.Add(neighbourNodes[i]);
            }
            visited.Add(tempNode);
            nodesToVisit.Remove(tempNode);
            nodesToVisit.TrimExcess();
            nodesToVisit = SortListHeuristic(nodesToVisit);
        }
        List<Node> path = new List<Node>();
        tempNode = visited[visited.Count - 1];
        while (tempNode.previousNode != null) {
            path.Add(tempNode);
            tempNode = tempNode.previousNode;
        }
        path.Reverse();
        return path;
    }

    private static List<Node> SortListHeuristic(List<Node> t_listToSort) {
        t_listToSort.Sort(delegate(Node x, Node y) { return x.heuristic.CompareTo(y.heuristic); });
        return t_listToSort;
    }
}