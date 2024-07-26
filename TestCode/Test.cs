using System.Text.Json;

public class Node {
    public int Id { get; private set; }
    public RoomType RoomType { get; private set; }

    public class Builder {
        private int m_id = 0;
        private RoomType m_roomType = RoomType.None;


        public Builder withId(int t_id) {
            m_id = t_id;
            return this;
        }

        public Builder withRoomType(string t_roomType) {
            switch (t_roomType) {
                case "Goal":
                    m_roomType = RoomType.Goal;
                    break;
                case "Entrance":
                    m_roomType = RoomType.Entrance;
                    break;
                case "Room":
                    m_roomType = RoomType.Room;
                    break;
                default:
                    throw new Exception($"{t_roomType} is not recognized as a room type");
                    break;
            }
            return this;
        }

        public Node build() {
            return new Node {
                Id = m_id,
                RoomType = m_roomType
            };
            ;
        }
    }
}

public enum RoomType {
    None = 0,
    Entrance = 1,
    Goal = 2,
    Room = 3,
}

public class Edge {
    public Node From { get; set; }
    public Node To { get; set; }

    public Edge(Node t_from, Node t_to) {
        From = t_from;
        To = t_to;
    }
}

public class Graph {
    public List<Node> Nodes { get; } = new List<Node>();
    public List<Edge> Edges { get; } = new List<Edge>();

    private int m_totalNodes = 0; // Used to assign ID to node.

    private void addNode(Node t_node) {
        m_totalNodes++;
        Nodes.Add(t_node);
    }

    private void addEdge(Edge t_edge) {
        Edges.Add(t_edge);
    }

    private void removeNode(Node t_node) {
        Nodes.Remove(t_node);
        Edges.RemoveAll(t_e => t_e.From == t_node || t_e.To == t_node);
    }

    private void removeEdge(Edge t_edge) {
        Edges.Remove(t_edge);
    }

    private bool containsNode(RoomType t_roomType) {
        foreach (Node node in Nodes) {
            // TODO: Add an operator override in Node
            if (node.RoomType == t_roomType) {
                return true;
            }
        }
        return false;
    }

    private bool containsEdge(Edge t_edge) {
        foreach (Edge edge in Edges) {
            // TODO: Add an operator override in edge
            if (edge.To.RoomType == t_edge.To.RoomType && edge.From.RoomType == t_edge.From.RoomType) {
                return true;
            }
        }
        return false;
    }

    // TODO: Create JSON syntax to represent a transform rule.
    public void hasSubgraph(GraphGrammarRule t_rule) {
        foreach (Node ruleNode in t_rule.rule.Nodes) {
            if (!containsNode(ruleNode.RoomType)) {
                continue;
            }
            Console.WriteLine($"Node {ruleNode.RoomType.ToString()} exists in graph");
        }
        foreach (Edge ruleEdge in t_rule.rule.Edges) {
            if (!containsEdge(ruleEdge)) {
                continue;
            }
            Console.WriteLine(
                $"Edge: from: {ruleEdge.From.RoomType.ToString()} to: {ruleEdge.To.RoomType.ToString()} exists in graph");
        }
        Console.WriteLine("Rule is subgraph of graph");
    }

    public void print() {
        Console.WriteLine("Nodes:");
        foreach (Node node in Nodes) {
            Console.WriteLine($" - {node.Id}");
        }
        Console.WriteLine("Edges:");
        foreach (Edge edge in Edges) {
            Console.WriteLine($" - {edge.From.Id} -> {edge.To.Id}");
        }
    }

    /// <summary>
    /// Loads the graph data from a JSON file and populates the graph with nodes and edges.
    /// </summary>
    /// <param name="t_filePath">The file path to the JSON file containing the graph data.</param>
    public void loadFromJsonFile(string t_filePath) {
        // Dictionary to map node IDs to their labels.
        Dictionary<int, string> idDictionary = new Dictionary<int, string>();
        // Read the JSON content from the file.
        string jsonString = File.ReadAllText(t_filePath);
        // Deserialize the JSON content into a GraphData object.
        GraphData? graphData = JsonSerializer.Deserialize<GraphData>(jsonString);
        if (graphData == null) {
            return;
        }
        // Add nodes to the graph based on the deserialized data.
        foreach (NodeData nodeData in graphData.Nodes) {
            idDictionary.Add(m_totalNodes, nodeData.Label);
            addNode(new Node.Builder()
                .withId(m_totalNodes)
                .withRoomType(nodeData.Type)
                .build());
        }
        // Add edges to the graph based on the deserialized data.
        foreach (EdgeData edgeData in graphData.Edges) {
            // Will iterate over the Nodes list and find the node with Label "A". It will set fromNode to that node.
            Node? fromNode = Nodes.FirstOrDefault(n => idDictionary[n.Id] == edgeData.From);
            Node? toNode = Nodes.FirstOrDefault(n => idDictionary[n.Id] == edgeData.To);
            if (fromNode != null && toNode != null) {
                addEdge(new Edge(fromNode, toNode));
            }
        }
    }


    public void saveToJsonFile(string t_filePath) {
        GraphData graphData = new GraphData {
            Nodes = Nodes.Select(t_node => new NodeData
                { Label = t_node.Id.ToString(), Type = t_node.RoomType.ToString() }).ToList(),
            Edges = Edges.Select(t_edge => new EdgeData
                { From = t_edge.From.Id.ToString(), To = t_edge.To.Id.ToString() }).ToList()
        };
        string jsonString = JsonSerializer.Serialize(graphData, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(t_filePath, jsonString);
    }
}

public class GraphData {
    public List<NodeData> Nodes { get; set; }
    public List<EdgeData> Edges { get; set; }
}

public class NodeData {
    public string Label { get; set; }
    public string Type { get; set; }
}

public class EdgeData {
    public string From { get; set; }
    public string To { get; set; }
}

public class GraphGrammarRule {
    public readonly Graph rule;

    public GraphGrammarRule(string t_filePath) {
        rule = new Graph();
        rule.loadFromJsonFile(t_filePath);
    }

    // public void apply(Graph t_graph) {
    //     List<Node> matchedNodes = t_graph.Nodes.Where(t_n => NodeLabelsToMatch.Contains(t_n.Label)).ToList();
    //     if (matchedNodes.Count == NodeLabelsToMatch.Count) {
    //         List<Edge> matchedEdges = t_graph.Edges.Where(t_e =>
    //             EdgeLabelsToMatch.Any(t_elm => t_elm.from == t_e.From.Label && t_elm.to == t_e.To.Label)).ToList();
    //         if (matchedEdges.Count == EdgeLabelsToMatch.Count) {
    //             foreach (Edge edge in matchedEdges) {
    //                 t_graph.removeEdge(edge);
    //             }
    //             foreach (string label in NewNodeLabels) {
    //                 t_graph.addNode(new Node(label));
    //             }
    //             foreach ((string from, string to) in NewEdges) {
    //                 Node fromNode = t_graph.Nodes.First(t_n => t_n.Label == from);
    //                 Node toNode = t_graph.Nodes.First(t_n => t_n.Label == to);
    //                 t_graph.addEdge(new Edge(fromNode, toNode));
    //             }
    //         }
    //     }
    // }
}