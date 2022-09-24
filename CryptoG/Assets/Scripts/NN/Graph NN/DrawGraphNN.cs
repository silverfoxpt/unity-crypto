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
    [SerializeField] private Vector2 shift = new Vector2(0f, 0f);

    [Header("Line options")]
    [SerializeField] private float lineWidth = 0.025f;
    [SerializeField] private Color lineColor = Color.black;

    [Header("References")]
    [SerializeField] private GameObject pointPref;
    [SerializeField] private GameObject tipPref;

    public List<GameObject> points;

    private void Awake()
    {
        transform.position += (Vector3) shift;
    }

    private void Start()
    {
        points = new List<GameObject>();
        CreateGraph();
    }

    private void CreateGraph()
    {
        DeleteAll();
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
        CreateTips();
    }

    private void CreateTips()
    {
        var xTip = Instantiate(tipPref, shift + new Vector2(xLen * spaceLen + addLen, 0f), Quaternion.identity, transform);
        var yTip = Instantiate(tipPref, shift + new Vector2(0f, yLen * spaceLen + addLen), Quaternion.identity, transform);
        yTip.transform.eulerAngles = new Vector3(0f, 0f, 90f);
    }

    private void CreateLineRend(Vector2 start, Vector2 end)
    {
        var newLine = new GameObject();

        var trans = newLine.transform;
        trans.SetParent(transform);

        var rend = newLine.AddComponent<LineRenderer>();
        rend.useWorldSpace = false;
        rend.startColor = rend.endColor = lineColor;
        rend.startWidth = rend.endWidth = lineWidth;
        rend.positionCount = 2;
        rend.SetPosition(0, start);
        rend.SetPosition(1, end);

        trans.position = new Vector3(shift.x, shift.y, 0f);

        rend.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
    }

    public void DeleteAll()
    {
        foreach(Transform child in transform)
        {
            if (child.gameObject.tag != "NNPoint") {continue;}
            Destroy(child.gameObject);
        }
    }

    public void PlotPoint(Vector2 pos, Color col, float scale = 0.1f)
    {
        var po = Instantiate(pointPref, pos, Quaternion.identity, transform);

        var con = po.GetComponent<PointControlNN>();
        con.SetLocalPos(pos);
        con.SetColor(col);
        con.SetScale(scale);

        points.Add(po);
    }
}
