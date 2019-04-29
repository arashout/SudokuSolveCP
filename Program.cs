using System;

namespace SudokuSolve {
    static class Constants {
        public const int N = 9; // Board Size
        public const int W = 3; // Square Width
    }

    class Program {
        static void Main (string[] args) {
            // Handle piped input
            Console.SetIn (new System.IO.StreamReader (Console.OpenStandardInput (8192))); // This will allow input >256 chars
            while (Console.In.Peek () != -1) {
                string line = Console.In.ReadLine ();

                if (line.Contains ("Grid")) continue;
                SimpleBoard b = new SimpleBoard (line);
                b.Print (true);
                Console.WriteLine ();
                BackTrackSolver s = new BackTrackSolver ();
                s.Solve (b);
                b.Print ();
                Console.WriteLine (new String ('-', 20));

                CPSolver cps = new CPSolver(b);
                break;
            }
        }
    }
}