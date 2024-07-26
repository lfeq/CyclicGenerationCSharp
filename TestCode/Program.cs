// See https://aka.ms/new-console-template for more information

// Create graph
Graph graph = new Graph();
graph.loadFromJsonFile("initialGraph.json");
Console.WriteLine("Original graph:");
graph.print();
// Create graph rule
GraphGrammarRule rule = new GraphGrammarRule("initialGraph.json");
graph.hasSubgraph(rule);
// Save created graph to a file
graph.saveToJsonFile("changed_graph.json");