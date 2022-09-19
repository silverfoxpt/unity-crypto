using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawGraphNN : MonoBehaviour
{
    [Header("Graph options")]
    [SerializeField] private int xLen = 10;
    [SerializeField] private int yLen = 10;
    [SerializeField] private float spaceLen = 1f;
    [SerializeField] private float addLen = 0.05f;
    [SerializeField] private float addIndicatorLen = 0.05f;

    [Header("Line options")]
    [SerializeField] private float lineWidth = 0.025f;
    [SerializeField] private Color lineColor = Color.black;

    [Header("References")]
    [SerializeField] private GameObject pointPref;

    public List<GameObject> points;

    void Start()
    {
        points = new List<GameObject>();
        CreateGraph();
        // test
        //PlotPoint(new Vector2(1f, 1f), Color.red);
    }

    private void CreateGraph()
    {
        CreateLineRend(new Vector2(0f, 0f), new Vector2(xLen * spaceLen + addLen, 0f)); //x
        CreateLineRend(new Vector2(0f, 0f), new Vector2(0f, yLen * spaceLen + addLen)); //y

        for (int i = 1; i <= xLen; i++)
        {
            CreateLineRend(new Vector2(i, addIndicatorLen), new Vector2(i, -addIndicatorLen));
        }

        for (int i = 1; i <= yLen; i++)
        {
            CreateLineRend(new Vector2(addIndicatorLen, i), new Vector2(-addIndicatorLen, i));
        }
    }

    private void CreateLineRend(Vector2 start, Vector2 end)
    {
        var newLine = new GameObject();

        var trans = newLine.transform;
        trans.SetParent(transform);
        trans.position = new Vector3(0f, 0f, 0f);

        var rend = newLine.AddComponent<LineRenderer>();
        rend.startColor = rend.endColor = lineColor;
        rend.startWidth = rend.endWidth = lineWidth;
        rend.positionCount = 2;
        rend.SetPosition(0, start);
        rend.SetPosition(1, end);

        rend.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
    }

    private void DeleteAll()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void PlotPoint(Vector2 pos, Color col, float scale = 0.1f)
    {
        var po = Instantiate(pointPref, pos, Quaternion.identity, transform);

        po.GetComponent<SpriteRenderer>().color = col;
        po.transform.localScale = new Vector3(scale, scale, 1f);

        points.Add(po);
    }
}
