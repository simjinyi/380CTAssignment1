Sim Jin Yi (P17008744 / 9658521)
380CT - Assignment 1

README
------
- The exhaustive search algorithm can be found under the Exhaustive.cs file.
- The Tabu search algorithm can be found under the Tabu.cs file.
- The main program is located in the Driver.cs file.
	- To change the parameter, simply update the Graph graph = generator.<function>();
	- Note that the function returns null if the argument given is invalid, which might cause an exception to be thrown.
	
	- To specify the number of nodes in the connected graph, use Graph graph = generator.GenerateTotalNodes(numberOfNodes, true);
	- To specify the number of edges in the graph, use Graph graph = generator.GenerateTotalEdges(numberOfEdges);
	- To specify the number of edges per node in the graph, use Graph graph = generator.GeneratePerNodeEdges(numberOfEdgesPerNode);
	- To specify the number of nodes in the disconnected graph, use Graph graph = generator.GenerateTotalNodes(numberOfNodes, false);
	- To specify the diameter of the graphs, use Graph graph = generator.GenerateMaximumDiameter(diameter);
	- To specify the radius of the graphs, use Graph graph = generator.GenerateRadius(radius); 