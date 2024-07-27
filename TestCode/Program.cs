// See https://aka.ms/new-console-template for more information

Graph newGraph = new Graph(5, 5);
newGraph.setDungeonEntrance();
newGraph.setCycleEntrance();
Console.WriteLine(newGraph.ToString());