//using System;
//using System.Collections.Generic;

//namespace Clique
//{
//    class Driver
//    {
//public static void Main()
//{
//    // Infinite loop to be terminated by the user
//    for (int count = 1; ; ++count) 
//    {
//        // Instantiate the graph generator and generate the graph
//        Generator generator = new Generator();
//        Graph graph = generator.GenerateTotalNodes(count, false);

//        // Perform the Tabu search to find the maximum clique
//        DateTime tabuStart = HighResolutionClock.UtcNow;
//        Tabu tabuSearch = new Tabu(graph, 500);
//        List<int> maxClique = tabuSearch.FindMaximumClique();
//        DateTime tabuEnd = HighResolutionClock.UtcNow;

//        // Perform the k-clique search with the maximum clique determined
//        DateTime exhaustiveStart = HighResolutionClock.UtcNow;
//        Exhaustive exhaustiveSearch = new Exhaustive(graph);
//        bool kClique = exhaustiveSearch.FindKClique(maxClique.Count, true);
//        DateTime exhaustiveEnd = HighResolutionClock.UtcNow;

//        // Perform the (k + 1)-clique search to verify if the maximum clique is correct
//        DateTime exhaustiveVerifierStart = HighResolutionClock.UtcNow;
//        bool k1Clique = exhaustiveSearch.FindKClique(maxClique.Count + 1, true);
//        DateTime exhaustiveVerifierEnd = HighResolutionClock.UtcNow;

//        // Print out the result in a line
//        Console.WriteLine(
//            graph.NumberNodes + " " + // Number of nodes in the graph
//            graph.NumberEdges + " " + // Number of edges in the graph
//            maxClique.Count + " " +  // Maximum clique determined
//            kClique + " " + // Is the k-clique found?
//            k1Clique + " " + // Is the (k + 1)-clique found?
//            (tabuEnd - tabuStart) + " " + // Time taken for the Tabu search
//            (exhaustiveEnd - exhaustiveStart) + " " + // Time taken for the exhaustive (k) search
//            (exhaustiveVerifierEnd - exhaustiveVerifierStart) + " " + // Time taken for the exhaustive (k + 1) search
//            (kClique && !k1Clique)); // Is the maximum clique accurate? A k-clique must exist while a (k + 1)-clique must not exist
//    }
//}
//    }
//}
