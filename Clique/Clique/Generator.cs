using System;
using System.Collections.Generic;

namespace Clique
{
    class Generator
    {
        private Random random;

        public Generator()
        {
            random = new Random((int) HighResolutionClock.UtcNow.Ticks);
        }

        public Graph GenerateMaximumDiameter(int diameter)
        {
            // If the diameter is less than 1, then no nodes can be added
            if (diameter < 1)
                return null;

            // Obtain a constant number of nodes based on the diameter
            int numberOfNodes = random.Next(diameter * 2, diameter * 2 + 20);
            List<HashSet<int>> edges = new List<HashSet<int>>();

            for (int i = 0; i < numberOfNodes; ++i)
                edges.Add(new HashSet<int>());

            // Add the edges by jumping over intervals
            for (int i = 1, distance = numberOfNodes / 2; distance >= diameter; ++i, --distance)
            {
                for (int node = 0; node < numberOfNodes; ++node)
                {
                    // This ensures that the diameter must uphold, with the i == 1 || node < diameter
                    if (random.NextDouble() > 0.8 || i == 1 || node < diameter)
                    {
                        edges[node].Add((node + i) % numberOfNodes);
                        edges[(node + i) % numberOfNodes].Add(node);
                    }
                }
            }

            return new Graph(ConvertEdgesToAdjacencyMatrix(edges, numberOfNodes));
        }

        public Graph GenerateRadius(int radius)
        {
            // If the radius is less than 1, then no graph is undefined
            if (radius < 1)
                return null;

            // Obtain a random number of nodes from the given radius
            int numberOfNodes = random.Next(radius * 2, radius * 3);
            List<HashSet<int>> edges = new List<HashSet<int>>();

            for (int i = 0; i < numberOfNodes; ++i)
                edges.Add(new HashSet<int>());

            // Add the edges by jumping over intervals
            // Promises the radius to be as specified by the user
            for (int i = 1, distance = numberOfNodes / 2; distance >= radius; ++i, --distance)
            {
                for (int node = 0; node < numberOfNodes; ++node)
                {
                    edges[node].Add((node + i) % numberOfNodes);
                    edges[(node + i) % numberOfNodes].Add(node);
                }
            }

            return new Graph(ConvertEdgesToAdjacencyMatrix(edges, numberOfNodes));
        }

        public Graph GeneratePerNodeEdges(int numberOfEdgesPerNode)
        {
            // Get the constant possible number of nodes that can be added for the given number of edges
            int numberOfNodes = random.Next(numberOfEdgesPerNode + 1, 4 * (numberOfEdgesPerNode + 1));
            List<HashSet<int>> edges = new List<HashSet<int>>();

            for (int i = 0; i < numberOfNodes; ++i)
                edges.Add(new HashSet<int>());

            // Loop through the edges
            for (int node = 0; node < numberOfNodes; ++node)
            {
                // Add all the edges to the node
                for (int i = 0; i < numberOfEdgesPerNode; ++i)
                {
                    int nextNode = random.Next(0, numberOfNodes);

                    // Ensure that the node doesn't connect to itself and the edge do not exist
                    while (node == nextNode && !edges[node].Contains(nextNode))
                        nextNode = random.Next(0, numberOfNodes);

                    edges[node].Add(nextNode);
                    edges[nextNode].Add(node);
                }
            }

            return new Graph(ConvertEdgesToAdjacencyMatrix(edges, numberOfNodes));
        }

        public Graph GenerateTotalEdges(int numberOfEdges)
        {
            // Return null if there is no edge
            if (numberOfEdges < 1)
                return null;

            // Fix the range of the number of nodes that are allowed to be added
            int numberOfNodes = random.Next(GetMinimumNumberOfNodes(numberOfEdges), numberOfEdges + 1);
            List<HashSet<int>> edges = new List<HashSet<int>>();

            for (int i = 0; i < numberOfNodes; ++i)
                edges.Add(new HashSet<int>());

            // Generate the edges based on the numberOfEdges
            for (int edgeCount = 0; edgeCount < numberOfEdges ; ++edgeCount)
            {
                int node = edgeCount % numberOfNodes;
                int nextNode = random.Next(0, numberOfNodes);

                // Ensure that the edge does not exist and it does not connect back to the same node
                while (node == nextNode && !edges[node].Contains(nextNode))
                    nextNode = random.Next(0, numberOfNodes);

                edges[node].Add(nextNode);
                edges[nextNode].Add(node);
            }

            return new Graph(ConvertEdgesToAdjacencyMatrix(edges, numberOfNodes));
        }

        public Graph GenerateTotalNodes(int numberOfNodes, bool allowUnconnectedNodes)
        {
            // If the number of nodes is less than 2, then it don't make sense to create a test
            if (numberOfNodes < 2)
                return null;

            // Get the unconnected nodes (empty list if none)
            List<int> unconnectedNodes = GetRandomUnconnectedNodes(allowUnconnectedNodes ? numberOfNodes : 0);

            // Get the random minimum number of edges that can be formed
            int minimumNumberOfEdges = random.Next(SumToN(numberOfNodes - unconnectedNodes.Count) - 10, SumToN(numberOfNodes - unconnectedNodes.Count) + 1);
            List<HashSet<int>> edges = new List<HashSet<int>>();

            for (int i = 0; i < numberOfNodes; ++i)
                edges.Add(new HashSet<int>());
            
            // Loop through the nodes
            for (int node = 0; GetListOfHashSetCount(edges) < minimumNumberOfEdges; node = (node + 1) % numberOfNodes)
            {
                if (unconnectedNodes.BinarySearch(node) >= 0)
                    continue;

                int nextNode = random.Next(0, numberOfNodes); 

                // Make sure that none of the nodes are connecting to itself and the nodes that shouldn't be connected
                while (unconnectedNodes.BinarySearch(nextNode) >= 0 || node == nextNode)
                    nextNode = random.Next(0, numberOfNodes);

                // Add the edges for both ways (undirected graph)
                edges[node].Add(nextNode);
                edges[nextNode].Add(node);
            }

            return new Graph(ConvertEdgesToAdjacencyMatrix(edges, numberOfNodes));
        }

        private int SumToN(int n)
        {
            return n * (n + 1) / 2;
        }

        private int GetListOfHashSetCount(List<HashSet<int>> listOfHashSets)
        {
            int count = 0;

            for (int i = 0; i < listOfHashSets.Count; ++i)
                count += listOfHashSets[i].Count;

            return count;
        }

        private bool[][] ConvertEdgesToAdjacencyMatrix(List<HashSet<int>> edges, int numberOfNodes)
        {
            bool[][] adjacencyMatrix = new bool[numberOfNodes][];

            for (int i = 0; i < numberOfNodes; ++i)
            {
                adjacencyMatrix[i] = new bool[numberOfNodes];

                for (int j = 0; j < adjacencyMatrix[i].Length; ++j)
                    adjacencyMatrix[i][j] = false;

                foreach (int index in edges[i])
                    adjacencyMatrix[i][index] = true;
            }

            return adjacencyMatrix;
        }

        private List<int> GetRandomUnconnectedNodes(int numberOfNodes)
        {
            List<int> unconnectedNodes = new List<int>();

            if (numberOfNodes < 1)
                return unconnectedNodes;

            int numberOfUnconnectedNodes = random.Next(1, numberOfNodes);

            for (int i = 0; i < numberOfUnconnectedNodes; ++i)
            {
                int node = random.Next(0, numberOfNodes);
                unconnectedNodes.Sort();

                while (unconnectedNodes.BinarySearch(node) >= 0)
                    node = random.Next(0, numberOfNodes);

                unconnectedNodes.Add(node);
            }

            unconnectedNodes.Sort();
            return unconnectedNodes;
        }

        private int GetMinimumNumberOfNodes(int numberOfEdges)
        {
            return (int) Math.Ceiling((1 + Math.Sqrt(1 + 8 * numberOfEdges)) / 2);
        }
    }
}
