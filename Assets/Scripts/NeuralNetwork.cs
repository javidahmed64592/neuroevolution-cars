using UnityEngine;

public class NeuralNetwork : MonoBehaviour
{
    [HideInInspector] public int inputNodes;
    [HideInInspector] public int hiddenNodes;
    [HideInInspector] public int outputNodes;

    public Matrix weightsIH;
    public Matrix weightsHO;
    public Matrix biasH;
    public Matrix biasO;

    private Matrix newWeightsIH;
    private Matrix newWeightsHO;
    private Matrix newBiasH;
    private Matrix newBiasO;

    private void Awake()
    {
        weightsIH = Matrix.randomMatrix(hiddenNodes, inputNodes, -1f, 1f);
        weightsHO = Matrix.randomMatrix(outputNodes, hiddenNodes, -1f, 1f);
        biasH = Matrix.randomMatrix(hiddenNodes, 1, -0.4f, 0.4f);
        biasO = Matrix.randomMatrix(outputNodes, 1, -0.4f, 0.4f);
    }

    public float[] feedforward(float[] inputArray)
    {
        Matrix input = Matrix.fromArray(inputArray);

        Matrix hidden = (weightsIH * input) + biasH; // (hiddenNodes x inputNodes) * (inputNodes x 1) = (hiddenNodes x 1)
        hidden = Matrix.map(hidden, relu);

        Matrix output = (weightsHO * hidden) + biasO; // (outputNodes x hiddenNodes) * (hiddenNodes x 1) = (outputNodes x 1)
        output = Matrix.map(output, linear);

        return Matrix.toArray(output);
    }

    public void Crossover(NeuralNetwork NN, NeuralNetwork otherNN, float mutationRate)
    {
        newWeightsIH = Matrix.crossover(NN.weightsIH, otherNN.weightsIH, mutationRate, -1f, 1f);
        newWeightsHO = Matrix.crossover(NN.weightsHO, otherNN.weightsHO, mutationRate, -1f, 1f);
        newBiasH = Matrix.crossover(NN.biasH, otherNN.biasH, mutationRate, -0.4f, 0.4f);
        newBiasO = Matrix.crossover(NN.biasO, otherNN.biasO, mutationRate, -0.4f, 0.4f);
    }

    public void ApplyMatrices()
    {
        weightsIH = newWeightsIH;
        weightsHO = newWeightsHO;
        biasH = newBiasH;
        biasO = newBiasO;
    }

    // Activation functions
    private float sigmoid(float x)
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