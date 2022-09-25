using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerNN
{
    public float[,] weights;
    public float[] biases;
    public float[,] weightGradients;
    public float[] biasGradients;

    public int numNode, prevNumNode;

    public float[] thisLayerOutput;

    public LayerNN(int nNode, int pnNode)
    {
        numNode = nNode; prevNumNode = pnNode;

        weights = new float[prevNumNode, numNode];
        biases = new float[numNode];

        weightGradients = new float[prevNumNode, numNode];
        biasGradients = new float[numNode];

        CreateRandomWeightsAndBiases();
    }

    private void CreateRandomWeightsAndBiases()
    {
        for (int i = 0; i < prevNumNode; i++)
        {
            for (int j = 0; j < numNode; j++)
            {
                //weights[i, j] = UnityEngine.Random.Range(-1f, 1f);
                weights[i, j] = 1f;
            }
        }

        for (int i = 0; i < numNode; i++)
        {
            //biases[i] = UnityEngine.Random.Range(-1f, 1f);
            biases[i] = 1f;
        }
    }

    public void ApplyGradients(float learnRate)
    {
        for (int i = 0; i < prevNumNode; i++)
        {
            for (int j = 0; j < numNode; j++)
            {
                weights[i, j] -= weightGradients[i, j] * learnRate;
            }
        }
        for (int i = 0; i < numNode; i++)
        {
            biases[i] -= biasGradients[i] * learnRate;
        }
    }

    private float ReLU(float value) 
    {
        return Math.Max(0, value);
    }

    public static float Sigmoid(float value) 
    {
        float k = (float) Math.Exp(Convert.ToDouble(value));
        return k / (1.0f + k);
    }

    /// <summary>
    /// Calculate output for this layer of the neural network
    /// </summary>
    /// <param name="prevLayerOutput"></param>
    /// <returns></returns>
    public float[] CalculateLayer(float[] prevLayerOutput) //return list of value output for this layer
    {
        float[] curOutput = new float[numNode];

        for (int idx = 0; idx < numNode; idx++)
        {
            float val = 0f;
            for (int i = 0; i < prevNumNode; i++)
            {
                val += prevLayerOutput[i] * weights[i, idx];
            }
            val += biases[idx];
            curOutput[idx] = Sigmoid(val);
        }
        thisLayerOutput = curOutput;
        return curOutput;
    }

    /// <summary>
    /// Calculate this layer's cost, use for the LAST layer ONLY!!
    /// </summary>
    /// <param name="expectedOutput"></param>
    /// <returns></returns>
    public float CalculateLayerCost(float[] expectedOutput)
    {
        float val = 0;
        for (int i = 0; i < numNode; i++)
        {
            var delta = thisLayerOutput[i] - expectedOutput[i];
            val += (delta * delta);
        }
        return val;
    }
}
