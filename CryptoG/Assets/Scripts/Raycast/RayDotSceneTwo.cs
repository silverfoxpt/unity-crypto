using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayDotSceneTwo : MonoBehaviour
{
    [SerializeField] private GameObject dotPref;
    [SerializeField] private GameObject linePref;
    [SerializeField] private Color lineColor;
    [SerializeField] private GameObject bounder;
    [SerializeField] private GameObject sceneControl;
    [SerializeField] private Vector2 ownOffset = new Vector2(1f, 0f);
    private GameObject dot = null;
    private GameObject connectionLine = null;
    private LineRenderer rendLine = null;
    private Vector2 mainOffset;
    private const float e = 0.0000001f;

    private void Awake()
    {
        InstantiateScene();
    }

    private void InstantiateScene()
    {
        mainOffset = sceneControl.GetComponent<MainSceneController>().offset;
        dot = Instantiate(dotPref,
            bounder.GetComponent<RectangleDrawer>().GetCenterRect() + mainOffset + ownOffset,
            Quaternion.identity, transform);

        connectionLine = Instantiate(linePref, new Vector3(0f, 0f, 0f), Quaternion.identity, transform);
        var rend = connectionLine.GetComponent<LineRenderer>(); rendLine = rend;
        rend.startColor = lineColor;
        rend.endColor = lineColor;
        rend.positionCount = 2;

        gameObject.SetActive(false);
    }

    private void TEST()
    {
        float x1 = -1, y1 = 2;
        float x2 = 2, y2 = -4;

        float ax = ((float) (y1-y2))/(x1-x2);
        float bx = (float) y2 - ax*x2;
    }

    private void Update()
    {
        DrawMouseLine();
    }

    private void DrawMouseLine()
    {
        Vector2 p1 = dot.transform.position;
        Vector2 p2 = GetClosestPoint();
        rendLine.SetPosition(0, p1);
        rendLine.SetPosition(1, p2);
    }

    private Vector2 GetClosestPoint()
    {
        float x1m = dot.transform.position.x;
        float y1m = dot.transform.position.y;

        float x2m = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        float y2m = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;

        float ax2 = ((float) (y1m-y2m))/( (float) x1m-x2m);
        float bx2 = (float) y2m - ax2*x2m;

        actualLine sec = new actualLine(ax2, bx2, x1m, y1m);

        List<singleLine> lines = sceneControl.GetComponent<MainSceneController>().GetAllSingleLines();
        float minDist = float.MaxValue; Vector2 res = new Vector2(-1f, -1f);

        foreach(var sing in lines)
        {
            float x1 = sing.firstPoint.x + mainOffset.x, y1 = sing.firstPoint.y + mainOffset.y;
            float x2 = sing.secPoint.x + mainOffset.x, y2 = sing.secPoint.y + mainOffset.y;

            float ax = ((float) (y1-y2))/(x1-x2);
            float bx = (float) y2 - ax*x2;
           
            actualLine first = new actualLine(ax, bx, x1, y1); 
            Vector2 inter = sec.intersec(first);
            
            float di = PointCalculations.dist(inter, dot.transform.position);         
            if (di < minDist) { minDist = di; res = inter; }
        }
        return res;
    }
}
