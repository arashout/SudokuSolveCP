namespace SudokuSolve
{
    interface IBoard
    {
        void Print();
        int Get(int r, int c);
        void Set(int r, int c, int n);
    }
}
