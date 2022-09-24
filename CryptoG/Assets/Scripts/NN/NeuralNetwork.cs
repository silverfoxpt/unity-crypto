using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ReturnData
{
    public int idx;
    public bool correct;
    public int type;
}

public class NeuralNetwork 
{
    public List<LayerNN> layers;

    public int numHiddenLayer;
    public int numNodeOfHiddenLayer;
    public int numNodeOfOutputLayer;
    public int numNodeOfInputLayer;
    public int numTotalLayer;

    public NeuralNetwork(int hidden, int hiddenNodes, int outputNodes, int inputNodes)
    {
        numHiddenLayer = hidden; 
        numNodeOfHiddenLayer = hiddenNodes; numNodeOfOutputLayer = outputNodes; numNodeOfInputLayer = inputNodes;
        numTotalLayer = numHiddenLayer + 1; // end + hidden

        CreateAllLayers();
    }

    private void CreateAllLayers()
    {
        layers = new List<LayerNN>();
        for (int i = 0; i < numHiddenLayer; i++)
        {
            var newLayer = new LayerNN(
                numNodeOfHiddenLayer, 
                (i == 0) ? numNodeOfInputLayer : numNodeOfHiddenLayer
            );

            layers.Add(newLayer);
        }

        //output layer
        var lastLayer = new LayerNN(
            numNodeOfOutputLayer, 
            numNodeOfHiddenLayer
        );
        layers.Add(lastLayer);
    }

    /// <summary>
    /// Calculate cost for a single data point
    /// </summary>
    /// <param name="expectedOutput"></param>
    /// <param name="inputNN"></param>
    /// <returns></returns>
    private float CalculateCost(float[] expectedOutput, float[] inputNN)
    {
        float[] layerOutput = inputNN;
        for (int i = 0; i < numTotalLayer; i++)
        {
            layerOutput = layers[i].CalculateLayer(layerOutput);
        }
        return layers[numTotalLayer - 1].CalculateLayerCost(expectedOutput);
    }

    /// <summary>
    /// Calculate cost of all given data points, using the CalculateCost() method
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public float CalculateAverageTotalCost(DataPoint[] data)
    {
        float value = 0f;
        foreach (var dat in data)
        {
            //Debug.Log(dat.dataExpectedOutput);
            value += CalculateCost(dat.dataExpectedOutput, dat.dataInput);
        }
        return value / data.Length;
    }

    /// <summary>
    /// Help NN learn through gradient descent
    /// </summary>
    /// <param name="data"></param>
    /// <param name="learnRate"></param>
    /// <returns></returns>
    public void Learn(DataPoint[] data, float learnRate)
    {
        float h = 0.0001f;
        float originalCost = CalculateAverageTotalCost(data);

        foreach(var layer in layers)
        {
            for (int i = 0; i < layer.prevNumNode; i++)
            {
                for (int j = 0; j < layer.numNode; j++)
                {
                    layer.weights[i, j] += h;
                    float deltaCost = CalculateAverageTotalCost(data) - originalCost;
                    layer.weights[i, j] -= h;
                    layer.weightGradients[i, j] = deltaCost / h;
                }
            }

            for (int i = 0; i < layer.numNode; i++)
            {
                layer.biases[i] += h;
                float deltaCost = CalculateAverageTotalCost(data) - originalCost;
                layer.biases[i] -= h;
                layer.biasGradients[i] = deltaCost / h;
            }
        }

        foreach (var layer in layers)
        {
            layer.ApplyGradients(learnRate);
        }
    }

    /// <summary>
    /// Input all data points, check what data point is accurately predicted by the neural network. HARDCODED ALERT!
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public ReturnData[] CheckNeuralNetAccuracy(DataPoint[] data)
    {
        ReturnData[] re = new ReturnData[data.Length]; int idx = 0;
        foreach (var dat in data)
        {
            CalculateCost(dat.dataExpectedOutput, dat.dataInput);
            float[] output = layers[numTotalLayer - 1].thisLayerOutput;

            ReturnData ret = new ReturnData();
            ret.idx = idx; idx++;

            if (output[0] > output[1] && 
                dat.dataExpectedOutput[0] > dat.dataExpectedOutput[1])
            {
                ret.correct = true;
            }

            else if (output[0] <= output[1] && 
                dat.dataExpectedOutput[0] <= dat.dataExpectedOutput[1])
            {
                ret.correct = true;
            }

            else {ret.correct = false;}
            ret.type = (dat.dataExpectedOutput[0] > dat.dataExpectedOutput[1]) ? 1 : 2; //1 is safe

            re[idx-1] = ret;
        }
        
        return re;
    }
}
