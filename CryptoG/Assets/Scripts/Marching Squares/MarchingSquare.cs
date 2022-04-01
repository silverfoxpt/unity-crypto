using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingSquare : MonoBehaviour
{
    struct point
    {
        public float x, y;

        public point(float a, float b) 
        {
            //stupid round for dictionary
            this.x = (float) Math.Round(a, 3); 
            this.y = (float) Math.Round(b, 3); 
        }

        public Vector2 ToVec2()
        {
            return new Vector2(this.x, this.y);
        }

        public point mid(point other)
        {
            return new point((this.x + other.x)/2, (this.y + other.y)/2);
        }
    }

    struct line
    {
        public point fi, se;

        public line(point a, point b) {this.fi = a; this.se = b;}
    }

    [Header("Prefabs")]
    [SerializeField] private GameObject pointPref;
    [SerializeField] private GameObject linePref;

    [Header("Config")]
    [SerializeField] private float sideSize = 0.5f;
    [SerializeField] private float lineWidth = 0.05f;

    [Header("Perlin")]
    [SerializeField] private bool perlinNoise = false;
    [SerializeField] private float inc = 0.1f;

    [Header("Debug")]
    [SerializeField] private bool debug = true;

    private float rightBound, topBound;
    private float roundLeftBound, roundRightBound, roundTopBound, roundBottomBound;

    private float startX = 0f, startY = 0f;

    private Dictionary<point, float> pointVal;

    public void SetSideSize(float newSide) {sideSize = newSide;}
    public void SetLineWidth(float newW) {lineWidth = newW;}
    public void SetPerlinToggle(bool tog) {perlinNoise = tog;}
    public void SetInc(float incr) {inc = incr;}
    public void SetDebugToggle(bool deb) {debug = deb;}

    void Start()
    {
        NewMarchingScreen();
    }

    public void NewMarchingScreen()
    {
        DestroyEverything();

        pointVal = new Dictionary<point, float>();
        FindBounds();
        DrawPoints();
        DrawLines();
    }

    private void DestroyEverything()
    {
        foreach(Transform child in transform) {Destroy(child.gameObject);}
    }

    private void DrawLines()
    {
        for (float x = roundLeftBound; x <= roundRightBound - sideSize; x+=sideSize)
        {
            for (float y = roundTopBound; y >= roundBottomBound + sideSize; y-=sideSize)
            {
                //clockwise from topLeft
                point a = new point(x, y);
                point b = new point(x+sideSize, y);
                point c = new point(x+sideSize, y-sideSize);
                point d = new point(x, y-sideSize);

                DrawSingleLine(a, b, c, d);
            }
        }
    }

    private void DrawSingleLine(point pa, point pb, point pc, point pd)
    {        
        /*//test -> ok
        rend.SetPosition(0, pa.ToVec2());
        rend.SetPosition(1, pc.ToVec2());*/

        bool isa = pointVal[pa] > 1f, isb = pointVal[pb] > 1f;
        bool isc = pointVal[pc] > 1f, isd = pointVal[pd] > 1f;
        int comp = (isa ? 1 : 0) + (isb ? 1 : 0) * 2 + (isc ? 1 : 0) * 4 + (isd ? 1 : 0) * 8;
        
        List<line> needed = new List<line>();

        //lazy ass
        switch (comp)
        {
            case 0: break;
            case 1: needed.Add(new line(pa.mid(pb), pa.mid(pd))); break;
            case 2: needed.Add(new line(pb.mid(pa), pb.mid(pc))); break;
            case 3: needed.Add(new line(pa.mid(pd), pb.mid(pc))); break;

            case 4: needed.Add(new line(pc.mid(pb), pc.mid(pd))); break;
            case 5: needed.Add(new line(pa.mid(pb), pa.mid(pd))); needed.Add(new line(pc.mid(pb), pc.mid(pd))); break;
            case 6: needed.Add(new line(pa.mid(pb), pd.mid(pc))); break;
            case 7: needed.Add(new line(pd.mid(pa), pd.mid(pc))); break;

            //8 -> 1000
            case 8: needed.Add(new line(pd.mid(pa), pd.mid(pc))); break;
            case 9: needed.Add(new line(pa.mid(pb), pd.mid(pc))); break;
            case 10: needed.Add(new line(pb.mid(pa), pb.mid(pc))); needed.Add(new line(pd.mid(pa), pd.mid(pc))); break;
            case 11: needed.Add(new line(pc.mid(pb), pc.mid(pd))); break;

            case 12: needed.Add(new line(pa.mid(pd), pb.mid(pc))); break;
            case 13: needed.Add(new line(pb.mid(pa), pb.mid(pc))); break;
            case 14: needed.Add(new line(pa.mid(pb), pa.mid(pd))); break;
            case 15: break;
        }

        foreach (line li in needed)
        {
            CreateNewLineRend(li.fi.ToVec2(), li.se.ToVec2());
        }
    }

    private void CreateNewLineRend(Vector2 p1, Vector2 p2)
    {
        GameObject newLine  = Instantiate(linePref, new Vector3(0f, 0f, 0f), Quaternion.identity, transform);
        var rend            = newLine.GetComponent<LineRenderer>();
        rend.positionCount  = 2;
        rend.startWidth     = lineWidth;
        rend.endWidth       = lineWidth;

        rend.SetPosition(0, p1);
        rend.SetPosition(1, p2);
    }

    private void DrawPoints()
    {
        for (float x = roundLeftBound; x <= roundRightBound; x+=sideSize)
        {
            startX += inc;
            for (float y = roundBottomBound; y <= roundTopBound; y+=sideSize)
            {
                startY += inc;
                pointVal[new point(x, y)] = GetAPoint();

                if (debug) //debug only
                { 
                    GameObject point = Instantiate(pointPref, new Vector3(x, y, 0f), Quaternion.identity, transform); 
                    point.GetComponent<MarchingCircleController>().SetTextVal(pointVal[new point(x, y)]);
                }                 
            }
        }
    }

    private float GetAPoint(bool incX = false)
    {
        if (!perlinNoise) {return UnityEngine.Random.Range(0f, 2f);}

        float noise = Mathf.PerlinNoise(startX, startY);
        return Mathf.Clamp(noise, 0f, 1f)*2f;
    }

    private void FindBounds()
    {
        rightBound = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f)).x;
        topBound = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f)).y;

        float c = 0f;
        while (c <= rightBound) {c += sideSize;} roundRightBound = c;
        roundLeftBound = -roundRightBound;

        c = 0f;
        while (c <= topBound) {c += sideSize;} roundTopBound = c;
        roundBottomBound = -roundTopBound;
    }
}


