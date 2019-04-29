using System.Collections.Generic;
using System.Linq;

namespace SudokuSolve {
    class CPSolver : ISolver {
        public CPSolver (SimpleBoard board) {
            new CPBoard (board);
        }
        public bool Solve (SimpleBoard board) {
            new CPBoard (board);
            return true;
        }
    }

    class CPBoard {
        // Tiles will contain all the possible values for each time
        public Dictionary<string, HashSet<int>> Tiles { get; }
        // Peers will contain all the peers that must maintain the unique number invariant
        public Dictionary<string, HashSet<string>> Peers { get; }

        public CPBoard (SimpleBoard board) {
            var indices = Enumerable.Range (0, Constants.N);

            Tiles = (from r in indices from c in indices select $"{r}{c}")
                .ToDictionary (t => t, _ => indices.ToHashSet ());
            Peers = (from t in Tiles.Keys select t)
                .ToDictionary (t => t, _ => new HashSet<string> ());

            // @TODO: Anyway to reduce this?
            // Add all peers
            foreach (var i in indices) {
                var rowPeers = (from c in indices select $"{i}{c}").ToHashSet ();
                foreach (var rp in rowPeers) {
                    Peers[rp].UnionWith (rowPeers);
                }
                var columnPeers = (from r in indices select $"{r}{i}").ToHashSet ();
                foreach (var cp in columnPeers) {
                    Peers[cp].UnionWith (columnPeers);
                }
                // Add square peers
                int rs = i / Constants.W;
                int cs = i % Constants.W;
                var subRows = Enumerable.Range (0, Constants.W).Select (j => rs * Constants.W + j);
                var subColumns = Enumerable.Range (0, Constants.W).Select (j => cs * Constants.W + j);
                var squarePeers = (from r in subRows from c in subColumns select $"{r}{c}").ToHashSet ();
                foreach (var st in squarePeers) {
                    Peers[st].UnionWith (squarePeers);
                }

            }

            foreach (KeyValuePair<string, HashSet<string>> kvp in Peers) {
                System.Console.WriteLine ($"{kvp.Key}:  {string.Join(",", kvp.Value)}");
            }

            // Might 
        }
    }
}