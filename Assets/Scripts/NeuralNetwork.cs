using UnityEngine;

public class NeuralNetwork : MonoBehaviour
{
    public int[] layerNodes;
    public Matrix[] weights;
    public Matrix[] biases;
    public Matrix[] newWeights;
    public Matrix[] newBiases;

    private void Awake()
    {
        weights = new Matrix[layerNodes.Length - 1];
        biases = new Matrix[layerNodes.Length - 1];
        newWeights = new Matrix[layerNodes.Length - 1];
        newBiases = new Matrix[layerNodes.Length - 1];

        for (int index = 0; index < layerNodes.Length - 1; index++)
        {
            weights[index] = Matrix.randomMatrix(layerNodes[index + 1], layerNodes[index], -1f, 1f);
            biases[index] = Matrix.randomMatrix(layerNodes[index + 1], 1, -0.4f, 0.4f);
        }
    }

    public float[] feedforward(float[] inputArray)
    {
        Matrix data = Matrix.fromArray(inputArray);

        for (int index = 0; index < weights.Length; index++)
        {
            data = (weights[index] * data) + biases[index];
            data = Matrix.map(data, index == weights.Length - 1 ? linear : relu);
        }

        return Matrix.toArray(data);
    }

    public void Crossover(NeuralNetwork NN, NeuralNetwork otherNN, float mutationRate)
    {
        for (int index = 0; index < weights.Length; index++)
        {
            newWeights[index] = Matrix.crossover(NN.weights[index], otherNN.weights[index], mutationRate, -1f, 1f);
            newBiases[index] = Matrix.crossover(NN.biases[index], otherNN.biases[index], mutationRate, -0.4f, 0.4f);
        }
    }

    public void ApplyMatrices()
    {
        for (int index = 0; index < weights.Length; index++)
        {
            weights[index] = newWeights[index];
            biases[index] = newBiases[index];
        }
    }

    // Activation functions
    public float sigmoid(float x)
    {
        return 1 / (1 + Mathf.Exp(-x));
    }

    private float tanh(float x)
    {
        return (Mathf.Exp(x) - Mathf.Exp(-x)) / (Mathf.Exp(x) + Mathf.Exp(-x));
    }

    private float relu(float x)
    {
        return Mathf.Max(0f, x);
    }

    private float linear(float x)
    {
        return x;
    }
}