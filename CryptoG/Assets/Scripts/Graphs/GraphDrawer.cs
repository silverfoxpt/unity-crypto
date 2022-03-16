using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphDrawer : MonoBehaviour
{
    [Header("Graph Settings")]
    [Range(0f, 100f)] [SerializeField] public float portionServing;
    [Range(0f, 1f)] [SerializeField] private float spacingBetweenMarking;

    [Header("Line Settings")]
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private float lineWidth = 0.01f;
    [SerializeField] private int maxPoint = 30000;

    private GraphInitializer graphInitializer;
 
    void Start()
    {
        graphInitializer = GraphInitializer.instance;
        DrawGraph();   
    }

    private void DrawGraph()
    {
        //space
        float mini = -graphInitializer.GetSideLength() * (1/GetPortionScale()); float maxi = -mini;
        float step = (maxi-mini)/maxPoint;

        //line
        GameObject newLine = Instantiate(linePrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, transform);
        newLine.GetComponent<LineRenderer>().startWidth = lineWidth;
        newLine.GetComponent<LineRenderer>().endWidth = lineWidth;

        int idx = 0;
        while(mini <= maxi)
        {
            SpawnPoint(
                mini * GetPortionScale(), 
                FindObjectOfType<GraphCalculatorEquation>().Function(mini) * GetPortionScale(),
                newLine.GetComponent<LineRenderer>(), 
                idx
            ); 
            
            mini += step; idx++;
        }
        newLine.GetComponent<LineRenderer>().positionCount -= 2; 
        newLine.GetComponent<FunctionLineColliderController>().CreateMeshCollider();
    }

    private float GetPortionScale()
    {
        return graphInitializer.GetPortionLength() * (1/portionServing);
    }

    private void SpawnPoint(float x, float y, LineRenderer rend, int idx)
    {
        rend.positionCount = rend.positionCount+1;
        if (float.IsNaN(y)) {y = 0;}
        rend.SetPosition(idx, new Vector3(x, y, 0f));
    }

    public void ResetGraph()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        DrawGraph();
    }
}
