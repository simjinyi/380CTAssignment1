using System;
using System.Collections.Generic;

namespace Clique
{
    class Driver
    {
        public static void Main()
        {
            Generator generator = new Generator();
            Graph graph = generator.GeneratePerNodeEdges(11);

            Exhaustive exhaustiveSearch = new Exhaustive(graph);

            Tabu tabuSearch1 = new Tabu(graph, 100000);
            List<int> aa = tabuSearch1.FindMaximumClique();
            int maxSize = aa.Count;

            Console.WriteLine(maxSize);

            while (!(exhaustiveSearch.FindKClique(maxSize, true) && !exhaustiveSearch.FindKClique(maxSize + 1, true)))
            {
                aa = tabuSearch1.FindMaximumClique();
                maxSize = aa.Count;
                Console.WriteLine(maxSize);
            }

            int[] x = new int[100];

            Console.WriteLine("Testing");
            for (int count = 1; count < 1000; count += 1)
            {
                //Console.WriteLine(graph);

                //for (int i = 0; i < graph.NumberNodes; ++i)
                //    if (graph.NumberNeighbors(i) < 1)
                //        Console.WriteLine("Node 0 Degree: " + i);

                //Console.WriteLine("Testing Graph with " + graph.NumberNodes + " nodes and " + graph.NumberEdges + " edges");

                DateTime tabuStart = HighResolutionClock.UtcNow;
                Tabu tabuSearch = new Tabu(graph, count);
                List<int> maxClique = tabuSearch.FindMaximumClique();

                //Console.WriteLine("Maximum Clique of Size " + maxClique.Count + " Found");
                //Console.WriteLine("Maximum Clique Enumeration: [{0}]", string.Join(", ", maxClique));
                DateTime tabuEnd = HighResolutionClock.UtcNow;

                //Console.WriteLine();

                //// Console.WriteLine("Optimized");
                //DateTime exhaustiveStart = HighResolutionClock.UtcNow;
                //Exhaustive exhaustiveSearch = new Exhaustive(graph);
                //bool kClique = exhaustiveSearch.FindKClique(count, true);
                ////Console.WriteLine("K-Clique of Size " + (maxClique.Count) + (kClique ? " Found" : " Not Found"));
                //DateTime exhaustiveEnd = HighResolutionClock.UtcNow;

                //// Console.WriteLine("Not Optimized");
                //DateTime exhaustiveVerifierStart = HighResolutionClock.UtcNow;
                //bool k1Clique = exhaustiveSearch.FindKClique(count + 1, true);
                ////Console.WriteLine("K-Clique of Size " + (maxClique.Count + 1) + (k1Clique ? " Found" : " Not Found"));
                //DateTime exhaustiveVerifierEnd = HighResolutionClock.UtcNow;

                //Console.WriteLine();

                //Console.WriteLine("Time Taken for Tabu Search: " + (tabuEnd - tabuStart));
                //Console.WriteLine("Time Taken for Exhaustive Search: " + (exhaustiveEnd - exhaustiveStart));
                //Console.WriteLine("Time Taken for Exhaustive Search (Verifier): " + (exhaustiveVerifierEnd - exhaustiveVerifierStart));

                // Console.WriteLine(graph.NumberNodes + " " + graph.NumberEdges + " " + count + " " + maxSize + " " + maxClique.Count + " " + (maxSize == maxClique.Count));
                //Console.WriteLine();

                if (maxSize != maxClique.Count)
                    x[count / 100]++;
            }

            Console.WriteLine(graph.NumberNodes + " " + graph.NumberEdges + maxSize);
            Console.WriteLine("[{0}]", string.Join(", ", x));
        }
    }
}
