using System;

namespace SudokuSolve
{
    static class Constants
    {
        public const int N = 9; // Board Size
    }
    class SimpleBoard : IBoard
    {
        public int[,] Tiles = new int[9, 9];
        public SimpleBoard(string tileString)
        {
            for (int i = 0; i < tileString.Length; i++)
            {
                int n;
                char c = tileString[i];
                if (!int.TryParse(c.ToString(), out n))
                {
                    throw new ArgumentException(c.ToString() + " :is not a valid number");
                }
                Tiles[i / Constants.N, i % Constants.N] = n;
            }
        }
        public void Print()
        {
            for (int i = 0; i < Constants.N; i++)
            {
                for (int j = 0; j < Constants.N; j++)
                {
                    Console.Write(Tiles[i, j].ToString() + ' ');
                }
                Console.WriteLine();
            }
        }
        public int Get(int r, int c)
        {
            // TODO: Add out of bounds check
            return Tiles[r, c];
        }
        public void Set(int r, int c, int n)
        {
            Tiles[r, c] = n;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Small work-around for not suspending application for input when data not piped
            // https://stackoverflow.com/a/4074212/5258887
            String pipedText = "";
            bool isKeyAvailable;
            try
            {
                isKeyAvailable = System.Console.KeyAvailable;
                IBoard b = new SimpleBoard("003020600900305001001806400008102900700000008006708200002609500800203009005010300");
                b.Print();
            }
            catch (InvalidOperationException expected)
            {
                pipedText = System.Console.In.ReadToEnd();
                string[] lines = pipedText.Split('\n');
                foreach (var line in lines)
                {
                    if (line.Contains("Grid")) continue;
                    IBoard b = new SimpleBoard(line);
                    b.Print();
                    break;


                }
            }
        }
    }
}