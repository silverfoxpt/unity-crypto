using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HilbertDrawer : MonoBehaviour
{
    struct point
    {
        public int x,y;

        public point(int x, int y) {this.x = x; this.y =y;}

        public Vector2 ToVec2() {return new Vector2(this.x, this.y);}

        //public void Shrink(float size) {this.x /= size; this.y /= size;}
    }
    [SerializeField] private GameObject pointPref;
    [SerializeField] private GameObject linePref;
    [SerializeField] private int hilbertSideLength = 2;
    [SerializeField] private float lineWidth = 0.025f;
    [SerializeField] private float originalSideLength = 4;

    [Header("Repeat options")]
    [SerializeField] private bool repeat = true;
    [SerializeField] private float goTo = 32;
    [SerializeField] private float delay = 1f;

    [Header("Line draw options")]
    [SerializeField] private bool slowLine = true;
    [SerializeField] private float lineDelay = 0.05f;

    [Header("Other options")]
    [SerializeField] private bool drawPoints = false;
    [SerializeField] private bool paintColor = true;

    private List<point> pointsToMatch;
    private float diminishingFactors = -1;
    private float hue = 0;
    private Vector2 offset;

    private List<point> defPoints = new List<point>()
    {
        new point(0, 0),
        new point(0, 1),
        new point(1, 1),
        new point(1, 0),
    };

    public void SetHilbertSideLength(int length) {hilbertSideLength = length;}
    public void SetLineWidth(float width) {lineWidth = width;}
    public void SetOriginalSideLength(float len) {originalSideLength = len;}

    public void SetRepeatDraw(bool dr) {repeat = dr;}
    public void SetGoTo(int gotO) {goTo = gotO;}
    public void SetDelay(float del) {delay = del;}

    public void SetSlowLine(bool sl) {slowLine = sl;}
    public void SetLineDelay(float del) {lineDelay = del;}

    public void SetPoint(bool po) {drawPoints = po;}
    public void SetPaint(bool pa) {paintColor = pa;}

    void Start()
    {
        //DrawMain();
    }

    public void DrawMain()
    {
        this.StopAllCoroutines();
        if (repeat) { offset = new Vector2(-originalSideLength / 2, -originalSideLength / 2); }
        else {offset = new Vector2(-originalSideLength * hilbertSideLength / 2f, -originalSideLength * hilbertSideLength / 2f);}

        if (!repeat) { DrawHilbertNonCoroutine(hilbertSideLength); }
        else { StartCoroutine(DrawRepeat()); }
    }

    private void DrawHilbertNonCoroutine(int sid)
    {
        DeleteEverything();
        ResetVariables();
        GetAllPoints(sid);
        if (!slowLine) {DrawHilbertCurvesNonCoroutine(sid);}
        else {StartCoroutine(DrawHilbertCurves(sid));}
    }

    private void DrawHilbertCurvesNonCoroutine(int side)
    {
        int numPoints = side*side;
        for (int idx = 0; idx < numPoints-1; idx++)
        {
            Vector2 p1 = pointsToMatch[idx].ToVec2(), p2 = pointsToMatch[idx+1].ToVec2();

            //do some stuffs
            if (repeat) {p1 *= diminishingFactors; p2 *= diminishingFactors;}
            p1 *= originalSideLength; p2 *= originalSideLength;
            p1 += offset; p2 += offset;
            hue += 1f/numPoints;

            MakePoint(p1);
            MakeLine(p1, p2);
        }
        MakePoint(pointsToMatch[numPoints-1].ToVec2());
    }

    #region coroutines
    IEnumerator DrawRepeat()
    {
        for (int idx = 2; idx <= goTo; idx *= 2)
        {
            diminishingFactors = 1f / (idx-1);

            if (!slowLine) { DrawHilbertNonCoroutine(idx); }
            else {yield return StartCoroutine(DrawHilbert(idx));}
            
            yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator DrawHilbert(int sid)
    {
        DeleteEverything();
        ResetVariables();
        GetAllPoints(sid);
        if (!slowLine) {DrawHilbertCurvesNonCoroutine(sid);}
        else {yield return StartCoroutine(DrawHilbertCurves(sid));}
    }

    private void ResetVariables()
    {
        pointsToMatch = new List<point>();
        hue = 0;
    }

    private void DeleteEverything()
    {
        foreach(Transform child in transform) {Destroy(child.gameObject);}
    }

    IEnumerator DrawHilbertCurves(int side)
    {
        int numPoints = side*side;
        for (int idx = 0; idx < numPoints-1; idx++)
        {
            Vector2 p1 = pointsToMatch[idx].ToVec2(), p2 = pointsToMatch[idx+1].ToVec2();

            //do some stuffs
            if (repeat) {p1 *= diminishingFactors; p2 *= diminishingFactors;}
            p1 *= originalSideLength; p2 *= originalSideLength;
            p1 += offset; p2 += offset;
            hue += 1f/numPoints;

            MakePoint(p1);

            if (!slowLine) { MakeLine(p1, p2);}
            else {yield return SlowLine(p1, p2);}
        }
        MakePoint(pointsToMatch[numPoints-1].ToVec2());
    }


    private void MakePoint(Vector2 p)
    {
        if (!drawPoints) {return;}
        Instantiate(pointPref, p, Quaternion.identity, transform);
    }

    IEnumerator SlowLine(Vector2 p1, Vector2 p2)
    {
        MakeLine(p1, p2);
        yield return new WaitForSeconds(lineDelay);
    }
    #endregion

    private void MakeLine(Vector2 p1, Vector2 p2)
    {
        GameObject newLine = Instantiate(linePref, new Vector3(0f, 0f, 0f), Quaternion.identity, transform);

        var rend = newLine.GetComponent<LineRenderer>();
        rend.positionCount  = 2;
        rend.startWidth     = lineWidth;
        rend.endWidth       = lineWidth;
        if (paintColor)
        {
            rend.startColor     = Color.HSVToRGB(hue, 1f, 1f);
            rend.endColor       = Color.HSVToRGB(hue, 1f, 1f);
        }

        rend.SetPosition(0, p1);
        rend.SetPosition(1, p2);
    }   

    private void GetAllPoints(int side)
    {
        int numPoints = side*side;
        for (int idx = 0; idx < numPoints; idx++)
        {
            pointsToMatch.Add(FindPoint(idx, side));
        }
    }

    private point FindPoint(int pIdx, int side)
    {
        int pairs = (int) Mathf.Round(Mathf.Log(side, 2f)); //number of pairs needed in the binary string
        string bin = System.Convert.ToString(pIdx, 2);
        while(bin.Length < pairs*2) {bin = '0' + bin;}
        
        int curX = -1, curY = -1, tmp = -1; int N = 1;
        for (int idx = bin.Length - 1; idx >= 0; idx -= 2)
        {
            N *= 2; int N2 = N/2;
            int quarterIdx = (bin[idx] - '0') + (bin[idx-1] - '0')*2;

            if (idx == bin.Length-1) //first order, N = 2;
            {
                curX = defPoints[quarterIdx].x; curY = defPoints[quarterIdx].y; continue;
            }

            switch(quarterIdx)
            {
                case 0: (curX, curY) = (curY, curX); break;
                case 1: curY = curY + N2; break;
                case 2: curX = curX + N2; curY = curY + N2; break;
                case 3: tmp = curY;
                    curY = (N2-1) - curX;
                    curX = (N2-1) - tmp;
                    curX = curX + N2;
                    break;
            }
        }

        return new point(curX, curY); //placeholder
    }
}

