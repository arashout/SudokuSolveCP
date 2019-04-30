using System;

namespace SudokuSolve {
    static class Const {
        public const int N = 9; // Board Size
        public const int W = 3; // Square Width
        public const int N2 = N*N;
    }
    class Program {
        static void Main (string[] args) {
            // Handle piped input
            Console.SetIn (new System.IO.StreamReader (Console.OpenStandardInput (8192))); // This will allow input >256 chars
            while (Console.In.Peek () != -1) {
                string line = Console.In.ReadLine ();

                if (line.Contains ("Grid")) {
                    Console.WriteLine(line);
                }
                else{
                    var cpb = new CPBoard(line);
                    cpb.Print();
                    Console.WriteLine (new String ('-', 20));
                    var sr = CPSolver.Solve(cpb);
                    if(!sr.DidSolve){
                        throw new Exception($"Could not solve:\n {cpb.ToString()}");
                    }
                    sr.CPB.Print();
                }
            }
        }
    }
}