namespace Ostryzhnyi.AdaptiveGrid.CodeBase.Field.Bricks
{
    public static class ArrayRotator
    {
        public static T[,] Rotate90<T>(T[,] array)
        {
            int rows = array.GetLength(0);
            int cols = array.GetLength(1);
            T[,] rotatedArray = new T[cols, rows];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    rotatedArray[j, rows - 1 - i] = array[i, j];
                }
            }

            return rotatedArray;
        }

        public static T[,] RotateMinus90<T>(T[,] array)
        {
            int rows = array.GetLength(0);
            int cols = array.GetLength(1);
            T[,] rotatedArray = new T[cols, rows];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    rotatedArray[cols - 1 - j, i] = array[i, j];
                }
            }

            return rotatedArray;
        }

        public static T[,] Rotate180<T>(T[,] array)
        {
            int rows = array.GetLength(0);
            int cols = array.GetLength(1);
            T[,] rotatedArray = new T[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    rotatedArray[rows - 1 - i, cols - 1 - j] = array[i, j];
                }
            }

            return rotatedArray;
        }
    }
}