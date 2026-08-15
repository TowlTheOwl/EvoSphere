using UnityEngine;

public class NeuralNetwork : MonoBehaviour
{
    public int[] layerSizes;
    public float mutationRate;
    public float mutationStrength;
    public float[][] neurons;
    public float[][][] weights;
    public float[][] biases;
    public float[] returnArray;

    public static bool ArraysEqual(float[] a, float[] b)
    {
        if (a.Length != b.Length) return false;
        for (int i = 0; i < a.Length; i++)
        {
            if (!Mathf.Approximately(a[i], b[i])) return false;
        }
        return true;
    }

    public static bool ArraysEqual(float[][] a, float[][] b)
    {
        if (a.Length != b.Length) return false;
        for (int i = 0; i < a.Length; i++)
        {
            if (!ArraysEqual(a[i], b[i])) return false; // reuse 1D comparison
        }
        return true;
    }

    public static bool ArraysEqual(float[][][] a, float[][][] b)
    {
        if (a.Length != b.Length) return false;
        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] == null && b[i] == null) continue;
            if (a[i] == null || b[i] == null) return false;
            if (!ArraysEqual(a[i], b[i])) return false; // reuse 2D comparison
        }
        return true;
    }



    public static float[] DeepCopy1D(float[] original)
    {
        float[] copy = new float[original.Length];
        for (int i = 0; i < original.Length; i++)
        {
            copy[i] = original[i];
        }
        return copy;
    }


    public static float[][] DeepCopy2D(float[][] original)
    {
        float[][] copy = new float[original.Length][];
        for (int i = 0; i < original.Length; i++)
        {
            copy[i] = DeepCopy1D(original[i]);
        }
        return copy;
    }


    public static float[][][] DeepCopy3D(float[][][] original)
    {
        float[][][] copy = new float[original.Length][][];
        for (int i = 0; i < original.Length; i++)
        {
            copy[i] = DeepCopy2D(original[i]);
        }
        return copy;
    }


    public void Initialize(int numRays) {
        layerSizes = new int[] {numRays * 2+1, 16, 8, 2};
        InitializeNetwork();
    }

    public void Inherit(NeuralNetwork parentPolicy) {
        this.layerSizes = parentPolicy.layerSizes;
        this.neurons = DeepCopy2D(parentPolicy.neurons);
        this.biases = DeepCopy2D(parentPolicy.biases);
        this.weights = DeepCopy3D(parentPolicy.weights);
        this.returnArray = new float[layerSizes[^1]];
        this.mutationRate = parentPolicy.mutationRate;
        this.mutationStrength = parentPolicy.mutationStrength;
        Mutate();
    }

    private void InitializeNetwork()
    {
        neurons = new float[layerSizes.Length][];
        biases = new float[layerSizes.Length-1][];
        weights = new float[layerSizes.Length-1][][];
        returnArray = new float[layerSizes[^1]];

        for (int i = 0; i < layerSizes.Length; i++)
        {
            neurons[i] = new float[layerSizes[i]];
        }

        for (int i = 0; i < layerSizes.Length - 1; i++)
        {
            biases[i] = new float[layerSizes[i+1]];
            int inputSize = layerSizes[i];
            int outputSize = layerSizes[i + 1];
            weights[i] = new float[outputSize][];
            for (int j = 0; j < outputSize; j++)
            {
                weights[i][j] = new float[inputSize];
                biases[i][j] = GetRandomValue()*10;
                for (int k = 0; k < inputSize; k++)
                {
                    weights[i][j][k] = GetRandomValue()*10;
                }
            }
        }
    }

    public void Forward(float[] input)
    {
        // Assign input to first neuron layer
        for (int i = 0; i < input.Length; i++)
        {
            neurons[0][i] = input[i];
        }

        // Forward pass through all layers using weights[i] to compute neurons[i+1]
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < layerSizes[i + 1]; j++)
            {
                float sum = biases[i][j];
                for (int k = 0; k < layerSizes[i]; k++)
                {
                    sum += weights[i][j][k] * neurons[i][k];
                }
                neurons[i + 1][j] = Activate(sum);
            }
        }
        
        for (int k = 0; k < returnArray.Length; k++) {
            returnArray[k] = neurons[layerSizes.Length - 1][k];
        }
    }
    public void Mutate()
    {
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < layerSizes[i+1]; j++)
            {
                // Mutate biases
                if (Random.value < mutationRate)
                {
                    biases[i][j] += GetRandomValue() * mutationStrength;
                }

                for (int k = 0; k < layerSizes[i]; k++)
                {
                    // Mutate weights
                    if (Random.value < mutationRate)
                    {
                        weights[i][j][k] += GetRandomValue() * mutationStrength;
                    }
                }
            }
        }
    }
    private float Activate(float value)
    {
        // Using sigmoid activation function
        return 1.0f / (1.0f + Mathf.Exp(-value));
    }

    private float GetRandomValue()
    {
        return Random.Range(-1.0f, 1.0f); // Random value between -1 and 1
    }
}
