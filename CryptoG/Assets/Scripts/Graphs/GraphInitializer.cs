using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphInitializer : MonoBehaviour
{
    [Header("Graph Settings")]
    [Range(0f, 1000f)] [SerializeField] private float extraTipLength = 0.2f;

    [Header("Markings settings")]
    [Range(0f, 15f)] [SerializeField] private float portionLength = 1f;
    [SerializeField] private float halfLengthMarking = 0.05f;

    [Header("Line Settings")]
    [SerializeField] private GameObject line;
    [SerializeField] private float lineWidth;
    [SerializeField] private float gridTransparency = 0.5f;
    [SerializeField] private float gridWidth = 0.015f;

    public static GraphInitializer instance;

    private float sideLength = -1;
    private int portionNum = -1;

    #region unityMain
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        InitializeGraph();   
    }
    #endregion

    private void InitializeGraph()
    {
        //calculate sideLength & portionNum
        Vector3 len = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, 0f));
        sideLength = Mathf.Max(Mathf.Abs(len.x), Mathf.Abs(len.y));

        portionNum = (int) (sideLength/portionLength);

        //axes
        SpawnLine(0f, 0f, 0f, sideLength + extraTipLength);
        SpawnLine(0f, 0f, 0f, -sideLength - extraTipLength);
        SpawnLine(0f, 0f, sideLength + extraTipLength, 0f);
        SpawnLine(0f, 0f, -sideLength - extraTipLength, 0f);

        //markings
        float totalLength = sideLength + extraTipLength;
        if (portionNum * portionLength > totalLength) {Debug.LogError("Error: not enough spacing"); return;}

        //x axis markings
        for (int i = -portionNum; i <= portionNum; i++)
        {
            string markText = (i == 0) ? "" : i.ToString();
            LineRenderer rend = SpawnLine(i*portionLength, halfLengthMarking, i*portionLength, -halfLengthMarking, markText, true);

            SpawnGridLine(i*portionLength, sideLength + extraTipLength, i*portionLength, -(sideLength + extraTipLength), true);
        }

        //y axis markings
        for (int i = -portionNum; i <= portionNum; i++)
        {
            string markText = (i == 0) ? "" : i.ToString();
            LineRenderer rend = SpawnLine(-halfLengthMarking, i*portionLength, halfLengthMarking, i*portionLength, markText, false);

            SpawnGridLine(-(sideLength + extraTipLength), i*portionLength, (sideLength + extraTipLength), i*portionLength, false);
        }
    }

    private LineRenderer SpawnLine(float x1, float y1, float x2, float y2, string textToFill = "", bool isX = false)
    {
        Vector3 newPos;
        if (isX) { newPos = new Vector3(x1, 0f, 0f);}
        else { newPos = new Vector3(0f, y1, 0f); }

        GameObject newLine = Instantiate(line, newPos, Quaternion.identity, this.transform);
        LineRenderer rend = newLine.GetComponent<LineRenderer>();
        rend.positionCount = 2;
        rend.startWidth = lineWidth;
        rend.endWidth = lineWidth;

        rend.SetPosition(0, new Vector2(x1, y1));
        rend.SetPosition(1, new Vector2(x2, y2));

        newLine.GetComponent<MarkerController>().SetMarkerText(textToFill, isX);
        return rend;
    }

    private LineRenderer SpawnGridLine(float x1, float y1, float x2, float y2, bool isX)
    {
        Vector3 newPos;
        if (isX) { newPos = new Vector3(x1, 0f, 0f);}
        else { newPos = new Vector3(0f, y1, 0f); }

        GameObject newLine = Instantiate(line, newPos, Quaternion.identity, this.transform);
        LineRenderer rend = newLine.GetComponent<LineRenderer>();

        rend.positionCount  = 2;
        rend.startWidth     = gridWidth;
        rend.endWidth       = gridWidth;

        rend.SetPosition(0, new Vector2(x1, y1));
        rend.SetPosition(1, new Vector2(x2, y2));
        rend.startColor     = new Color(rend.startColor.r, rend.startColor.g, rend.startColor.b, gridTransparency);
        rend.endColor       = new Color(rend.startColor.r, rend.startColor.g, rend.startColor.b, gridTransparency);

        newLine.GetComponent<MarkerController>().SetMarkerText("", isX);
        return rend;
    }

    public void ResetBaseGraph()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        InitializeGraph();
    }

    #region getters
    public float GetBaseLength() {return sideLength; }
    public float GetPortionLength() {return portionLength;}
    #endregion
}
