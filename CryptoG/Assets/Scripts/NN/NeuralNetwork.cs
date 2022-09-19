using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetwork 
{
    public List<LayerNN> layers;
    private int numHiddenLayer;
    private int numNodeOfHiddenLayer;
    private int numTotalLayer;

    public NeuralNetwork(int hidden, int hiddenNodes)
    {
        numHiddenLayer = hidden; numNodeOfHiddenLayer = hiddenNodes;
        numTotalLayer = numHiddenLayer + 2; // start + end + hidden
        CreateAllLayers();
    }

    private void CreateAllLayers()
    {
        layers = new List<LayerNN>();
    }
}
