using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphDrawer : MonoBehaviour
{
    [Header("Graph Settings")]
    [Range(0f, 100f)] [SerializeField] private float portionServing;
    [Range(0f, 1f)] [SerializeField] private float spacingBetweenMarking;

    [Header("Point Settings")]
    [SerializeField] private GameObject point;
    [Range(0f, 0.1f)] [SerializeField] private float pointScale = 1f;

    private GraphInitializer graphInitializer;
 
    void Start()
    {
        graphInitializer = GraphInitializer.instance;
        DrawGraph();   
    }

    private void DrawGraph()
    {
        //space
        float mini = -graphInitializer.GetBaseLength(); float maxi = -mini;
        while(mini <= maxi)
        {
            SpawnPoint(mini, Function(mini)); mini += spacingBetweenMarking;
        }
    }

    private void SpawnPoint(float x, float y)
    {
        GameObject newPoint = Instantiate(point, new Vector3(x, y, 0f), Quaternion.identity, transform);
        newPoint.transform.localScale = new Vector3(pointScale, pointScale, 1f);
    }

    private float Function(float x)
    {
        return Mathf.Sin(x);
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
