using System;
using System.Diagnostics;

namespace ReactSudoku {
    static class Const {
        public const int N = 9; // Board Size
        public const int W = 3; // Square Width
        public const int N2 = N * N;
    }
    static class Utils {
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

        public static int CalculateTileIndex (int r, int c) {
            return r * Const.N + c % Const.N;
        }
        public static string CalculateTileString (int i) {
            return $"{i/Const.N}{i%Const.N}";
        }
    }
    /*
    Need those since Debug.Assert(false) is not behaving properly
     */
    public static class CDebug {
        [Conditional ("DEBUG")]

        public static void Assert (this bool condition, string message = null) {
            if (!condition) {
                throw new Exception ($"Assertion failed with message:\n{message}");
            };
        }
    }

    static class TestUtils {
        public static bool tilesCompare (int[, ] a, int[, ] b) {
            for (int i = 0; i < Const.N; i++) {
                for (int j = 0; j < Const.N; j++) {
                    if (a[i, j] != b[i, j]) return false;
                }
            }
            return true;
        }
    }

}