using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SudokuSolve {
    public class CPSolverTest {

        private const string bSolvableByCP = "003020600900305001001806400008102900700000008006708200002609500800203009005010300";
        private const string bSolved = "743951682162487395958632714219873546374569128586124973495216837827395461631748259";
        private const string bInvalid = "303020600900305001001806400008102900700000008006708200002609500800203009005010300";
        private const string bSolvable = "300200000000107000706030500070009080900020004010800050009040301000702000000008006";
        private readonly string bEmpty = new String ('0', Const.N2);

        // [Fact]
        // public void TestSolve () {
        //     var cpb = new CPBoard (bSolvable);
        //     // solver.PrintUnits();
        // }

        [Fact]
        public void TestCopy () {
            var cpb = new CPBoard (bSolvableByCP);
            CPBoard copy = cpb.Copy ();
            // Changing original does not mutate copy
            Assert.NotEqual ("4", copy.Get (0));
            cpb.Set (0, "4");
            Assert.NotEqual ("4", copy.Get (0));

            // Chaning copy does not change original
            Assert.NotEqual ("2", cpb.Get (1));
            copy.Set (1, "2");
            Assert.NotEqual ("2", cpb.Get (1));
        }

        [Fact]
        public void TestFreeTile () {
            var cpb = new CPBoard (bSolvableByCP);
            var ft = cpb.GetFreeTile ();
            Assert.Equal (0, ft.Key);
            Assert.Equal ("123456789", ft.Value); // No elimination has occured yet

            cpb = new CPBoard (bSolved);
            ft = cpb.GetFreeTile ();
            Assert.Equal (0, ft.Key); // TODO: Really need maybe type, 0 is not a good default
            Assert.Null (ft.Value);

        }

        [Fact]
        public void TestIsSolved () {
            var cpb = new CPBoard (bSolvableByCP);
            Assert.False (CPSolver.IsSolved (cpb));

            cpb = new CPBoard (bSolved);
            Assert.True (CPSolver.IsSolved (cpb));

            cpb = new CPBoard (bInvalid);
            Assert.False (CPSolver.IsSolved (cpb));
        }

        [Fact]
        public void TestBasicEliminate () {
            var cpb = new CPBoard (bEmpty);
            CPSolver.Eliminate (cpb, 0, "1");
            Assert.Equal ("23456789", cpb.Get (0));
        }

        [Fact]
        public void TestAssignEliminate () {
            var cpb = new CPBoard (bEmpty);
            CPSolver.Assign (cpb, 0, "1");
            Assert.Equal ("1", cpb.Get (0));

            // Check assignment propagates
            foreach (var peerIndex in CPSolver.Peers[0]) {
                Assert.Equal ("23456789", cpb.Get (peerIndex));
            }
            // Check overlap elimination
            CPSolver.Assign (cpb, Const.N2 - 1, "9"); // Last tile
            Assert.Equal ("2345678", cpb.Get (Const.N - 1)); // 8
            Assert.Equal ("2345678", cpb.Get (Const.N * Const.N - Const.N)); // 72
        }

        /// <summary>
        /// Obvious invalid assignments should not propagate
        /// TODO: Not sure if this is a useful test and is not necessary behavior I should depend on
        /// </summary>
        [Fact]
        public void TestInvalidAssign () {
            var cpb = new CPBoard (bEmpty);
            CPSolver.Assign (cpb, 0, "1");

            Assert.False (CPSolver.Assign (cpb, 1, "1"), "Made invalid assignment!");
            Assert.Equal ("23456789", cpb.Get (1)); // If assignment fails do not mutate CPBoard

            Assert.False (CPSolver.Assign (cpb, 0, "2"), "Assigned an impossible value");
            // Check that invalid assignment did not propagates
            foreach (var peerIndex in CPSolver.Peers[0]) {
                Assert.Equal ("23456789", cpb.Get (peerIndex));
            }
        }

        [Fact]
        public void TestInitializeConstraints () {
            // This board is solvable from the constraints alone
            var cpb = new CPBoard (bSolvableByCP);
            Assert.True (CPSolver.InitializeConstraints (cpb));
            Assert.True (CPSolver.IsSolved (cpb));

            cpb = new CPBoard (bInvalid);
            Assert.False (CPSolver.InitializeConstraints (cpb));

            cpb = new CPBoard (bEmpty);
            Assert.True (CPSolver.InitializeConstraints (cpb));
            for (int i = 0; i < Const.N2; i++) {
                Assert.Equal ("123456789", cpb.Get (i));
            }

            cpb = new CPBoard ("1234" + bEmpty.Remove (0, 4));
            CPSolver.InitializeConstraints (cpb);
            Assert.Equal ("1", cpb.Get (0));
            Assert.Equal ("2", cpb.Get (1));
            Assert.Equal ("3", cpb.Get (2));
            Assert.Equal ("4", cpb.Get (3));
            Assert.Equal ("56789", cpb.Get (4));

            // TODO: Double check results
            cpb = new CPBoard (bSolvable);
            CPSolver.InitializeConstraints (cpb);
            Assert.Equal ("1458", cpb.Get (2));
        }

        [Fact]
        public void TestSolve () {
            var cpb = new CPBoard (bSolvableByCP);
            var sr = CPSolver.Solve (cpb);
            Assert.True (sr.DidSolve);
            Assert.True(CPSolver.IsSolved(sr.CPB));

            cpb = new CPBoard (bSolved);
            sr = CPSolver.Solve (cpb);
            Assert.True (sr.DidSolve);
            Assert.Equal ("7", cpb.Get (0));
            Assert.True(CPSolver.IsSolved(sr.CPB));

            // Double check results
            cpb = new CPBoard (bSolvable);
            sr = CPSolver.Solve (cpb);
            Assert.True (sr.DidSolve);
            Assert.True(CPSolver.IsSolved(sr.CPB));

            cpb = new CPBoard (bEmpty);
            sr = CPSolver.Solve (cpb);
            Assert.True (sr.DidSolve);
            Assert.True(CPSolver.IsSolved(sr.CPB));
        }
    }
}