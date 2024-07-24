using System.Text.Json;

class Node {
    public string Label { get; set; }

    public Node(string t_label) {
        Label = t_label;
    }
}

class Edge {
    public Node From { get; set; }
    public Node To { get; set; }

    public Edge(Node t_from, Node t_to) {
        From = t_from;
        To = t_to;
    }
}

class Graph {
    public List<Node> Nodes { get; } = new List<Node>();
    public List<Edge> Edges { get; } = new List<Edge>();

    public void addNode(Node t_node) {
        Nodes.Add(t_node);
    }

    public void addEdge(Edge t_edge) {
        Edges.Add(t_edge);
    }

    public void removeNode(Node t_node) {
        Nodes.Remove(t_node);
        Edges.RemoveAll(t_e => t_e.From == t_node || t_e.To == t_node);
    }

    public void removeEdge(Edge t_edge) {
        Edges.Remove(t_edge);
    }

    public void print() {
        Console.WriteLine("Nodes:");
        foreach (Node node in Nodes) {
            Console.WriteLine($" - {node.Label}");
        }
        Console.WriteLine("Edges:");
        foreach (Edge edge in Edges) {
            Console.WriteLine($" - {edge.From.Label} -> {edge.To.Label}");
        }
    }

    public void loadFromJsonFile(string t_filePath) {
        string jsonString = File.ReadAllText(t_filePath);
        GraphData? graphData = JsonSerializer.Deserialize<GraphData>(jsonString);
        if (graphData == null) {
            return;
        }
        foreach (string nodeLabel in graphData.Nodes) {
            addNode(new Node(nodeLabel));
        }
        foreach (EdgeData edgeData in graphData.Edges) {
            Node? fromNode = Nodes.FirstOrDefault(n => n.Label == edgeData.From);
            Node? toNode = Nodes.FirstOrDefault(n => n.Label == edgeData.To);
            if (fromNode != null && toNode != null) {
                addEdge(new Edge(fromNode, toNode));
            }
        }
    }

    public void SaveToJsonFile(string t_filePath) {
        GraphData graphData = new GraphData {
            Nodes = Nodes.Select(t_node => t_node.Label).ToList(),
            Edges = Edges.Select(t_edge => new EdgeData { From = t_edge.From.Label, To = t_edge.To.Label }).ToList()
        };
        string jsonString = JsonSerializer.Serialize(graphData, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(t_filePath, jsonString);
    }

    private class GraphData {
        public List<string> Nodes { get; set; }
        public List<EdgeData> Edges { get; set; }
    }

    private class EdgeData {
        public string From { get; set; }
        public string To { get; set; }
    }
}

// TODO: split Graph grammar rule into two, pattern (left side) and replacement (right side)
// right side should be a list of possible options.
class GraphGrammarRule {
    public List<string> NodeLabelsToMatch { get; set; }
    public List<(string from, string to)> EdgeLabelsToMatch { get; set; }
    public List<string> NewNodeLabels { get; set; }
    public List<(string from, string to)> NewEdges { get; set; }

    public void apply(Graph t_graph) {
        List<Node> matchedNodes = t_graph.Nodes.Where(t_n => NodeLabelsToMatch.Contains(t_n.Label)).ToList();
        if (matchedNodes.Count == NodeLabelsToMatch.Count) {
            List<Edge> matchedEdges = t_graph.Edges.Where(t_e =>
                EdgeLabelsToMatch.Any(t_elm => t_elm.from == t_e.From.Label && t_elm.to == t_e.To.Label)).ToList();
            if (matchedEdges.Count == EdgeLabelsToMatch.Count) {
                foreach (Edge edge in matchedEdges) {
                    t_graph.removeEdge(edge);
                }
                foreach (string label in NewNodeLabels) {
                    t_graph.addNode(new Node(label));
                }
                foreach ((string from, string to) in NewEdges) {
                    Node fromNode = t_graph.Nodes.First(t_n => t_n.Label == from);
                    Node toNode = t_graph.Nodes.First(t_n => t_n.Label == to);
                    t_graph.addEdge(new Edge(fromNode, toNode));
                }
            }
        }
    }
}

class Test {
    public static void main() {
        // Create graph
        Graph graph = new Graph();
        graph.loadFromJsonFile("initialGraph.json");
        Console.WriteLine("Original graph:");
        graph.print();

        // // Define a graph grammar rule to replace subgraph A->B->C with A->B, B->D, D->C
        // GraphGrammarRule rule = new GraphGrammarRule {
        //     NodeLabelsToMatch = new List<string> { "A", "B", "C" },
        //     EdgeLabelsToMatch = new List<(string from, string to)> {
        //         ("A", "B"),
        //         ("B", "C")
        //     },
        //     NewNodeLabels = new List<string> { "D" },
        //     NewEdges = new List<(string from, string to)> {
        //         ("A", "B"),
        //         ("B", "D"),
        //         ("D", "C")
        //     }#
        // };
        //
        // // Apply the rule
        // rule.apply(graph);
        // Console.WriteLine("Graph after applying the rule:");
        // graph.print();
        graph.SaveToJsonFile("changed_graph.json");
    }
}