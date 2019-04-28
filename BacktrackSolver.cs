
using System;
using System.Diagnostics;

namespace SudokuSolve {
    class BackTrackSolver : ISolver {
        public bool Solve(SimpleBoard board) {
            if(board.IsFull()) return true;
            Debug.Assert(board.IsValidBoard());
            
            int r,c;
            findFreeTile(board, out r, out c);
            // Brute-Force all possibilities
            for (int i = 1; i <= Constants.N; i++)
            {
                if(board.Set(r, c, i)){
                    if(Solve(board)) return true;
                    else board.Set(r, c, 0);
                }
            }
            return false;
        }
        private void findFreeTile(SimpleBoard board, out int r, out int c){
            for(int i = 0; i < Constants.N; i++){
                for(int j = 0; j < Constants.N; j++){
                    if(board.Tiles[i,j] == 0){
                        r = i;
                        c = j;
                        return;
                    }
                }
            }
            // Maybe I should throw exception?
            r = -1;
            c = -1;
        }
    }
}