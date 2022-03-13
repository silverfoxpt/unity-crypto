using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphInitializer : MonoBehaviour
{
    [Header("Graph Settings")]
    [Range(0f, 1f)] [SerializeField] private float extraTipLength = 0.2f;

    [Header("Markings settings")]
    [Range(0f, 15f)] [SerializeField] private float portionLength = 1f;
    [SerializeField] private float halfLengthMarking = 0.05f;

    [Header("Line Settings")]
    [SerializeField] private GameObject line;
    [SerializeField] private float lineWidth;

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
            LineRenderer rend = SpawnLine(i*portionLength, halfLengthMarking, i*portionLength, -halfLengthMarking);
        }

        //y axis markings
        for (int i = -portionNum; i <= portionNum; i++)
        {
            LineRenderer rend = SpawnLine(-halfLengthMarking, i*portionLength, halfLengthMarking, i*portionLength);
        }
    }

    private LineRenderer SpawnLine(float x1, float y1, float x2, float y2)
    {
        GameObject newLine = Instantiate(line, new Vector3(0f, 0f, 0f), Quaternion.identity, this.transform);
        LineRenderer rend = newLine.GetComponent<LineRenderer>();
        rend.positionCount = 2;
        rend.startWidth = lineWidth;
        rend.endWidth = lineWidth;

        rend.SetPosition(0, new Vector2(x1, y1));
        rend.SetPosition(1, new Vector2(x2, y2));
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
