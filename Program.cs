using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SudokuSolve {

    class Program {
        static void Main (string[] args) {
            Console.WriteLine (string.Join (',', args));
            if (args.Length > 0) {
                IList<string> gridStrings = new List<string> ();
                string line;

                string fileName = @"puzzles.txt";
                System.IO.StreamReader file = new System.IO.StreamReader (fileName);
                while ((line = file.ReadLine ()) != null) {
                    if (line.Contains ("Grid")) continue;
                    else {
                        gridStrings.Add (line.Trim ());
                    }
                    System.Console.WriteLine (line);
                }

                file.Close ();
                System.Console.WriteLine ("{0} in {1}", gridStrings.Count, fileName);

                // TODO: Do initial solve to reduce jitter

                Console.WriteLine ("Starting benchmarking");
                var solveTimes = new List<double> (new double[gridStrings.Count]);
                Stopwatch sw = new Stopwatch ();
                int numIterations = 10;
                for (int i = 0; i < numIterations; i++) {
                    for (int j = 0; j < gridStrings.Count; j++) {
                        var cpb = new CPBoard (gridStrings[j]);
                        cpb.Print ();

                        Console.WriteLine (new String ('-', 20));
                        sw.Restart ();
                        var sr = CPSolver.Solve (cpb);
                        sw.Stop ();
                        double ticks = sw.ElapsedTicks;
                        double seconds = ticks / Stopwatch.Frequency;
                        double milliseconds = (ticks / Stopwatch.Frequency) * 1000;
                        solveTimes[j] += milliseconds;

                        if (!sr.DidSolve) {
                            throw new Exception ($"Could not solve:\n {cpb.ToString()}");
                        }
                        sr.CPB.Print ();
                    }
                }
                var averageTimes = solveTimes.Select (st => st / numIterations).ToList ();
                var averageTime = averageTimes.Average ();
                Console.WriteLine ($"Average solve time was: {averageTime}");
                for (int i = 0; i < averageTimes.Count; i++) {
                    Console.WriteLine ($"Grid {i} : {averageTimes[i]}");
                }
            } else {
                string line;
                // Handle piped input
                Console.SetIn (new System.IO.StreamReader (Console.OpenStandardInput (8192))); // This will allow input >256 chars
                while (Console.In.Peek () != -1) {
                    line = Console.In.ReadLine ();

                    if (line.Contains ("Grid")) {
                        var puzzleName = line;
                        Console.WriteLine ($"Attempting ${puzzleName}");
                    } else {
                        var cpb = new CPBoard (line);
                        cpb.Print ();
                        Console.WriteLine (new String ('-', 20));
                        var sr = CPSolver.Solve (cpb);
                        if (!sr.DidSolve) {
                            throw new Exception ($"Could not solve:\n {cpb.ToString()}");
                        }
                        sr.CPB.Print ();
                    }
                }
            }
        }
    }
}