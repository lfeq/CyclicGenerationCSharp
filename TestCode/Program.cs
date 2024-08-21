// See https://aka.ms/new-console-template for more information

using TestCode;
using TestCode.Graphs;

// Create Initial Graph
Graph newGraph = new Graph(5, 5);
newGraph.setDungeonEntrance();
// Setup entrance of dungeon
Console.WriteLine("Setting entrance");
Console.WriteLine(newGraph.ToString());
// Set cycle entrance
newGraph.setCycleEntrance();
Console.WriteLine("Setting cycle entrance");
Console.WriteLine(newGraph.ToString());
// Create a path from the las point from the cycle to the entrance of the cycle
newGraph.generatePath(2);
Console.WriteLine("Creating cycle");
Console.WriteLine(newGraph.ToString());
// Set the goal of the dungeon
newGraph.generateGoal();
Console.WriteLine("Setting cycle end");
Console.WriteLine(newGraph.ToString());
// Create low resolution tilemap
LowResolutionTilemap lowResolutionTilemap = new LowResolutionTilemap(newGraph);
lowResolutionTilemap.generateLowResolutionTileMap(newGraph);
Console.WriteLine("Low Res Tilemap");
Console.WriteLine(lowResolutionTilemap.ToString());
// Create final tilemap
HighResolutionTilemap highResolutionTilemap = new HighResolutionTilemap(lowResolutionTilemap);
highResolutionTilemap.generateTilemap();
Console.WriteLine("High Resolution tile map");
Console.WriteLine(highResolutionTilemap.ToString());