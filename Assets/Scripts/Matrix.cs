using System;

public class Matrix
{
    public float[,] matrix;

    public Matrix(Matrix otherMatrix)
    {
        matrix = otherMatrix.matrix;
    }

    public Matrix(float[,] otherMatrix)
    {
        matrix = otherMatrix;
    }

    public Matrix(int rows, int cols, float element)
    {
        matrix = new float[rows, cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                matrix[i, j] = element;
            }
        }
    }

    public Matrix(int rows, int cols, float[] elements)
    {
        matrix = new float[rows, cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                matrix[i, j] = elements[(i * cols) + (j)];
            }
        }
    }

    public Matrix(int rows, int cols, int[] elements)
    {
        matrix = new float[rows, cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                matrix[i, j] = elements[(i * cols) + (j)];
            }
        }
    }

    public static Matrix operator +(Matrix matrix, Matrix otherMatrix)
    {
        int rows = matrix.matrix.GetLength(0);
        int cols = matrix.matrix.GetLength(1);
        int otherRows = otherMatrix.matrix.GetLength(0);
        int otherCols = otherMatrix.matrix.GetLength(0);

        float[,] newMatrix = new float[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                newMatrix[i, j] = matrix.matrix[i, j] + otherMatrix.matrix[i, j];
            }
        }

        return new Matrix(newMatrix);
    }

    public static Matrix operator +(Matrix matrix, float num)
    {
        int rows = matrix.matrix.GetLength(0);
        int cols = matrix.matrix.GetLength(1);

        return matrix + new Matrix(rows, cols, num);
    }

    public static Matrix operator -(Matrix matrix, Matrix otherMatrix)
    {
        int rows = matrix.matrix.GetLength(0);
        int cols = matrix.matrix.GetLength(1);
        int otherRows = otherMatrix.matrix.GetLength(0);
        int otherCols = otherMatrix.matrix.GetLength(0);

        float[,] newMatrix = new float[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                newMatrix[i, j] = matrix.matrix[i, j] - otherMatrix.matrix[i, j];
            }
        }

        return new Matrix(newMatrix);
    }

    public static Matrix operator -(Matrix matrix, float num)
    {
        int rows = matrix.matrix.GetLength(0);
        int cols = matrix.matrix.GetLength(1);

        return matrix - new Matrix(rows, cols, num);
    }

    public static Matrix operator -(Matrix matrix)
    {
        int rows = matrix.matrix.GetLength(0);
        int cols = matrix.matrix.GetLength(1);

        float[,] newMatrix = new float[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                newMatrix[i, j] = -matrix.matrix[i, j];
            }
        }

        return new Matrix(newMatrix);
    }

    public static Matrix operator *(Matrix matrix, Matrix otherMatrix)
    {
        int rows = matrix.matrix.GetLength(0);
        int cols = matrix.matrix.GetLength(1);
        int otherRows = otherMatrix.matrix.GetLength(0);
        int otherCols = otherMatrix.matrix.GetLength(1);

        float[,] newMatrix = new float[rows, otherCols];

        for (int i1 = 0; i1 < rows; i1++)
        {
            for (int j2 = 0; j2 < otherCols; j2++)
            {
                for (int j1 = 0; j1 < cols; j1++)
                {
                    newMatrix[i1, j2] += matrix.matrix[i1, j1] * otherMatrix.matrix[j1, j2];
                }
            }
        }

        return new Matrix(newMatrix);
    }

    public static Matrix operator *(Matrix matrix, float num)
    {
        int rows = matrix.matrix.GetLength(0);
        int cols = matrix.matrix.GetLength(1);

        float[,] newMatrix = new float[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                newMatrix[i, j] = matrix.matrix[i, j] * num;
            }
        }

        return new Matrix(newMatrix);
    }

    public static Matrix operator *(float num, Matrix matrix)
    {
        return matrix * num;
    }

    public static Matrix operator /(Matrix matrix, float num)
    {
        int rows = matrix.matrix.GetLength(0);
        int cols = matrix.matrix.GetLength(1);

        float[,] newMatrix = new float[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                newMatrix[i, j] = matrix.matrix[i, j] / num;
            }
        }

        return new Matrix(newMatrix);
    }

    public static Matrix randomMatrix(int rows, int cols, float low, float high)
    {
        float[] elements = new float[rows * cols];

        for (int i = 0; i < elements.Length; i++)
        {
            elements[i] = UnityEngine.Random.Range(low, high);
        }

        return new Matrix(rows, cols, elements);
    }

    public static Matrix fromArray(float[] arr)
    {
        return new Matrix(arr.Length, 1, arr);
    }

    public static float[] toArray(Matrix matrix)
    {
        int rows = matrix.matrix.GetLength(0);
        int cols = matrix.matrix.GetLength(1);

        float[] newArray = new float[rows * cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                newArray[(i * cols) + j] = matrix.matrix[i, j];
            }
        }

        return newArray;
    }

    public static Matrix map(Matrix matrix, Func<float, float> f)
    {
        int rows = matrix.matrix.GetLength(0);
        int cols = matrix.matrix.GetLength(1);

        float[] newArray = new float[rows * cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                newArray[(i * cols) + j] = f(matrix.matrix[i, j]);
            }
        }

        return new Matrix(rows, cols, newArray);
    }

    public static Matrix crossover(Matrix matrix1, Matrix matrix2, float mutationRate, float low, float high)
    {
        int rows = matrix1.matrix.GetLength(0);
        int cols = matrix1.matrix.GetLength(1);

        Matrix newMatrix = new Matrix(rows, cols, 0);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                float rng = UnityEngine.Random.Range(0f, 1f);
                float newElement;

                if (rng < mutationRate) { newElement = UnityEngine.Random.Range(low, high); }
                else if (rng < 0.5 + (mutationRate / 2)) { newElement = matrix1.matrix[i, j]; }
                else { newElement = matrix2.matrix[i, j]; }

                newMatrix.matrix[i, j] = newElement;
            }
        }

        return newMatrix;
    }
}