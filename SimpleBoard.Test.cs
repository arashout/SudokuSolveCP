using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SudokuSolve {
    public class SimpleBoardTest {
        private class TestCase {
            public string TileString { get; }
            public int[, ] ExpectedTiles { get; }
            public string Message { get; }
            public bool IsValid { get; }
            public bool IsSolved { get; }
            public TestCase (
                string tilesString,
                int[, ] expectedTiles,
                string message = "",
                bool isValid = true,
                bool isSolved = false
            ) {
                TileString = string.Concat (tilesString.Where (c => !char.IsWhiteSpace (c)));
                ExpectedTiles = expectedTiles;
                Message = message;
                IsValid = isValid;
                IsSolved = isSolved;
            }
        }
        /* fixformat ignore:start */
        private TestCase[] testTable = new [] {
            new TestCase(@"
            000000000
            000000000
            000000000
            000000000
            000000000
            000000000
            000000000
            000000000
            000000000", 
            new int[Constants.N, Constants.N] { 
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 } 
            }, "Empty TileSet"),
            new TestCase(@"
            110000000
            000000000
            000000000
            000000000
            000000000
            000000000
            000000000
            000000000
            000000000", 
            new int[Constants.N, Constants.N] {
                { 1, 1, 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 
                { 0, 0, 0, 0, 0, 0, 0, 0, 0 } 
            }, "Same row 1", false),
            new TestCase(@"
            743951682
            162487395
            958632714
            219873546
            374569128
            586124973
            495216837
            827395461
            631748259", 
            new int[Constants.N, Constants.N] {
                {7,4,3,9,5,1,6,8,2},
                {1,6,2,4,8,7,3,9,5},
                {9,5,8,6,3,2,7,1,4},
                {2,1,9,8,7,3,5,4,6},
                {3,7,4,5,6,9,1,2,8},
                {5,8,6,1,2,4,9,7,3},
                {4,9,5,2,1,6,8,3,7},
                {8,2,7,3,9,5,4,6,1},
                {6,3,1,7,4,8,2,5,9}
            }, "Solved", true, true)
        };
        /* fixformat ignore:end */

        [Fact]
        public void TestSimpleBoard () {

            foreach (var test in testTable) {
                SimpleBoard b = new SimpleBoard (test.TileString);
                Assert.True (TestUtilService.tilesCompare (b.Tiles, test.ExpectedTiles), $"String tiles not read correctly for: {test.Message}");
                Assert.True (b.IsValidBoard () == test.IsValid, $"Validity does not match: {test.Message}");
                Assert.True (b.IsFull () == test.IsSolved, $"Solved does not match: {test.Message}");
            }
        }

        [Fact]
        public void TestSolve () {
            SimpleBoard b = new SimpleBoard ("003020600900305001001806400008102900700000008006708200002609500800203009005010300");
            b.Print ();
            BackTrackSolver s = new BackTrackSolver ();
            s.Solve (b);
            b.Print ();
        }
    }
}