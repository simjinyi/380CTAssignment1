//using System;
//using System.Collections.Generic; // List<int>
//using System.Text; // StringBuilder
//using System.IO; // FileStream
//using System.Collections; // Hashtable

//namespace TabuMaxClique
//{
//    class TabuMaxCliqueProgram
//    {
//        static Random random = null;

//        static void Main(string[] args)
//        {
//            try
//            {
//                Console.WriteLine("\nBegin tabu algorithm maximum clique demo\n");

//                string graphFile = "..\\..\\DimacsGraph.clq";
//                Console.WriteLine("Graph data DIMACS file is " + graphFile);
//                Console.WriteLine("Loading graph into memory");
//                Console.WriteLine("Graph loaded and validated\n");

//                MyGraph graph = new MyGraph(graphFile, "DIMACS");

//                int maxTime = 50; // for demo program
//                                  int maxTime = 100000; // for real problems
//                int targetCliqueSize = graph.NumberNodes;

//                Console.WriteLine("Using tabu algorithm with adaptive prohibit time\n");
//                List<int> maxClique = FindMaxClique(graph, maxTime, targetCliqueSize);
//                Console.WriteLine("\n\nMaximum time reached");
//                Console.WriteLine("\nSize of best clique found = " + maxClique.Count);

//                Console.WriteLine("\nBest clique found:");
//                Console.WriteLine(ListAsString(maxClique));

//                Console.WriteLine("\nEnd tabu agorithm maximum clique demo\n");
//                Console.ReadLine();
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("Fatal: " + ex.Message);
//                Console.ReadLine();
//            }
//        } // Main


//         Sim: First function to be called
//        static List<int> FindMaxClique(MyGraph graph, int maxTime, int targetCliqueSize)
//        {
//            List<int> clique = new List<int>();
//            random = new Random(1);
//            int time = 0;
//            int timeBestClique = 0;
//            int timeRestart = 0;

//            int prohibitPeriod = 1; // Sim: Number of iteration to be tabooed
//            int timeProhibitChanged = 0;

//            int nodeToAdd = -1;
//            int nodeToDrop = -1;

//             Sim: Initialize the last moved element to int.MinValue to signify that none of the solution in the neighborhood has been changed (moved)
//            int[] lastMoved = new int[graph.NumberNodes]; // the time when [node] was last added or dropped; determines tabu or not.
//            for (int i = 0; i < lastMoved.Length; ++i) { lastMoved[i] = int.MinValue; }

//            Hashtable history = new Hashtable(); // cliques seen. used to modify prohibit period, starting is empty

//             Sim: Store the seen clique information, starting is empty
//            System.Collections.Generic.HashSet<CliqueInfo> hs = new HashSet<CliqueInfo>();

//             Sim: Get a random node from the graph
//            int randomNode = random.Next(0, graph.NumberNodes);
//            Console.WriteLine("Adding node " + randomNode);

//            clique.Add(randomNode); // Sim: Assume one node as a clique, 1-clique

//             Sim: List to store the largest clique
//            List<int> bestClique = new List<int>();

//            Console.WriteLine("Start here");
//            foreach (int a in clique) Console.WriteLine(a);
//            Console.WriteLine("End here");

//             Sim: Temporarily the 1-clique (obtained randomly) is the best clique we can get before even performing the search
//            bestClique.AddRange(clique);

//             Sim: first is 1 because that's the 1-clique we had only at the first time
//            int bestSize = bestClique.Count;

//             Sim: How long the clique was unchanged since the largest clique was determined
//            timeBestClique = time;

//             Sim: JUMP
//            List<int> possibleAdd = MakePossibleAdd(graph, clique); // nodes which will increase size of clique
//            List<int> oneMissing = MakeOneMissing(graph, clique); // used to determine best node to drop from the clique

//             Sim: time is iteration, meaning if the maxTime is 50, the loop will maximum only go 50 times
//             Sim: targetCliqueSize is the number of node in the graph
//            while (time < maxTime && bestSize < targetCliqueSize)
//            {
//                ++time;

//                bool cliqueChanged = false; // to control branching logic in loop

//                if (possibleAdd.Count > 0) // Sim: nodes to add that will make the clique larger
//                {
//                     Sim: JUMP
//                    List<int> allowedAdd = SelectAllowedNodes(possibleAdd, time, prohibitPeriod, lastMoved); // could have Count == 0

//                     Run only if there's at least one allowed node
//                    if (allowedAdd.Count > 0)
//                    {
//                         Sim: JUMP
//                        nodeToAdd = GetNodeToAdd(graph, allowedAdd, possibleAdd); // node from allowedAdd which is most connected to nodes in possibleAdd

//                        Console.WriteLine("Adding node " + nodeToAdd);

//                        clique.Add(nodeToAdd); // Add the node to the clique
//                        lastMoved[nodeToAdd] = time; // Mark it as moved and add it to the taboo list

//                        clique.Sort();
//                        cliqueChanged = true;

//                        if (clique.Count > bestSize)
//                        {
//                            bestSize = clique.Count;
//                            bestClique.Clear();
//                            bestClique.AddRange(clique);
//                            timeBestClique = time;
//                            Console.WriteLine("new best = " + bestSize);
//                        }
//                    }
//                } // add allowed node


//                 Sim: If none of the operation above modified the clique
//                if (cliqueChanged == false)
//                {
//                    if (clique.Count > 0)
//                    {
//                        List<int> allowedInClique = SelectAllowedNodes(clique, time, prohibitPeriod, lastMoved); // nodes which are in clique and which are allowed
//                        if (allowedInClique.Count > 0)
//                        {
//                             Sim: Drop one node from the clique that does not connect to every other node in the clique
//                            nodeToDrop = GetNodeToDrop(graph, allowedInClique, oneMissing); // find allowed node in clique which generate max increase in possibleAdd set.

//                            Console.WriteLine("Dropping allowed node " + nodeToDrop);
//                            clique.Remove(nodeToDrop);

//                            lastMoved[nodeToDrop] = time;
//                            clique.Sort();
//                            cliqueChanged = true;
//                        }
//                    }
//                } // drop allowed node


//                 Sim: If there's still no node to be dropped, forcefully drop a random node from the clique
//                 **Sim: FAILED TO DETERMINE THE POSSIBILITY FOR THIS BLOCK
//                if (cliqueChanged == false)
//                {
//                    if (clique.Count > 0)
//                    {
//                        nodeToDrop = clique[random.Next(0, clique.Count)];
//                        Console.WriteLine("Dropping random node " + nodeToDrop);
//                        clique.Remove(nodeToDrop);
//                        lastMoved[nodeToDrop] = time;
//                        clique.Sort();
//                        cliqueChanged = true;
//                    }
//                } // drop random node


//                int restart = 100 * bestSize; // for real problems
//                int restart = 2 * bestSize; // dummy short value for demo program

//                 Sim: Determine how long you want to restart the entire search, when you have a few iteration without getting improvement
//                 Sim: The timeRestart determines if you should restart
//                if (time - timeBestClique > restart && time - timeRestart > restart) // restart?
//                {
//                    Console.WriteLine("\nRestarting with prohibit period " + prohibitPeriod);
//                    timeRestart = time;
//                    prohibitPeriod = 1;
//                    timeProhibitChanged = time;

//                     Sim: history stores the clique seen before this, now clear it
//                     **Sim: IMPOSSIBLE TO HAPPEN DETERMINED THUS FAR (MIGHT BE USELESS)
//                    history.Clear();

//                    int seedNode = -1;
//                    List<int> temp = new List<int>();

//                     Sim: Find and see how many unmoved nodes are there
//                    for (int i = 0; i < lastMoved.Length; ++i)
//                    {
//                        if (lastMoved[i] == int.MinValue) temp.Add(i);
//                    }

//                     Sim: Restart at a different node, which hadn't moved
//                    if (temp.Count > 0)
//                        seedNode = temp[random.Next(0, temp.Count)];

//                     Sim: Restart a another random node
//                    else
//                        seedNode = random.Next(0, graph.NumberNodes);

//                     Remove the entire clique
//                    clique.Clear();
//                    Console.WriteLine("Adding seed node " + seedNode);
//                    clique.Add(seedNode);

//                } // restart

//                possibleAdd = MakePossibleAdd(graph, clique);   // nodes which will increase size of clique
//                oneMissing = MakeOneMissing(graph, clique);     // nodes which can be removed clique

//                 Sim: JUMP UpdateProhibitPeriod
//                prohibitPeriod = UpdateProhibitPeriod(graph, clique, bestSize, history, time, prohibitPeriod, ref timeProhibitChanged);

//            } // main processing loop

//            return bestClique;
//        } // FindMaxClique

//         ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

//         Sim: Returns a list of the possible nodes to be added (maximizes the clique)
//        static List<int> MakePossibleAdd(MyGraph graph, List<int> clique)
//        {
//             create list of nodes in graph which are connected to all nodes in clique and therefore will form a larger clique
//             calls helper FormsALargerClique
//            List<int> result = new List<int>();

//            for (int i = 0; i < graph.NumberNodes; ++i) // each node in graph
//            {
//                 Sim: If adding the node forms a larger graph, add it into the result
//                 Sim: JUMP
//                if (FormsALargerClique(graph, clique, i) == true)
//                    result.Add(i);
//            }

//            return result; // could be empty List
//        } // MakePossibleAdd


//         Sim: Checks and returns if the new node forms a larger clique
//        static bool FormsALargerClique(MyGraph graph, List<int> clique, int node)
//        {
//             is node connected to all nodes in clique?
//            for (int i = 0; i < clique.Count; ++i) // compare node against each member of clique
//            {
//                 Sim: JUMP
//                if (clique[i] == node) return false; // node is aleady in clique so node will not form a larger clique
//                if (graph.AreAdjacent(clique[i], node) == false) return false; // node is not connected to one of the nodes in the clique
//            }
//            return true; // passed all checks
//        } // FormsALargerClique


//         Sim: Get only one node to add from the allowed list (not in taboo)
//        static int GetNodeToAdd(MyGraph graph, List<int> allowedAdd, List<int> possibleAdd)
//        {
//             find node from a List of allowedAdd which has max degree in posibleAdd
//             there could be more than one, if so, pick one at random
//            if (allowedAdd == null) throw new Exception("List allowedPossibleAdd is null in GetNodeToAdd");
//            if (possibleAdd == null) throw new Exception("List possibleAdd is null in GetNodeToAdd");
//            if (allowedAdd.Count == 0) throw new Exception("List allowedPossibleAdd has Count 0 in GetNodeToAdd");
//            if (possibleAdd.Count == 0) throw new Exception("List possibleAdd has Count 0 in GetNodeToAdd");

//             Sim: Return the only node allowed to be added
//            if (allowedAdd.Count == 1) // there is only 1 node to choose from
//                return allowedAdd[0];

//             examine each node in allowedAdd to find the max degree in possibleAdd (because there could be several nodes tied with max degree)
//            int maxDegree = 0;

//             Sim: Iterate over the allowed list (the nodes within the list is surely able to be added)
//            for (int i = 0; i < allowedAdd.Count; ++i)
//            {
//                int currNode = allowedAdd[i];
//                int degreeOfCurrentNode = 0;

//                 Sim: Iterate over the list whereby adding the node will increase the size of the clique
//                 Sim: But there is no promise that these nodes are not in the taboo list!
//                 Sim: GET THE DEGREE OF THE NODE TO THE NODES WITHIN THE POSSIBLE LIST

//                for (int j = 0; j < possibleAdd.Count; ++j) // check each node in possibleAdd
//                {
//                    int otherNode = possibleAdd[j];
//                    if (graph.AreAdjacent(currNode, otherNode) == true) ++degreeOfCurrentNode;
//                }

//                 Sim: Get the maximum degree of the node in the allowed list to the possible list
//                if (degreeOfCurrentNode > maxDegree)
//                    maxDegree = degreeOfCurrentNode;
//            }

//             now rescan allowedAdd and grab all nodes which have maxDegree
//            List<int> candidates = new List<int>();
//            for (int i = 0; i < allowedAdd.Count; ++i)
//            {
//                int currNode = allowedAdd[i];

//                int degreeOfCurrentNode = 0;

//                for (int j = 0; j < possibleAdd.Count; ++j) // check each node in possibleAdd
//                {
//                    int otherNode = possibleAdd[j];
//                    if (graph.AreAdjacent(currNode, otherNode) == true) ++degreeOfCurrentNode;
//                }

//                 Sim: Add only the nodes with the max degree (here results in not optimal decision, but it's ok)
//                if (degreeOfCurrentNode == maxDegree)
//                    candidates.Add(currNode);
//            }

//            if (candidates.Count == 0) throw new Exception("candidates List has size 0 just before return in GetNodeToAdd");

//             Return a random node within the possible candidates list
//            return candidates[random.Next(0, candidates.Count)]; // if candidates has Count 1 we'll get that one node

//        } // GetNodeToAdd

//         Sim: Get only one node to remove from the allowed list (not in taboo)
//        static int GetNodeToDrop(MyGraph graph, List<int> allowedInClique, List<int> oneMissing)
//        {
//             get a node from allowed clique set, which if dropped, gives the largest increase in PA set size
//             we use the oneMissing set to determine which clique node to pick
//            if (clique == null) throw new Exception("clique is null in GetNodeToDrop");
//            if (clique.Count == 0) throw new Exception("clique has Count 0 in GetNodeToDrop");

//             Sim: Return the only node
//            if (allowedInClique.Count == 1)
//                return allowedInClique[0];

//             scan each node in clique and determine the max possibleAdd size if node removed
//            int maxCount = 0; // see explanation below

//             Sim: Iterate over the nodes that are not in the taboo list, which is in the clique
//            for (int i = 0; i < allowedInClique.Count; ++i) // each node in clique nodes List
//            {
//                int currCliqueNode = allowedInClique[i];

//                int countNotAdjacent = 0;

//                 Sim: Iterate over the list of the nodes to be removed
//                for (int j = 0; j < oneMissing.Count; ++j) // each node in the one missing list
//                {
//                    int currOneMissingNode = oneMissing[j];

//                    if (graph.AreAdjacent(currCliqueNode, currOneMissingNode) == false) // we like this
//                        ++countNotAdjacent;

//                     if currCliqueNode is not connected to omNode then currCliqueNode is the 'missing'
//                     it would be good to drop this cliqueNode because after dropped from clique
//                     the remaining nodes in the clique will all be connected to the omNode
//                     and so the omNode would become a posibleAdd node and increase PA set size
//                     So the best node to drop from clique will be the one which is least connected
//                     to the nodes in OM
//                }


//                if (countNotAdjacent > maxCount)
//                    maxCount = countNotAdjacent;
//            }

//             at this point we know what the max-not-connected count is but there could be several clique nodes which give that size
//            List<int> candidates = new List<int>();
//            for (int i = 0; i < allowedInClique.Count; ++i) // each node in clique
//            {
//                int currCliqueNode = allowedInClique[i];
//                int countNotAdjacent = 0;
//                for (int j = 0; j < oneMissing.Count; ++j) // each node in the one missing list
//                {
//                    int currOneMissingNode = oneMissing[j];
//                    if (graph.AreAdjacent(currCliqueNode, currOneMissingNode) == false)
//                        ++countNotAdjacent;
//                }

//                if (countNotAdjacent == maxCount) // cxurrent clique node has max count not connected
//                    candidates.Add(currCliqueNode);
//            }

//            if (candidates.Count == 0) throw new Exception("candidates List has size 0 just before return in GetNodeToDropFromAllowedInClique");
//            return candidates[random.Next(0, candidates.Count)]; // must have size of at least 1

//        } // GetNodeToDrop

//         ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

//         Sim: Remove one node from the graph (which connects to every node in the clique except one node)
//        static List<int> MakeOneMissing(MyGraph graph, List<int> clique)
//        {
//             make a list of nodes in graph which are connected to all but one of the nodes in clique
//            int count; // number of nodes in clique which are connected to a candidate node. if final count == (clique size - 1) then candidate is a winner
//            List<int> result = new List<int>();

//             Sim: Loop through every node in the graph
//            for (int i = 0; i < graph.NumberNodes; ++i) // each node in graph i a candidate
//            {
//                count = 0;

//                 Sim: The node can only possibly connect to one node within the clique, skip it
//                if (graph.NumberNeighbors(i) < clique.Count - 1) continue; // node i has too few neighbors to possibly be connected to all but 1 node in clique

//                if (LinearSearch(clique, i) == true) continue; // node i is in clique. clique is not sorted so use LinearSearch -- consider Sort + BinarySearch

//                 The node is in the clique (skip 1)
//                if (clique.BinarySearch(i) >= 0) continue;

//                 Sim: Count how many nodes in the clique are connected to this node
//                for (int j = 0; j < clique.Count; ++j) // count how many nodes in clique are connected to candidate i
//                {
//                     Sim: JUMP
//                    if (graph.AreAdjacent(i, clique[j]))
//                        ++count;
//                }
//                if (count == clique.Count - 1)
//                    result.Add(i);
//            } // each candidate node i
//            return result;

//        } // MakeOneMissing


//         Sim: Return the nodes that are not in the taboo list from the listOfNodes
//        static List<int> SelectAllowedNodes(List<int> listOfNodes, int time, int prohibitPeriod, int[] lastMoved)
//        {
//            if (listOfNodes == null) throw new Exception("null listOfNodes in SelectAllowedNodes");

//            List<int> result = new List<int>();

//             Sim: No node can be added after all
//            if (listOfNodes.Count == 0) return result; // if the basis List has no nodes the subset List can't have any nodes

//             Sim: Check through every node in the list to make sure that they are not in the taboo list
//            for (int i = 0; i < listOfNodes.Count; ++i)
//            {
//                int currNode = listOfNodes[i];

//                 The taboo period has passed for the node
//                 Add the node into the list
//                if (time > lastMoved[currNode] + prohibitPeriod) // allowed because time is greater than time last moved plus the prohibit
//                    result.Add(currNode);
//            }

//            return result; // could have Count == 0
//        } // SelectAllowedNodes


//        static int UpdateProhibitPeriod(MyGraph graph, List<int> clique, int bestSize, Hashtable history, int time, int prohibitPeriod, ref int timeProhibitChanged)
//        {
//             increase or decrease the prohibition interval based on whether or not the current clique has been seen
//             before, and if seen, how recentl
//            int result = prohibitPeriod; // default return is no change

//            CliqueInfo cliqueInfo = new CliqueInfo(clique, time);
//            if (history.Contains(cliqueInfo.GetHashCode()) == true) // current clique is in the history hash table and so has been seen before
//            {
//                CliqueInfo ci = (CliqueInfo)history[cliqueInfo.GetHashCode()]; // to simplify
//                int intervalSinceLastVisit = time - ci.LastSeen; // how long has it been since the clique was last seen?
//                ci.LastSeen = time;
//                if (intervalSinceLastVisit < 2 * (graph.NumberNodes - 1)) // it has been a 'short' time since we last saw current clique (we're getting dups) so increase T to make more prohibited nodes which will decrease duplications of clique.
//                {
//                     inceasing the prohibit time
//                    timeProhibitChanged = time;
//                    if (prohibitPeriod + 1 < 2 * bestSize) // 2 * bestSize or something else
//                        return prohibitPeriod + 1; // new prohibit period
//                    else
//                        return 2 * bestSize; // maximum prohibit period
//                }
//            }
//            else // currentClique is not in hashTable and therefore has not been seen before
//            {
//                history.Add(cliqueInfo.GetHashCode(), cliqueInfo); // add currentClique and time to hashTable
//            }

//             at this point, either the clique has been seen before but was seen a 'long' time ago (so we didn't increase prohibit time)
//             or clique hasn't been seen before.
//             if it has been a long time since we made a change in prohibit period then we want to try and reduce it
//             because we will have a greater number of allowed nodes to choose from.

//            if ((time - timeProhibitChanged) > (10 * bestSize)) // we have not changed prohibitPeriod in a long time; let's decrease it to get more allowed nodes
//            {
//                timeProhibitChanged = time;
//                if (prohibitPeriod - 1 > 1)
//                    return prohibitPeriod - 1;
//                else
//                    return 1; // mijnimum prohibit period
//            }
//            else
//            {
//                return result; // the default value which is no change
//            }

//        } // UpdateprohibitPeriod

//        static string ListAsString(List<int> list)
//        {
//            string s = "";
//            for (int i = 0; i < list.Count; ++i)
//            {
//                if (i % 10 == 0 && i > 0) s += Environment.NewLine;
//                s += list[i] + " ";
//            }

//            s += Environment.NewLine;
//            return s;
//        } // ListAsString

//        static void ValidateState(MyGraph graph, List<int> clique, List<int> possibleAdd, List<int> oneMissing)
//        {
//             any duplicates in clique?
//            for (int i = 0; i < clique.Count - 1; ++i)
//            {
//                for (int j = i + 1; j < clique.Count; ++j)
//                {
//                    if (clique[i] == clique[j])
//                        throw new Exception("Dup value in clique");
//                }
//            }
//             any values in clique in possibleAdd or oneMissing?
//            for (int i = 0; i < possibleAdd.Count; ++i)
//            {
//                if (clique.BinarySearch(possibleAdd[i]) >= 0)
//                    throw new Exception("Possible Add value in clique");
//            }
//            for (int i = 0; i < oneMissing.Count; ++i)
//            {
//                if (clique.BinarySearch(oneMissing[i]) >= 0)
//                    throw new Exception("One Missing value in clique");
//            }
//             any values in possibleAdd in oneMissing?
//            for (int i = 0; i < possibleAdd.Count; ++i)
//            {
//                if (oneMissing.Contains(possibleAdd[i]))
//                    throw new Exception("Value in possibleAdd found in oneMissing");
//            }
//            for (int i = 0; i < oneMissing.Count; ++i)
//            {
//                if (possibleAdd.Contains(oneMissing[i]))
//                    throw new Exception("Value in oneMissing found in possibleAdd");
//            }
//        } // ValidateState

//         --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

//        private class CliqueInfo
//        {
//            private List<int> clique;
//            private int lastSeen;

//            public CliqueInfo(List<int> clique, int lastSeen)
//            {
//                this.clique = new List<int>();
//                this.clique.AddRange(clique);
//                this.lastSeen = lastSeen;
//            }

//            public int LastSeen
//            {
//                get { return this.lastSeen; }
//                set { this.lastSeen = value; }
//            }

//            public override int GetHashCode()
//            {
//                StringBuilder sb = new StringBuilder();
//                for (int i = 0; i < clique.Count; ++i)
//                {
//                    sb.Append(clique[i]);
//                    sb.Append(" ");
//                }

//                string s = sb.ToString();
//                return s.GetHashCode();
//            }

//            public override string ToString()
//            {
//                string s = "";
//                for (int i = 0; i < clique.Count; ++i)
//                    s += clique[i] + " ";
//                return s;
//            }

//        } // class CliqueInfo

//         --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

//    } // class TabuMaxCliqueProgram

//    public class MyGraph
//    {
//        private BitMatrix data;
//        private int numberNodes;
//        private int numberEdges;
//        private int[] numberNeighbors;

//        public MyGraph(string graphFile, string fileFormat)
//        {
//            if (fileFormat.ToUpper() == "DIMACS")
//                LoadDimacsFormatGraph(graphFile);
//            else
//                throw new Exception("Format " + fileFormat + " not supported");
//        }

//        private void LoadDimacsFormatGraph(string graphFile)
//        {
//            FileStream ifs = new FileStream(graphFile, FileMode.Open);
//            StreamReader sr = new StreamReader(ifs);
//            string line = "";
//            string[] tokens = null;

//             advance to and get the p line (ex: "p edge 9 16")
//            line = sr.ReadLine(); // read first line of file as a priming read
//            line = line.Trim();
//            while (line != null && line.StartsWith("p") == false)
//            {
//                line = sr.ReadLine();
//                line = line.Trim();
//            }

//            tokens = line.Split(' ');
//            int numNodes = int.Parse(tokens[2]); // number nodes
//            int numEdges = int.Parse(tokens[3]); // number edges

//            sr.Close(); ifs.Close();

//            this.data = new BitMatrix(numNodes);

//            ifs = new FileStream(graphFile, FileMode.Open); // reopen file
//            sr = new StreamReader(ifs);
//            while ((line = sr.ReadLine()) != null)
//            {
//                line = line.Trim();
//                if (line.StartsWith("e") == true) // (ex: "e 7 4")
//                {
//                    tokens = line.Split(' ');
//                    int nodeA = int.Parse(tokens[1]) - 1; // DIMACS is 1-based. subtract 1 to convert to 0-based
//                    int nodeB = int.Parse(tokens[2]) - 1;
//                    data.SetValue(nodeA, nodeB, true);
//                    data.SetValue(nodeB, nodeA, true);
//                }
//            }
//            sr.Close(); ifs.Close();

//            this.numberNeighbors = new int[numNodes];
//            for (int row = 0; row < numNodes; ++row)
//            {
//                int count = 0;
//                for (int col = 0; col < numNodes; ++col)
//                {
//                    if (data.GetValue(row, col) == true)
//                        ++count;
//                }
//                numberNeighbors[row] = count;
//            }

//            this.numberNodes = numNodes;
//            this.numberEdges = numEdges;
//            return;
//        }

//        public int NumberNodes
//        {
//            get { return this.numberNodes; }
//        }

//        public int NumberEdges
//        {
//            get { return this.numberEdges; }
//        }

//        public int NumberNeighbors(int node)
//        {
//            return this.numberNeighbors[node];
//        }


//         Sim: Determine if nodeA and nodeB are in the adjacency list (BitMatrix)
//        public bool AreAdjacent(int nodeA, int nodeB)
//        {
//            if (this.data.GetValue(nodeA, nodeB) == true) // Sim: If the table cell is marked with 1 (true)
//                return true;
//            else
//                return false;
//        }

//        public override string ToString()
//        {
//            string s = "";
//            for (int i = 0; i < this.data.Dim; ++i)
//            {
//                s += i + ": ";
//                for (int j = 0; j < this.data.Dim; ++j)
//                {
//                    if (this.data.GetValue(i, j) == true)
//                        s += j + " ";
//                }
//                s += Environment.NewLine;
//            }
//            return s;
//        }

//        public static void ValidateGraphFile(string graphFile, string fileFormat)
//        {
//            if (fileFormat.ToUpper() == "DIMACS")
//                ValidateDimacsGraphFile(graphFile);
//            else
//                throw new Exception("Format " + fileFormat + " not supported");
//        }

//        public static void ValidateDimacsGraphFile(string graphFile)
//        {
//            FileStream ifs = new FileStream(graphFile, FileMode.Open);
//            StreamReader sr = new StreamReader(ifs);
//            string line = "";
//            string[] tokens = null;

//            while ((line = sr.ReadLine()) != null)
//            {
//                line = line.Trim();
//                if (line.StartsWith("c") == false && line.StartsWith("p") == false &&
//                  line.StartsWith("e") == false)
//                    throw new Exception("Unknown line type: " + line + " in ValidateDimacsGraphFile");

//                try
//                {
//                    if (line.StartsWith("p"))
//                    {
//                        tokens = line.Split(' ');
//                        int numNodes = int.Parse(tokens[2]);
//                        int numEdges = int.Parse(tokens[3]);
//                    }
//                    else if (line.StartsWith("e"))
//                    {
//                        tokens = line.Split(' ');
//                        int nodeA = int.Parse(tokens[1]);
//                        int nodeB = int.Parse(tokens[2]);
//                    }
//                }
//                catch
//                {
//                    throw new Exception("Error parsing line = " + line + " in ValidateDimacsGraphFile");
//                }
//            }

//            sr.Close();
//            ifs.Close();
//            return;
//        } // ValidateDimacsGraphFile

//        public void ValidateGraph()
//        {
//             total number edges items must be an even number
//            int numConnections = 0;
//            for (int i = 0; i < this.data.Dim; ++i)
//            {
//                for (int j = 0; j < this.data.Dim; ++j)
//                {
//                    if (this.data.GetValue(i, j) == true)
//                        ++numConnections;
//                }
//            }
//            if (numConnections % 2 != 0)
//                throw new Exception("Total number of connections in graph is " + numConnections + ". Should be even");

//             fully symmetric
//            for (int i = 0; i < this.data.Dim; ++i)
//            {
//                if (this.data.GetValue(i, i) == true)
//                    throw new Exception("Node " + i + " is connected to itself");
//                for (int j = 0; j < this.data.Dim; ++j)
//                {
//                    if (this.data.GetValue(i, j) != this.data.GetValue(j, i))
//                        throw new Exception("Graph is not symmetric at " + i + " and " + j);
//                }
//            }
//            return;
//        } // ValidateGraph

//         ---------------------------------------------------------------------------------------------------------------------------------------------------------

//        private class BitMatrix
//        {
//            private BitArray[] data;
//            public readonly int Dim;

//            public BitMatrix(int n)
//            {
//                this.data = new BitArray[n];
//                for (int i = 0; i < data.Length; ++i)
//                {
//                    this.data[i] = new BitArray(n);
//                }
//                this.Dim = n;
//            }
//            public bool GetValue(int row, int col)
//            {
//                return data[row][col];
//            }
//            public void SetValue(int row, int col, bool value)
//            {
//                data[row][col] = value;
//            }
//            public override string ToString()
//            {
//                string s = "";
//                for (int i = 0; i < data.Length; ++i)
//                {
//                    for (int j = 0; j < data[i].Length; ++j)
//                    {
//                        if (data[i][j] == true) s += "1 "; else s += "0 ";
//                    }
//                    s += Environment.NewLine;
//                }
//                return s;
//            }

//        } // class BitMatrix

//    } // class MyGraph

//} // ns
