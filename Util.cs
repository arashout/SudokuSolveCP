using System;

namespace SudokuSolve {
    static class UtilService {
        public static int parseChar (char c) {
            int n;
            if (!int.TryParse (c.ToString (), out n)) throw new ArgumentException ($"{c} :is not a integer");
            return n;
        }
        public static void Populate<T> (this T[] arr, T value) {
            for (int i = 0; i < arr.Length; i++) {
                arr[i] = value;
            }
        }
    }

    static class TestUtilService {
        public static bool tilesCompare (int[, ] a, int[, ] b) {
            for (int i = 0; i < Constants.N; i++) {
                for (int j = 0; j < Constants.N; j++) {
                    if (a[i, j] != b[i, j]) return false;
                }
            }
            return true;
        }
    }

}