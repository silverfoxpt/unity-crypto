using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphTestNN : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NNPointPlot pointPlot;
    [SerializeField] private DrawGraphNN refGraph;
    [SerializeField] private InfoController infoCon;

    [Header("Reference graph options")]
    [SerializeField] private Color inPointCol;
    [SerializeField] private Color outPointCol;
    [SerializeField] private Color wrongPointCol;

    [Header("NN Options")]
    [SerializeField] private float learnRate;

    private DataPoint[] datPoints;
    private int accuracy, total;

    IEnumerator Start()
    {
        CreateAllDataPoints();

        var neuralNet = new NeuralNetwork(2, 3, 2, 2);
        total = pointPlot.inPoints.Count + pointPlot.outPoints.Count;

        while(true)
        {
            yield return new WaitForSeconds(0.05f);

            var cost = neuralNet.CalculateAverageTotalCost(datPoints);

            PlotReferencePointAccuracy(neuralNet);
            infoCon.UpdateNetGraphInfo(accuracy, total, cost, neuralNet);

            neuralNet.Learn(datPoints, learnRate);
        }
    }

    private void PlotReferencePointAccuracy(NeuralNetwork nn)
    {
        refGraph.DeleteAll();
        ReturnData[] networkAccuracy = nn.CheckNeuralNetAccuracy(datPoints);

        int idx = 0;
        accuracy = 0;
        foreach(var po in pointPlot.inPoints)
        {
            var pos = po.GetLocalPos();
            ReturnData ret = networkAccuracy[idx]; idx++;

            if (!ret.correct)
            {
                refGraph.PlotPoint(pos, wrongPointCol);
            }
            else
            {
                accuracy += 1;
                if (ret.type == 1)
                {
                    refGraph.PlotPoint(pos, inPointCol);
                }
                else
                {
                    refGraph.PlotPoint(pos, outPointCol);
                }
            }
        }

        foreach(var po in pointPlot.outPoints)
        {
            var pos = po.GetLocalPos();
            ReturnData ret = networkAccuracy[idx]; idx++;

            if (!ret.correct)
            {
                refGraph.PlotPoint(pos, wrongPointCol);
            }
            else
            {
                accuracy += 1;
                if (ret.type == 1)
                {
                    refGraph.PlotPoint(pos, inPointCol);
                }
                else
                {
                    refGraph.PlotPoint(pos, outPointCol);
                }
            }
        }
    }

    private float PlotNumberToNewRange(float num, float oriStart, float oriEnd, float newStart, float newEnd)
    {
        float newNum = (num - oriStart) / (oriEnd - oriStart) * (newEnd - newStart);
        return newStart + newNum;
    }

    private void CreateAllDataPoints()
    {
        datPoints = new DataPoint[pointPlot.numberOfPoint];
        Vector2 pointRange = pointPlot.plotRange;

        int c = 0;
        foreach(var po in pointPlot.inPoints)
        {
            float[] inp = new float[2];
            inp[0] = po.GetLocalPos().x; inp[1] = po.GetLocalPos().y; 
            inp[0] = PlotNumberToNewRange(inp[0], 0f, pointRange.y, 0f, 1f);
            inp[1] = PlotNumberToNewRange(inp[1], 0f, pointRange.y, 0f, 1f);

            float[] outp = new float[2];
            outp[0] = 1f; outp[1] = 0f;

            DataPoint d = new DataPoint(inp, outp);
            datPoints[c] = d; c++;
        }

        foreach(var po in pointPlot.outPoints)
        {
            float[] inp = new float[2];
            inp[0] = po.GetLocalPos().x; inp[1] = po.GetLocalPos().y; 

            inp[0] = PlotNumberToNewRange(inp[0], 0f, pointRange.y, 0f, 1f);
            inp[1] = PlotNumberToNewRange(inp[1], 0f, pointRange.y, 0f, 1f);

            float[] outp = new float[2];
            outp[0] = 0f; outp[1] = 1f;

            DataPoint d = new DataPoint(inp, outp);
            datPoints[c] = d; c++;
        }
    }
}
