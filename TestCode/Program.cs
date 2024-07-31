// See https://aka.ms/new-console-template for more information

Graph newGraph = new Graph(5, 5);
newGraph.setDungeonEntrance();
Console.WriteLine("Setting entrance");
Console.WriteLine(newGraph.ToString());
newGraph.setCycleEntrance();
Console.WriteLine("Setting cycle entrance");
Console.WriteLine(newGraph.ToString());
newGraph.generatePath(2);
Console.WriteLine("Creating cycle");
Console.WriteLine(newGraph.ToString());
newGraph.generateGoal();
Console.WriteLine("Setting cycle end");
Console.WriteLine(newGraph.ToString());