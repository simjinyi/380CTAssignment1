using System;
using System.Collections;
using System.IO;
using System.Text;

namespace Clique
{
    class Graph
    {
        private readonly bool[][] adjacencyMatrix;

        public int NumberNodes { get; private set; }
        public int NumberEdges { get; private set; }
        private int[] numberNeighbors;

        public Graph(bool[][] adjacencyMatrix)
        {
            this.adjacencyMatrix = adjacencyMatrix;
            ValidateGraph();

            NumberNodes = adjacencyMatrix.Length;
            NumberEdges = GetNumberOfEdges() / 2;

            numberNeighbors = new int[adjacencyMatrix.Length];

            for (int i = 0; i < adjacencyMatrix.Length; ++i)
                numberNeighbors[i] = 0;

            for (int i = 0; i < adjacencyMatrix.Length; ++i)
                for (int j = 0; j < adjacencyMatrix[i].Length; ++j)
                    if (AreAdjacent(i, j))
                        ++numberNeighbors[i];
        }

        private void ValidateGraph()
        {
            if (adjacencyMatrix == null)
                throw new Exception("Adjacency matrix cannot be null");

            for (int i = 1; i < adjacencyMatrix.Length; ++i)
                if (adjacencyMatrix[i].Length != adjacencyMatrix[0].Length)
                    throw new Exception("Uneven adjacency matrix");

            if (adjacencyMatrix.Length != adjacencyMatrix[0].Length)
                throw new Exception("Uneven adjacency matrix");

            if (GetNumberOfEdges() % 2 != 0)
                throw new Exception("Unsymmetrical adjacency matrix");

            for (int i = 0; i < adjacencyMatrix.Length; ++i)
            {
                for (int j = 0; j < adjacencyMatrix[i].Length; ++j)
                {
                    if (j == 0 && adjacencyMatrix[i][i])
                        throw new Exception("Node " + i + " is connected to itself");

                    if (adjacencyMatrix[i][j] != adjacencyMatrix[j][i])
                        throw new Exception("Graph is not symmetrical at " + i + " and " + j);
                }
            }
        }

        private int GetNumberOfEdges()
        {
            int count = 0;

            for (int i = 0; i < adjacencyMatrix.Length; ++i)
                for (int j = 0; j < adjacencyMatrix[i].Length; ++j)
                    if (adjacencyMatrix[i][j])
                        ++count;

            return count;
        }

        public bool AreAdjacent(int node1, int node2)
        {
            return adjacencyMatrix[node1][node2] && adjacencyMatrix[node2][node1];
        }

        public int NumberNeighbors(int node)
        {
            return numberNeighbors[node];
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < adjacencyMatrix.Length; ++i)
            {
                for (int j = 0; j < adjacencyMatrix[i].Length; ++j)
                    stringBuilder.Append((adjacencyMatrix[i][j] ? 1 : 0) + " ");
                stringBuilder.Append('\n');
            }

            return stringBuilder.ToString();
        }
    }
}
