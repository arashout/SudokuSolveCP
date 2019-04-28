using System;
using System.Diagnostics;

namespace SudokuSolve {
    class SimpleBoard {
        public int[, ] Tiles { get; }
        public bool[, ] FixedTiles { get; }
        public SimpleBoard (string tileString) {
            Tiles = new int[Constants.N, Constants.N];
            FixedTiles = new bool[Constants.N, Constants.N];
            for (int i = 0; i < tileString.Length; i++) {
                int n = UtilService.parseChar (tileString[i]);
                if (n != 0) {
                    Tiles[i / Constants.N, i % Constants.N] = n;
                    FixedTiles[i / Constants.N, i % Constants.N] = true;
                }
            }
        }

        public void Print (bool withFixed = false) {
            for (int i = 0; i < Constants.N; i++) {
                for (int j = 0; j < Constants.N; j++) {
                    if (withFixed) {
                        if (FixedTiles[i, j]) {
                            Console.Write ($"{Tiles[i, j]}f ");
                        } else {
                            Console.Write ($"{Tiles[i, j]}  ");
                        }
                    } else {
                        Console.Write ($"{Tiles[i, j]} ");
                    }
                }
                Console.WriteLine ();
            }
        }
        public int Get (int r, int c) {
            Debug.Assert (boundsCheck (r, c));
            return Tiles[r, c];
        }
        public bool Set (int r, int c, int n) {
            Debug.Assert (boundsCheck (r, c));
            if (FixedTiles[r, c]) {
                return false;
            }
            // Try to set the tile, if it invalidates the board then stop
            int temp = Tiles[r, c];
            Tiles[r,c] = n;
            if(IsValidBoard()) return true;
            else{
                Tiles[r, c] = temp;
                return false;
            }

        }
        private bool validatePeers (Func<int, int> ithPeerValue, bool partialAllowed = true) {
            bool[] seen = new bool[Constants.N + 1];
            for (int i = 0; i < Constants.N; i++) {
                int val = ithPeerValue (i);
                if (val == 0 && partialAllowed) continue;
                else if (val == 0) return false;
                else if (seen[val]) return false;
                else seen[val] = true;
            }
            return true;
        }
        private bool validRow (int r, bool partialAllowed = true) {
            return validatePeers ((i) => Get (r, i), partialAllowed);
        }
        private bool validColumn (int c, bool partialAllowed = true) {
            return validatePeers ((i) => Get (i, c), partialAllowed);
        }

        private bool validSquare (int squareNum, bool partialAllowed = true) {
            // Get major row and major column == Square Indices
            int rs = squareNum / Constants.W;
            int cs = squareNum % Constants.W;

            // Enumerate all peers
            bool[] seen = new bool[Constants.N + 1];
            for (int i = 0; i < Constants.W; i++) {
                int rt = rs * Constants.W + i;
                for (int j = 0; j < Constants.W; j++) {
                    int ct = cs * Constants.W + j;

                    int val = Get (rt, ct);
                    if (val == 0 && partialAllowed) continue;
                    else if (val == 0) return false;
                    else if (seen[val]) return false;
                    else seen[val] = true;
                }
            }
            return true;
        }

        public bool IsValidBoard () {
            for (int i = 0; i < Constants.N; i++) {
                if (validSquare (i, true) && validColumn (i, true) && validRow (i, true)) continue;
                else return false;
            }
            return true;
        }
        public bool IsFull () {
            for (int i = 0; i < Constants.N; i++) {
                if (validSquare (i, false) && validColumn (i, false) && validRow (i, false)) continue;
                else return false;
            }
            return true;
        }

        private bool boundsCheck (int r, int c) {
            if (r >= Constants.N || c >= Constants.N || r < 0 || c < 0) {
                throw new IndexOutOfRangeException ($"{r}:{c} is out of bounds");
            }
            return true;
        }
    }
}