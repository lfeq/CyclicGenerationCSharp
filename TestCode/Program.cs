// See https://aka.ms/new-console-template for more information

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
graph.saveToJsonFile("changed_graph.json");