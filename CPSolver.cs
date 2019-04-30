using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolve {
    static class CPSolver {
        // TODO: Could use readonly here -> Need to update language version
        // TODO: Make these private
        static public HashSet<int>[] Peers { get; }
        static public HashSet<int>[] Units { get; }

        static CPSolver () {
            // Initialize hashsets
            Peers = new HashSet<int>[Const.N2];
            Units = new HashSet<int>[Const.N * 3];
            for (int i = 0; i < Peers.Length; i++) {
                Peers[i] = new HashSet<int> ();
            }
            for (int i = 0; i < Units.Length; i++) {
                Units[i] = new HashSet<int> ();
            }
            populatePeers ();
        }

        // TODO: Rename? Also make private for only using in solve?
        static internal bool InitializeConstraints (CPBoard cpb) {
            for (int i = 0; i < Const.N2; i++) {
                var value = cpb.Get (i);
                if (value.Length == 1) {
                    if (!Assign (cpb, i, value)) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Assign a single value to the tile, propagate the changes via calls to Eliminate
        /// NOTE: Method mutates the CPBoard cpb parameter
        /// </summary>
        /// <param name="cpb"></param>
        /// <param name="tileIndex"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        static public bool Assign (CPBoard cpb, int tileIndex, string value) {
            CDebug.Assert (value.Length == 1, "Cannot assign multiple values");
            var values = cpb.Get (tileIndex);
            // Cannot assign a value that is a not possible value of the tile
            if (!values.Contains (value)) return false;

            // Attempt to eliminate values from peers
            cpb.Set (tileIndex, value);
            foreach (var peer in Peers[tileIndex]) {
                if (!Eliminate (cpb, peer, value)) return false;
            }

            return true;
        }

        /// <summary>
        /// Eliminate will remove the value from the tile and propagate changes via calls to Assign
        /// if there is only a single value remaining
        /// NOTE: This method mutates the CPBoard cpb parameter
        /// </summary>
        /// <param name="cpb"></param>
        /// <param name="tile"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        static public bool Eliminate (CPBoard cpb, int tileIndex, string value) {
            var values = cpb.Get (tileIndex);
            CDebug.Assert (values.Length > 0, "values.Length > 0");

            if (values.Length == 1) {
                // Reached a contradiction, eliminating this value will leave the tile EMPTY!
                if (values == value) return false;
                // Value has already been eliminated from this tile
                return true;
            }

            var newValues = values.Replace (value, "");
            cpb.Set (tileIndex, newValues);
            // If there is only one possible value after the change 
            // Call assign to propagate changes
            if (newValues.Length == 1) {
                return Assign (cpb, tileIndex, newValues);
            }

            return true;
        }
        /// <summary>
        /// SearchResult struct is used to return the new board
        /// as well as the result of the search used for decision making
        /// </summary>
        public struct SearchResult {
            public CPBoard CPB;
            public bool DidSolve;
            public SearchResult (CPBoard cpb, bool didSolve) {
                CPB = cpb;
                DidSolve = didSolve;
            }
        }

        /// <summary>
        /// Exhaustively try all possible values for the tiles, using constraints to 
        /// reduce the search space
        /// </summary>
        /// <param name="cpb"></param>
        /// <param name="depth"></param> Parameter debugging and curiousity
        /// <returns></returns>
        static private SearchResult search (CPBoard cpb, int depth) {
            var freeTile = cpb.GetFreeTile ();
            // No free tile available
            if (freeTile.Value == null) {
                return new SearchResult (cpb, IsSolved (cpb));
            }
            var possibleValues = freeTile.Value;

            foreach (var c in possibleValues) {
                // Make a copy in case we reach contradiction and need to revert back
                var copy = cpb.Copy ();
                bool didAssign = Assign (copy, freeTile.Key, c.ToString ());
                CDebug.Assert (copy.Get (freeTile.Key) != cpb.Get (freeTile.Key), "Original CPB mutated!");
                if (didAssign) {
                    SearchResult sr = search (copy, depth + 1);
                    if (sr.DidSolve) return sr;
                }
            }
            return new SearchResult (cpb, false);
        }
        /// <summary>
        /// Helper function for the search function
        /// </summary>
        /// <param name="cpb"></param>
        /// <returns></returns>
        static public SearchResult Solve (CPBoard cpb) {
            if (!InitializeConstraints (cpb)) {
                return new SearchResult (cpb, false);
            }
            return search (cpb, 0);
        }

        /// <summary>
        /// For a the puzzle to be solved, each tile within a unit must have distinct value
        /// A unit is either:
        /// - a row (1x9)
        /// - a column (9x1)
        /// - a square (3x3)
        /// </summary>
        /// <returns></returns>
        static public bool IsSolved (CPBoard cpb) {
            foreach (var unit in Units) {
                var values = string.Join ("", unit.Select (tileIndex => cpb.Get (tileIndex)));
                if (values.Distinct ().Count () != values.Count ()) return false;
            }
            return true;
        }

        /// <summary>
        /// Function that initializes the internal representation of the constraint propagotion board
        /// </summary>
        static private void populatePeers () {
            var indices = Enumerable.Range (0, Const.N);
            // TODO: Maybe reduce some duplicate code here?
            foreach (var i in indices) {
                var rowPeers = (from c in indices select Utils.CalculateTileIndex (i, c)).ToHashSet ();
                foreach (var rp in rowPeers) {
                    Peers[rp] = Peers[rp].Union (rowPeers).Except (new int[] { rp }).ToHashSet ();
                    Units[i] = rowPeers;
                }
                var columnPeers = (from r in indices select Utils.CalculateTileIndex (r, i)).ToHashSet ();
                foreach (var cp in columnPeers) {
                    Peers[cp] = Peers[cp].Union (columnPeers).Except (new int[] { cp }).ToHashSet ();
                    Units[i + Const.N] = columnPeers;
                }
                // Add square peers
                int rs = i / Const.W;
                int cs = i % Const.W;
                var subRows = Enumerable.Range (0, Const.W).Select (j => rs * Const.W + j);
                var subColumns = Enumerable.Range (0, Const.W).Select (j => cs * Const.W + j);
                var squarePeers = (from r in subRows from c in subColumns select Utils.CalculateTileIndex (r, c)).ToHashSet ();
                foreach (var sp in squarePeers) {
                    Peers[sp] = Peers[sp].Union (squarePeers).Except (new int[] { sp }).ToHashSet ();
                    Units[i + Const.N * 2] = squarePeers;
                }

                CDebug.Assert (Peers[i].Count == 20, $"Should be 20 peers each tile- {string.Join(',', Peers[i])}");
            }
        }
        /// <summary>
        /// For DEBUGGING purposes
        /// </summary>
        static public void PrintPeers () {
            for (int i = 0; i < Peers.Length; i++) {
                Console.WriteLine ($"{i}: {string.Join(',', Peers[i])}");
            }
        }

        static public void PrintUnits () {
            for (int i = 0; i < Units.Length; i++) {
                Console.WriteLine ($"{i}: {string.Join(',', Units[i])}");
            }
        }
    }

    class CPBoard {

        // Tiles will contain all the possible values without constraints
        // Basically as defined by user
        private string[] Tiles = new string[Const.N2];

        public CPBoard (string board) {
            var indices = Enumerable.Range (0, Const.N);

            CDebug.Assert (board.Length == Const.N2, "board.Length == Const.N2");
            for (int i = 0; i < board.Length; i++) {
                int n = Utils.parseChar (board[i]);
                if (n != 0) {
                    Tiles[i] = n.ToString ();
                } else {
                    Tiles[i] = string.Join ("", indices.Select ((num) => num + 1));
                }
            }
        }
        /// <summary>
        /// Returns a tile that is still undecided in terms of it's final value
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<int, string> GetFreeTile () {
            return Tiles
                .Select ((values, i) => new KeyValuePair<int, string> (i, values))
                .Where (kvp => kvp.Value.Length > 1)
                .FirstOrDefault ();
        }

        /// <summary>
        /// Constructor used for creating deep copies
        /// </summary>
        /// <param name="tiles"></param>
        private CPBoard (string[] tiles) {
            for (int i = 0; i < tiles.Length; i++) {
                Tiles[i] = String.Copy (tiles[i]);
            }
        }
        /// <summary>
        /// Deep copy function that uses the private constructor
        /// Useful reverting calls from Assign and Eliminate
        /// Copy function is a convience function for the private constructor
        /// </summary>
        /// <returns></returns>
        public CPBoard Copy () {
            CPBoard copy = new CPBoard (Tiles);
            return copy;
        }

        ///
        /// Functions below are largely convenience functions
        /// Useful for debugging and observation

        public string Get (int index) {
            return Tiles[index];
        }
        public void Set (int index, string value) {
            Tiles[index] = value;
        }

        /// <summary>
        /// Print the grid representation
        /// A '-' represents that there are additional values that could be chosen
        /// </summary>
        public void Print () {
            for (int i = 0; i < Tiles.Length; i++) {
                var values = Tiles[i];
                var padding = new String (' ', Const.N - values.Length);
                var first = values.First ();
                var remaining = string.Join ("", values.TakeLast (values.Count () - 1));
                if (i % Const.N == 0) Console.Write ($"\n{first}({remaining}){padding}");
                else Console.Write ($" {first}({remaining}){padding}");
            }
            Console.WriteLine ();
        }
        public override string ToString () {
            return string.Join ("\n", Tiles.Select ((values, i) => $"{Utils.CalculateTileString(i)} : {values}"));
        }

    }
}