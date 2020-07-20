using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Clique
{
    class Tabu
    {
        private Random random;

        private Graph graph;
        private List<int> clique;
        private int[] timeLastMoved;

        private List<int> bestClique;
        private int bestCliqueSize;

        private int time;
        private int timeBestCliqueFound;
        private int timeLastRestart;

        private int prohibitionPeriod;
        private int timeProhibitionPeriodChanged;

        private Hashtable cliqueHistory;

        private int maxTime;
        private int targetCliqueSize;

        public Tabu(Graph graph, int maxTime)
        {
            random = new Random((int) HighResolutionClock.UtcNow.Ticks);

            this.graph = graph;
            clique = new List<int>();
            timeLastMoved = new int[graph.NumberNodes];

            for (int i = 0; i < graph.NumberNodes; ++i)
                timeLastMoved[i] = int.MinValue;

            bestClique = new List<int>();
            bestCliqueSize = 0;

            time = timeBestCliqueFound = timeLastRestart = 0;

            prohibitionPeriod = 1;
            timeProhibitionPeriodChanged = 0;

            cliqueHistory = new Hashtable();

            this.maxTime = maxTime;
            targetCliqueSize = graph.NumberNodes;
        }

        public List<int> FindMaximumClique()
        {
            // Add a random node into the graph, consider that as a 1-clique
            clique.Add(GetRandomNodeFromGraph());

            // Consider the current clique as the best clique available
            bestClique.AddRange(clique);
            bestCliqueSize = bestClique.Count;

            // Update the time when the best clique is found to the current time
            timeBestCliqueFound = time;

            // Get all possible nodes to add and remove
            // The possible nodes to add are those that are connected to every other node within the clique
            // The nodes to be removed are those in the graph but are not connected to at least one node from the formed clique
            List<int> possibleNodesToAdd = GetPossibleNodesToAdd();
            List<int> possibleNodesToImprove = GetPossibleNodesToImprove();

            // If the current time is not beyond the allowable test time
            // Or the clique found is not the entire graph (meaning that the entire graph is a clique)
            // Keep trying for a better solution
            while (time < maxTime && bestCliqueSize < targetCliqueSize)
            {
                ++time;

                int nodeToAdd = -1;
                int nodeToRemove = -1;

                bool hasCliqueChanged = false;

                // Check if there's any possible nodes to be added
                if (possibleNodesToAdd.Count > 0)
                {
                    // From the allowed list nodes to add, get the nodes which are not in the taboo list
                    List<int> allowedNodesToAdd = GetAllowedNodes(possibleNodesToAdd);

                    // If there exists at least one node that's not in the taboo list
                    if (allowedNodesToAdd.Count > 0)
                    {
                        // Randomly select a node that are connected to the most nodes to be added into the clique
                        nodeToAdd = GetBestNodeToAdd(allowedNodesToAdd, possibleNodesToAdd);

                        // Add the node and move the node into the taboo list
                        clique.Add(nodeToAdd);
                        timeLastMoved[nodeToAdd] = time;

                        clique.Sort();
                        hasCliqueChanged = true;

                        // If the clique is larger than the best clique found
                        // Update the best clique solution
                        if (clique.Count > bestCliqueSize)
                        {
                            bestCliqueSize = clique.Count;
                            bestClique.Clear();
                            bestClique.AddRange(clique);
                            timeBestCliqueFound = time;
                        }
                    }
                }

                // If no addition of node can be made,
                // Attempt to remove a node from the graph
                if (!hasCliqueChanged)
                {
                    if (clique.Count > 0)
                    {
                        // Get the nodes in the clique that are not in the taboo list
                        List<int> allowedNodesToRemove = GetAllowedNodes(clique);

                        // If there is at least one node that can be removed
                        if (allowedNodesToRemove.Count > 0)
                        {
                            // Randomly select a node that are connected to the least nodes to be removed from the clique
                            nodeToRemove = GetBestNodeToRemove(allowedNodesToRemove, possibleNodesToImprove);

                            // Remove the node from the clique
                            clique.Remove(nodeToRemove);

                            timeLastMoved[nodeToRemove] = time;
                            clique.Sort();
                            hasCliqueChanged = true;
                        }
                    }
                }

                // If all the nodes are in the taboo list
                // Forcefully remove a node from the clique randomly
                if (!hasCliqueChanged)
                {
                    if (clique.Count > 0)
                    {
                        nodeToRemove = clique[random.Next(0, clique.Count)];

                        clique.Remove(nodeToRemove);
                        timeLastMoved[nodeToRemove] = time;
                        clique.Sort();
                        hasCliqueChanged = true;
                    }
                }

                // Wait for 100 times of the best clique size before restarting everything again
                int restart = 100 * bestCliqueSize;

                // If it's more than 100 * bestCliqueSize unit of time not getting a better clique
                // And the prohibition period since the last restart has elapsed
                if (time - timeBestCliqueFound > restart && time - timeLastRestart > restart)
                {
                    // Reinitialize everything
                    timeLastRestart = time;
                    prohibitionPeriod = 1;
                    timeProhibitionPeriodChanged = time;

                    // Clear the previous clique found history
                    cliqueHistory.Clear();

                    // Find out the unmoved nodes
                    int seedNode = -1;
                    List<int> unmovedNodes = new List<int>();

                    for (int i = 0; i < timeLastMoved.Length; ++i)
                        if (timeLastMoved[i] == int.MinValue)
                            unmovedNodes.Add(i);

                    // Get one random unmoved node
                    // If not, randomly get a node
                    seedNode = (unmovedNodes.Count > 0) ? unmovedNodes[random.Next(0, unmovedNodes.Count)] : random.Next(0, graph.NumberNodes);

                    // Clear the previous clique and restart with a new clique
                    clique.Clear();
                    clique.Add(seedNode);
                }

                // Restart the iteration again
                possibleNodesToAdd = GetPossibleNodesToAdd();
                possibleNodesToImprove = GetPossibleNodesToImprove();

                // Recalculate the optimum prohibition period
                // For adaptive prohibition period based on the current situation
                UpdateProhibitionPeriod();
            }

            return bestClique;
        }

        private int GetRandomNodeFromGraph()
        {
            return random.Next(0, graph.NumberNodes);
        }

        private List<int> GetPossibleNodesToAdd()
        {
            List<int> results = new List<int>();

            // Check through all the nodes in the graph
            for (int i = 0; i < graph.NumberNodes; ++i)
                if (DoesAddingNodeFormsLargerClique(i))
                    results.Add(i);

            return results;
        }

        private bool DoesAddingNodeFormsLargerClique(int node)
        {
            for (int i = 0; i < clique.Count; ++i)
                if (!graph.AreAdjacent(clique[i], node)) 
                    return false;

            return true;
        }

        private List<int> GetPossibleNodesToImprove()
        {
            List<int> results = new List<int>();

            // Check through every node in the graph
            for (int i = 0; i < graph.NumberNodes; ++i)
            {
                int count = 0;

                // The node cannot form a better clique (it won't connect to at least one node within the clique)
                if (graph.NumberNeighbors(i) < clique.Count - 1)
                    continue;

                // The node is already in the clique
                if (clique.BinarySearch(i) >= 0)
                    continue;

                for (int j = 0; j < clique.Count; ++j)
                    if (graph.AreAdjacent(i, clique[j]))
                        ++count;

                // Add those nodes in the graph which is connecting to all but one node into the possible list
                if (count == clique.Count - 1)
                    results.Add(i);
            }

            return results;
        }

        private List<int> GetAllowedNodes(List<int> nodes)
        {
            List<int> results = new List<int>();

            if (nodes.Count > 0)
            {
                // Loop through the nodes
                for (int i = 0; i < nodes.Count; ++i)
                {
                    int currentNode = nodes[i];

                    // If the prohibition time has elapsed, add the node into a list
                    if (time > timeLastMoved[currentNode] + prohibitionPeriod)
                        results.Add(currentNode);
                }
            }

            // Return the list of allowed nodes (not in the taboo list)
            return results;
        }

        private int GetBestNodeToAdd(List<int> allowedNodesToAdd, List<int> possibleNodesToAdd)
        {
            // If there's only one allowed node, return that one node since there's no other choice
            if (allowedNodesToAdd.Count == 1)
                return allowedNodesToAdd[0];

            int maxDegree = 0;
            Hashtable nodeDegrees = new Hashtable();

            // Loop through every allowed node
            for (int i = 0; i < allowedNodesToAdd.Count; ++i)
            {
                int currentNode = allowedNodesToAdd[i];
                int currentNodeDegree = 0;

                // Loop through the possible nodes to be added to check the degree of the
                // current allowed node to every other allowed nodes
                for (int j = 0; j < possibleNodesToAdd.Count; ++j)
                    if (graph.AreAdjacent(currentNode, possibleNodesToAdd[j]))
                        ++currentNodeDegree;

                // Add the result into the hashtable
                nodeDegrees.Add(currentNode, currentNodeDegree);

                // Find out the nodes with the maximum degree
                if (currentNodeDegree > maxDegree)
                    maxDegree = currentNodeDegree;
            }

            List<int> results = new List<int>();

            // Only add the nodes with the maximum degree to the results list
            foreach (DictionaryEntry nodes in nodeDegrees)
                if ((int) nodes.Value >= maxDegree)
                    results.Add((int) nodes.Key);

            // Return only one random result
            return results[random.Next(0, results.Count)];
        }

        private int GetBestNodeToRemove(List<int> allowedNodesToRemove, List<int> possibleNodesToImprove)
        {
            // Return the only node allowed to be removed if there's only one
            if (allowedNodesToRemove.Count == 1)
                return allowedNodesToRemove[0];

            // Find the node which has the least degree to the possible nodes to improve the clique
            int maxNotAdjacentCount = 0;
            Hashtable nodeNotAdjacentCount = new Hashtable();

            for (int i = 0; i < allowedNodesToRemove.Count; ++i)
            {
                int currentNode = allowedNodesToRemove[i];
                int currentNodeNotAdjacentCount = 0;

                // Check and see if the current node is not adjacent to the nodes in the list of possible nodes to improve the clique
                for (int j = 0; j < possibleNodesToImprove.Count; ++j)
                    if (!graph.AreAdjacent(currentNode, possibleNodesToImprove[j]))
                        ++currentNodeNotAdjacentCount;

                nodeNotAdjacentCount.Add(currentNode, currentNodeNotAdjacentCount);

                // Find the maximum non-adjacent count
                if (currentNodeNotAdjacentCount > maxNotAdjacentCount)
                    maxNotAdjacentCount = currentNodeNotAdjacentCount;
            }

            List<int> results = new List<int>();

            foreach (DictionaryEntry nodes in nodeNotAdjacentCount)
                if ((int) nodes.Value >= maxNotAdjacentCount)
                    results.Add((int) nodes.Key);

            // Remove the least adjacent node
            return results[random.Next(0, results.Count)];
        }

        private void UpdateProhibitionPeriod()
        {
            // Create a new clique information instance to store the clique found and the time in which it is discovered
            CliqueInformation newCliqueInformation = new CliqueInformation(clique, time);

            // Check if the history contains the exact same clique
            if (cliqueHistory.Contains(newCliqueInformation.GetHashCode()))
            {
                // If the clique was seen previously
                // Find the last seen time, and update the last seen time of the clique to now
                CliqueInformation cliqueInformation = (CliqueInformation) cliqueHistory[newCliqueInformation.GetHashCode()];
                int lastSeen = time - cliqueInformation.LastSeen;
                cliqueInformation.LastSeen = time;

                // If the clique was just seen shortly before this
                if (lastSeen < 2 * (graph.NumberNodes - 1))
                {
                    // Update the prohibition period
                    timeProhibitionPeriodChanged = time;

                    // Gradually increases the prohibition period
                    // To lengthen the time in which a node cannot be used to prevent repetitive results
                    // 2 * bestCliqueSize will be the largest prohibition period
                    if (prohibitionPeriod + 1 < 2 * bestCliqueSize)
                        ++prohibitionPeriod;
                    else
                        prohibitionPeriod =  2 * bestCliqueSize;
                }
            }
            else
            {
                // If the clique was new, add it into the history
                cliqueHistory.Add(newCliqueInformation.GetHashCode(), newCliqueInformation);
            }

            // If the time since the update of the prohibition time was long
            // Time to reduce the prohibition period to facilitate more node changes
            if ((time - timeProhibitionPeriodChanged) > (10 * bestCliqueSize))
            {
                timeProhibitionPeriodChanged = time;

                // Gradually drcreases the prohibition period
                // To shorten the time in which a node cannot be used to facilitate nodes change
                // 1 will be the smallest prohibition period
                if (prohibitionPeriod - 1 > 1)
                    --prohibitionPeriod;
                else
                    prohibitionPeriod = 1;
            }
        }

        private class CliqueInformation
        {
            private List<int> clique;
            public int LastSeen { get; set; }

            public CliqueInformation(List<int> clique, int lastSeen)
            {
                this.clique = clique;
                LastSeen = lastSeen;
            }

            public override int GetHashCode()
            {
                StringBuilder stringBuilder = new StringBuilder();

                for (int i = 0; i < clique.Count; ++i)
                    stringBuilder.Append(clique[i] + " ");

                return stringBuilder.ToString().GetHashCode();
            }

            public override string ToString()
            {
                string str = "";

                for (int i = 0; i < clique.Count; ++i)
                    str += clique[i] + " ";

                return str;
            }

        }
    }
}
