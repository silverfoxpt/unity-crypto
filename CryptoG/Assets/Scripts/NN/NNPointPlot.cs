using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NNPointPlot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DrawGraphNN graph;

    [Header("Options")]
    [SerializeField] private Vector2 plotRange = new Vector2(0f, 8f);
    [SerializeField] private int numberOfPoint = 100;
    [SerializeField] private Color inPointCol;
    [SerializeField] private Color outPointCol;

    private List<GameObject> inPoints, outPoints;

    private void Start()
    {
        PlotRand();
        ColorCodePoint();
    }

    private void PlotRand()
    {
        for (int i = 0; i < numberOfPoint; i++)
        {
            Vector2 pos = new Vector2(UnityEngine.Random.Range(plotRange.x, plotRange.y), 
                UnityEngine.Random.Range(plotRange.x, plotRange.y));
            graph.PlotPoint(pos, Color.red);
        }
    }

    private void ColorCodePoint()
    {
        inPoints = new List<GameObject>();
        outPoints = new List<GameObject>();

        foreach (var point in graph.points)
        {
            Vector2 pos = point.transform.position; bool ok = PointCheck(pos);
            point.GetComponent<SpriteRenderer>().color = (ok) ? inPointCol : outPointCol;
        }
    }

    public bool PointCheck(Vector2 pos) //used to calculate costs, so be fking careful bruh
    {
        float px = -0.2f * pos.x * pos.x + 6; //HARDCODE WARNING!!!
        if (pos.y <= px) {return true;} return false;
    }    
}
