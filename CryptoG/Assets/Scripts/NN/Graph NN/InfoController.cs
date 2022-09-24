using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoController : MonoBehaviour
{
    private TextMeshProUGUI tex;
    private float fps = 30f;
    private bool ranned = false;

    private void Awake()
    {
        ranned = false;
        tex = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateNetGraphInfo(int accuracy, int total, float cost, NeuralNetwork nn)
    {
        string res = "";
        res += "- Number of input nodes: " + nn.numNodeOfInputLayer.ToString() + "\n";
        res += "- Number of output nodes: " + nn.numNodeOfOutputLayer.ToString() + "\n";
        res += "- Number of hidden layer nodes: " + nn.numNodeOfHiddenLayer.ToString() + "\n";
        res += "- Number of hidden layers: " + nn.numHiddenLayer.ToString() + "\n";
        res += "- Number of layers: " + (nn.numTotalLayer+1).ToString() + "\n\n";
        res += "Accuracy: " + accuracy.ToString() + "/" + total.ToString() + "\n";
        res += "Cost: " + cost.ToString() + "\n\n";

        res += "FPS: " + System.Math.Round(fps, 5).ToString() + "\n";
        
        tex.text = res;
    }

    private void Update()
    {
        if (!ranned)
        {
            ranned = true;
            fps = 1.0f / Time.deltaTime;
        }
        float newFPS = 1.0f / Time.deltaTime;
        fps = Mathf.Lerp(fps, newFPS, 0.005f);
    }
}
