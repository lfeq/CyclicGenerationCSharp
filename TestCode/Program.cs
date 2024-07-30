// See https://aka.ms/new-console-template for more information

Graph newGraph = new Graph(5, 5);
newGraph.setDungeonEntrance();
newGraph.setCycleEntrance();
newGraph.generatePath(2);
Console.WriteLine(newGraph.ToString());