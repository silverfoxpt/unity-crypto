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

    [Header("References")]
    [SerializeField] private GraphCalculatorEquation graphCalculator;
    [SerializeField] private GraphDisplayerUIController graphDisplayControl;

    private GraphInitializer graphInitializer;
    public List<GameObject> graphs = new List<GameObject>();
 
    void Start()
    {
        graphInitializer = GraphInitializer.instance;

        //test
        //DrawNewGraph("x^2+1");   
        test();
    }

    private void test()
    {
        DrawNewGraph("x^2+1");   
        DrawNewGraph("3.61*x-0.226*x^2+0.00605*x^3-0.0000665*x^4+(0.000000253)*x^5");
        DrawNewGraph("sin(x)");
    }
    
    private void DrawNewGraph(string equation)
    {
        //ensure new equation
        graphCalculator.SetNewEquation(equation);

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
                graphCalculator.Function(mini) * GetPortionScale(),
                newLine.GetComponent<LineRenderer>(), 
                idx
            ); 
            
            mini += step; idx++;
        }
        newLine.GetComponent<LineRenderer>().positionCount -= 2; 

        //other nodules
        newLine.GetComponent<FunctionLineColliderController>().CreateMeshCollider();
        newLine.GetComponent<GraphController>().graphEquation = equation;
        newLine.GetComponent<GraphController>().graphIdx = graphs.Count;

        graphs.Add(newLine);
        graphDisplayControl.AddNewGraphDisplay(newLine);
    }

    public void ModifyGraph(int funcIdx, string newEquation)
    {
        GameObject graphToMod = graphs[funcIdx];
        graphToMod.GetComponent<GraphController>().graphEquation = newEquation;

        //ensure new equation
        graphCalculator.SetNewEquation(newEquation);

        //space
        float mini = -graphInitializer.GetSideLength() * (1/GetPortionScale()); float maxi = -mini;
        float step = (maxi-mini)/maxPoint;

        //line
        int idx = 0;
        while(mini <= maxi)
        {
            ModifyPoint(
                mini * GetPortionScale(), 
                graphCalculator.Function(mini) * GetPortionScale(),
                graphToMod.GetComponent<LineRenderer>(), 
                idx
            ); 
            
            mini += step; idx++;
        }
        graphToMod.GetComponent<LineRenderer>().positionCount -= 2; 

        //other nodules
        graphToMod.GetComponent<FunctionLineColliderController>().RefreshMeshCollider();
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

    private void ModifyPoint(float x, float y, LineRenderer rend, int idx)
    {
        while (rend.positionCount-1 < idx) {rend.positionCount++;}
        if (float.IsNaN(y)) {y = 0;}
        rend.SetPosition(idx, new Vector3(x, y, 0f));
    }

    public void RefreshAllGraph()
    {
        for (int idx = 0; idx < graphs.Count; idx++)
        {
            ModifyGraph(idx, graphs[idx].GetComponent<GraphController>().graphEquation);//set to exact same
        }
    }

    public void DeleteSingleGraph(int idx)
    {
        if (idx >= graphs.Count) {Debug.LogError("Too much!"); return;}

        //mod graphs
        GameObject graphToBeKilled = graphs[idx];
        graphs.RemoveAt(idx);
        for (int c = idx; c < graphs.Count; c++) //modify idx of graph after
        {
            graphs[c].GetComponent<GraphController>().graphIdx--;
        }

        graphDisplayControl.DeleteExistingGraph(idx);

        //Destroy
        Destroy(graphToBeKilled);
    }
}
