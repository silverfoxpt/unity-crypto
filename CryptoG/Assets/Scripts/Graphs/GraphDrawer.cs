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

    private GraphInitializer graphInitializer;
    public List<GameObject> graphs = new List<GameObject>();
 
    void Start()
    {
        graphInitializer = GraphInitializer.instance;

        //test
        DrawNewGraph("x^2+1");   
        //StartCoroutine(test());
    }

    IEnumerator test()
    {
        DrawNewGraph("x^2+1");   
        yield return new WaitForSeconds(3);
        ModifyGraph(0, "x^3");
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

        graphs.Add(newLine);
    }

    private void ModifyGraph(int funcIdx, string newEquation)
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
}
