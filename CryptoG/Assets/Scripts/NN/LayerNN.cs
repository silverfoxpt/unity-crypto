using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerNN
{
    private float[,] weights;
    private float[] biases;

    private int numNode, prevNumNode;

    //private float[,] prevVal; //given when calculating layer
    private float[] thisLayerOutput;

    public LayerNN(int nNode, int pnNode, bool needPrevConnection = true)
    {
        numNode = nNode; prevNumNode = pnNode;
        weights = new float[prevNumNode, numNode];
        biases = new float[numNode];
        
        if (needPrevConnection)
        {
            CreateRandomWeightsAndBiases();
        }
    }

    private void CreateRandomWeightsAndBiases()
    {
        for (int i = 0; i < prevNumNode; i++)
        {
            for (int j = 0; j < numNode; j++)
            {
                weights[i, j] = UnityEngine.Random.Range(-1f, 1f);
            }
        }

        for (int i = 0; i < numNode; i++)
        {
            biases[i] = UnityEngine.Random.Range(-1f, 1f);
        }
    }

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

    public float Sigmoid(float value) 
    {
        float k = (float) Math.Exp(Convert.ToDouble(value));
        return k / (1.0f + k);
    }

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
