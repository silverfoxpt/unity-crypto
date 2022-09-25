using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NNPointPlot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DrawGraphNN graph;

    [Header("Options")]
    [SerializeField] public Vector2 plotRange = new Vector2(0f, 8f);
    [SerializeField] public int numberOfPoint = 100;
    [SerializeField] private Color inPointCol;
    [SerializeField] private Color outPointCol;

    public List<PointControlNN> inPoints, outPoints;

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
        inPoints = new List<PointControlNN>();
        outPoints = new List<PointControlNN>();

        foreach (var point in graph.points)
        {
            var con = point.GetComponent<PointControlNN>();

            Vector2 pos = con.GetLocalPos(); bool ok = PointCheck(pos);
            point.GetComponent<SpriteRenderer>().color = (ok) ? inPointCol : outPointCol;

            if (ok) {inPoints.Add(con);} else {outPoints.Add(con);}
        }
    }

    public bool PointCheck(Vector2 pos) //used to calculate costs, so be fking careful bruh
    {
        //float px = -0.2f * pos.x * pos.x + 6; //HARDCODE WARNING!!!
        if (pos.x <=4 && pos.y <= 4) {return true;} return false;
    }    
}
