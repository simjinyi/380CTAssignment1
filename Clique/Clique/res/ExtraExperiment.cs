//using System;
//using System.Collections.Generic;

//namespace Clique
//{
//    class Driver
//    {
//        public static void Main()
//        {
//            for (int count = 2; ; ++count)
//            {
//                Generator generator = new Generator();
//                Graph graph = generator.GenerateTotalNodes(count, false);

//                //Console.WriteLine(graph);

//                //for (int i = 0; i < graph.NumberNodes; ++i)
//                //    if (graph.NumberNeighbors(i) < 1)
//                //        Console.WriteLine("Node 0 Degree: " + i);

//                //Console.WriteLine("Testing Graph with " + graph.NumberNodes + " nodes and " + graph.NumberEdges + " edges");

//                DateTime tabuStart = HighResolutionClock.UtcNow;
//                Tabu tabuSearch = new Tabu(graph, 500);
//                List<int> maxClique = tabuSearch.FindMaximumClique();

//                //Console.WriteLine("Maximum Clique of Size " + maxClique.Count + " Found");
//                //Console.WriteLine("Maximum Clique Enumeration: [{0}]", string.Join(", ", maxClique));
//                DateTime tabuEnd = HighResolutionClock.UtcNow;

//                //Console.WriteLine();

//                // Console.WriteLine("Optimized");
//                DateTime exhaustiveStart = HighResolutionClock.UtcNow;
//                Exhaustive exhaustiveSearch = new Exhaustive(graph);
//                bool kClique = exhaustiveSearch.FindKClique(count, true);
//                //Console.WriteLine("K-Clique of Size " + (maxClique.Count) + (kClique ? " Found" : " Not Found"));
//                DateTime exhaustiveEnd = HighResolutionClock.UtcNow;

//                // Console.WriteLine("Not Optimized");
//                DateTime exhaustiveVerifierStart = HighResolutionClock.UtcNow;
//                bool k1Clique = exhaustiveSearch.FindKClique(count + 1, false);
//                //Console.WriteLine("K-Clique of Size " + (maxClique.Count + 1) + (k1Clique ? " Found" : " Not Found"));
//                DateTime exhaustiveVerifierEnd = HighResolutionClock.UtcNow;

//                //Console.WriteLine();

//                //Console.WriteLine("Time Taken for Tabu Search: " + (tabuEnd - tabuStart));
//                //Console.WriteLine("Time Taken for Exhaustive Search: " + (exhaustiveEnd - exhaustiveStart));
//                //Console.WriteLine("Time Taken for Exhaustive Search (Verifier): " + (exhaustiveVerifierEnd - exhaustiveVerifierStart));

//                Console.WriteLine(graph.NumberNodes + " " + graph.NumberEdges + " " + (exhaustiveEnd - exhaustiveStart) + " " + (exhaustiveVerifierEnd - exhaustiveVerifierStart) + " " + !k1Clique);
//                //Console.WriteLine();
//            }
//        }
//    }
//}
