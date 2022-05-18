using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetaballsManager : MonoBehaviour
{
    struct point
    {
        public Vector2 pos;
        public float val;

        public point(Vector2 p, float v) {this.pos = p; this.val = v;}
    }

    [Header("Debug")]
    [SerializeField] private GameObject pointPref;
    [SerializeField] private bool debug = true;

    [Header("Line")]
    [SerializeField] private GameObject linePref;
    [SerializeField] private float lineWidth = 0.025f;
    
    [Header("Options")]
    [SerializeField] private float spacing = 0.5f;
    [SerializeField] private float updateDelay = 0.1f;
    [SerializeField] private bool precise = false;

    private float leftBound, rightBound, topBound, bottomBound;
    private List<List<DebugPointController>> pointsGame;
    private List<List<point>> points;
    private List<GameObject> lines = new List<GameObject>();
    private int rows, cols;
    private bool checker = false;
    private int numCir;

    private void Start()
    {
        checker = false;

        GetBounds();
        if (debug) { GenerateDebugpointsGame(); }
        GeneratePointsNormal();

        if (!precise) { StartCoroutine(CalculateMetaballsSimple()); }
        else {StartCoroutine(CalculateMetaballs());}
    }

    private void Update()
    {
        if (!checker)
        {
            checker = true;
            numCir = CirclesController.circles.Count;

            if (!debug)
            {
                foreach(var cir in CirclesController.circles)
                {
                    cir.TurnOffRend();
                }
            }
        }
    }

    IEnumerator CalculateMetaballs()
    {
        while(true)
        {
            UpdatePointValue();
            DrawAllLines();
            yield return new WaitForSeconds(updateDelay);
        }
    }

    private void DrawAllLines()
    {
        foreach(var x in lines) {Destroy(x);}
        lines = new List<GameObject>();

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (i == rows-1 || j == cols-1) {break;} //nothing there

                float topLeft = points[i][j].val;           int a1 = (topLeft >= 1) ? 1 : 0;
                float topRight = points[i][j+1].val;        int a2 = (topRight >= 1) ? 1 : 0;
                float bottomRight = points[i+1][j+1].val;   int a3 = (bottomRight >= 1) ? 1 : 0;
                float bottomLeft = points[i+1][j].val;      int a4 = (bottomLeft >= 1) ? 1 : 0;

                Vector2 pos1 = points[i][j].pos;
                Vector2 pos2 = points[i][j+1].pos;
                Vector2 pos3 = points[i+1][j+1].pos; 
                Vector2 pos4 = points[i+1][j].pos;

                float val1 = topLeft, val2 = topRight, val3 = bottomRight, val4 = bottomLeft;

                int summary = a1 + a2*2 + a3*4 + a4 * 8;

                //buggy lines part
                switch(summary)
                {
                    case 0: break;
                    case 15: break;

                    case 1 : CreateLine(pos1, pos2, pos1, pos4, val1, val2, val1, val4); break;
                    case 14 : CreateLine(pos1, pos2, pos1, pos4, val1, val2, val1, val4); break;

                    case 2 : CreateLine(pos2, pos1, pos2, pos3, val2, val1, val2, val3); break;
                    case 13 : CreateLine(pos2, pos1, pos2, pos3, val2, val1, val2, val3); break;

                    case 3: CreateLine(pos1, pos4, pos2, pos3, val1, val4, val2, val3); break;
                    case 12: CreateLine(pos1, pos4, pos2, pos3, val1, val4, val2, val3); break;

                    case 4: CreateLine(pos3, pos2, pos3, pos4, val3, val2, val3, val4); break;
                    case 11: CreateLine(pos3, pos2, pos3, pos4, val3, val2, val3, val4); break;

                    case 6: CreateLine(pos1, pos2, pos3, pos4, val1, val2, val3, val4); break;
                    case 9: CreateLine(pos1, pos2, pos3, pos4, val1, val2, val3, val4); break;

                    case 7: CreateLine(pos4, pos1, pos4, pos3, val4, val1, val4, val3); break;
                    case 8: CreateLine(pos4, pos1, pos4, pos3, val4, val1, val4, val3); break;

                    case 5:     CreateLine(pos1, pos2, pos1, pos4, val1, val2, val1, val4);
                                CreateLine(pos3, pos2, pos3, pos4, val3, val2, val3, val4); break; //1 + 4

                    case 10:    CreateLine(pos2, pos1, pos2, pos3, val2, val1, val2, val3); 
                                CreateLine(pos4, pos1, pos4, pos3, val4, val1, val4, val3); break; //2 + 7
                }
            }
        }
    }

    private void CreateLine(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, float vala1, float vala2, float valb1, float valb2)
    {
        Vector2 p1 = new Vector2();
        if (Equal(a1.x, a2.x))
        {
            p1.x = a1.x; //same x;
            p1.y = a2.y + (a1.y - a2.y) * ((1 - vala2) / (vala1 - vala2));
        }
        else
        {
            p1.y = a1.y; //same y
            p1.x = a2.x + (a1.x - a2.x) * ((1 - vala2) / (vala1 - vala2));
        }

        Vector2 p2 = new Vector2();
        if (Equal(b1.x, b2.x))
        {
            p2.x = b1.x; //same x;
            p2.y = b2.y + (b1.y - b2.y) * ((1 - valb2) / (valb1 - valb2));
        }
        else
        {
            p2.y = b1.y; //same y
            p2.x = b2.x + (b1.x - b2.x) * ((1 - valb2) / (valb1 - valb2));
        }

        //create line
        var line = Instantiate(linePref, new Vector3(0f, 0f, 0f), Quaternion.identity, transform);
        var re = line.GetComponent<LineRenderer>();
        re.startWidth = lineWidth;
        re.endWidth = lineWidth;

        re.positionCount = 2;
        re.SetPosition(0, p1);
        re.SetPosition(1, p2);

        lines.Add(line); 
    }

    private bool Equal(float a, float b)
    {
        return (Mathf.Abs(a-b) <= 0.00001f);
    }

    #region simple
    IEnumerator CalculateMetaballsSimple()
    {
        while(true)
        {
            UpdatePointValue();
            DrawAllLinesSimple();
            yield return new WaitForSeconds(updateDelay);
        }
    }

    private void DrawAllLinesSimple()
    {
        foreach(var x in lines) {Destroy(x);}
        lines = new List<GameObject>();

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (i == rows-1 || j == cols-1) {break;} //nothing there

                float topLeft = points[i][j].val;           int a1 = (topLeft >= 1) ? 1 : 0;
                float topRight = points[i][j+1].val;        int a2 = (topRight >= 1) ? 1 : 0;
                float bottomRight = points[i+1][j+1].val;   int a3 = (bottomRight >= 1) ? 1 : 0;
                float bottomLeft = points[i+1][j].val;      int a4 = (bottomLeft >= 1) ? 1 : 0;

                Vector2 pos1 = points[i][j].pos;
                Vector2 pos2 = points[i][j+1].pos;
                Vector2 pos3 = points[i+1][j+1].pos; 
                Vector2 pos4 = points[i+1][j].pos;

                int summary = a1 + a2*2 + a3*4 + a4 * 8;

                //buggy lines part
                switch(summary)
                {
                    case 0: break;
                    case 15: break;

                    case 1 : CreateLineSimple(pos1, pos2, pos1, pos4); break;
                    case 14 : CreateLineSimple(pos1, pos2, pos1, pos4); break;

                    case 2 : CreateLineSimple(pos2, pos1, pos2, pos3); break;
                    case 13 : CreateLineSimple(pos2, pos1, pos2, pos3); break;

                    case 3: CreateLineSimple(pos1, pos4, pos2, pos3); break;
                    case 12: CreateLineSimple(pos1, pos4, pos2, pos3); break;

                    case 4: CreateLineSimple(pos3, pos2, pos3, pos4); break;
                    case 11: CreateLineSimple(pos3, pos2, pos3, pos4); break;

                    case 6: CreateLineSimple(pos1, pos2, pos3, pos4); break;
                    case 9: CreateLineSimple(pos1, pos2, pos3, pos4); break;

                    case 7: CreateLineSimple(pos4, pos1, pos4, pos3); break;
                    case 8: CreateLineSimple(pos4, pos1, pos4, pos3); break;

                    case 5: CreateLineSimple(pos1, pos2, pos1, pos4); CreateLineSimple(pos3, pos2, pos3, pos4); break; //1 + 4

                    case 10: CreateLineSimple(pos2, pos1, pos2, pos3); CreateLineSimple(pos4, pos1, pos4, pos3); break; //2 + 7
                }
            }
        }
    }

    private void CreateLineSimple(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
    {
        var line = Instantiate(linePref, new Vector3(0f, 0f, 0f), Quaternion.identity, transform);
        var re = line.GetComponent<LineRenderer>();
        re.startWidth = lineWidth;
        re.endWidth = lineWidth;

        re.positionCount = 2;
        re.SetPosition(0, UtilityFunc.GetMidPoint(a1, a2));
        re.SetPosition(1, UtilityFunc.GetMidPoint(b1, b2));

        lines.Add(line); 
    }
    #endregion

    private void UpdatePointValue()
    {
        List<Vector2> posi = new List<Vector2>();
        List<float> size = new List<float>();

        foreach (var cir in CirclesController.circles)
        {
            Vector2 center = cir.GetPos();
            float sz = cir.GetSize();

            posi.Add(center); size.Add(sz);
        }

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Vector2 pos = points[i][j].pos;
                float sum = 0f;

                for (int k = 0; k < numCir; k++)
                {
                    float val = CirFunc(pos, posi[k], size[k] / 2f);
                    sum += val;
                }
                points[i][j] = new point(points[i][j].pos, sum);
                if (debug) { pointsGame[i][j].SetVal(sum); }
            }
        }
    }

    private float CirFunc(Vector2 point, Vector2 center, float radius)
    {
        return radius * radius / ((point.x - center.x) * (point.x - center.x) + (point.y - center.y) * (point.y - center.y));
    }

    #region helper
    private void GetBounds()
    {
        Vector2 bPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
        rightBound = bPos.x; leftBound = -rightBound;
        topBound = bPos.y; bottomBound = -topBound;

        leftBound = ((int) (leftBound / spacing) * spacing);
        rightBound = ((int) (rightBound / spacing) * spacing);
        topBound = ((int) (topBound / spacing) * spacing);
        bottomBound = ((int) (bottomBound / spacing) * spacing);
    }
    #endregion

    #region debug
    private void GeneratePointsNormal()
    {
        points = new List<List<point>>(); int counter = 0;

        for (float y = topBound; y >= bottomBound; y-=spacing)
        {
            points.Add(new List<point>());
            for (float x = leftBound; x <= rightBound; x+=spacing)
            {
                points[counter].Add(new point(new Vector2(x, y), float.MaxValue));
            }
            counter++;
        }

        rows = points.Count;
        cols = points[0].Count;
    }

    private void GenerateDebugpointsGame()
    {
        pointsGame = new List<List<DebugPointController>>(); int counter = 0;

        for (float y = topBound; y >= bottomBound; y-=spacing)
        {
            pointsGame.Add(new List<DebugPointController>());
            for (float x = leftBound; x <= rightBound; x+=spacing)
            {
                var p = CreatePoint(new Vector2(x, y));
                pointsGame[counter].Add(p);
            }
            counter++;
        }

        rows = pointsGame.Count;
        cols = pointsGame[0].Count;
    }

    private DebugPointController CreatePoint(Vector2 pos)
    {
        var p = Instantiate(pointPref, pos, Quaternion.identity, transform);

        return p.GetComponent<DebugPointController>();
    }
    #endregion
}

