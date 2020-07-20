using System;
using System.Collections.Generic;

namespace Clique
{
    class Exhaustive
    {
        private Graph graph;
        private int[] nodes;

        public Exhaustive(Graph graph)
        {
            this.graph = graph;

            nodes = new int[graph.NumberNodes];

            for (int i = 0; i < graph.NumberNodes; ++i)
                nodes[i] = i;
        }

        public bool FindKClique(int k, bool optimization)
        {
            // Optimization steps to prematurely determine the result
            if (optimization)
            {
                // If k is greater than the number of nodes, then k-clique cannot possibly exists
                if (k > nodes.Length)
                    return false;

                // If the number of edges in the graph is the maximum number of edges that can be fitted for the number of nodes
                // Conclude that the k-clique exist
                if (graph.NumberEdges >= SumToN(graph.NumberNodes))
                    return true;

                // Calculate how many nodes have at least k degree
                int satisfiedNodesCount = 0;

                for (int i = 0; i < nodes.Length; ++i)
                    if (GetDegree(i) >= k - 1)
                        ++satisfiedNodesCount;

                // If the number of nodes with at least k degree are less than k, then the k-clique cannot possibly exists
                if (satisfiedNodesCount < k)
                    return false;
            }

            // Call to perform the exhaustive search
            int[] data = new int[nodes.Length];
            return CheckCombination(data, 0, 0, k);
        }

        private int SumToN(int n)
        {
            return n * (n + 1) / 2;
        }

        private int GetDegree(int node)
        {
            int degree = 0;

            for (int i = 0; i < nodes.Length; ++i)
                if (graph.AreAdjacent(i, node))
                    ++degree;

            return degree;
        }

        private bool CheckCombination(int[] data, int currentIndex, int currentLength, int k)
        {
            // If the current number of elements taken into account is k
            if (currentLength == k)
            {
                List<int> results = new List<int>();

                // Add the subset into the list
                for (int i = 0; i < k; ++i)
                    results.Add(data[i]);

                // Check and see if the nodes within the subset is adjacent to every other nodes
                if (IsClique(results))
                {
                    // Print out the result and return true
                    // Console.WriteLine("[{0}]", string.Join(", ", results));
                    return true;
                }

                return false;
            }

            // If the range exceeds, halt the process
            if (currentIndex >= nodes.Length)
                return false;

            // Assign the current node at the currentIndex into the data list
            data[currentLength] = nodes[currentIndex];

            // Recursively call CheckCombination, one includes the next element while another doesn't include the next element
            // This call forms the 2^n complexity
            return CheckCombination(data, currentIndex + 1, currentLength + 1, k) 
                || CheckCombination(data, currentIndex + 1, currentLength, k);
        }

        private bool IsClique(List<int> nodes)
        {
            // Loop through every node in the subset
            for (int i = 0; i < nodes.Count; ++i)
            {
                // Loop through every node in the subset again
                for (int j = 0; j < nodes.Count; ++j)
                {
                    // Skip if it's the same node that's being checked
                    if (i == j)
                        continue;

                    // If there's one node that's not adjacent to at least one other node
                    // Then it's not forming a clique
                    if (!graph.AreAdjacent(nodes[i], nodes[j]))
                        return false;
                }
            }

            // All checks passed
            return true;
        }
    }
}
