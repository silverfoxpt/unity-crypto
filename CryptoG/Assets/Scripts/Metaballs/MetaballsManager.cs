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

    private float leftBound, rightBound, topBound, bottomBound;
    private List<List<DebugPointController>> pointsGame;
    private List<List<point>> points;
    private List<GameObject> lines = new List<GameObject>();
    private int rows, cols;
    private bool checker = false;

    private void Start()
    {
        checker = false;
        GetBounds();
        GenerateDebugpointsGame(); 
        CleanAndDestroy();
    }

    private void Update()
    {
        if (!checker)
        {
            checker = true;
            if (!debug)
            {
                foreach(var cir in CirclesController.circles)
                {
                    cir.TurnOffRend();
                }
            }
        }
        
        UpdatePointValue();
        DrawAllLinesSimple();
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

                    case 1 : CreateLinesSimple(pos1, pos2, pos1, pos4); break;
                    case 14 : CreateLinesSimple(pos1, pos2, pos1, pos4); break;

                    case 2 : CreateLinesSimple(pos2, pos1, pos2, pos3); break;
                    case 13 : CreateLinesSimple(pos2, pos1, pos2, pos3); break;

                    case 3: CreateLinesSimple(pos1, pos4, pos2, pos3); break;
                    case 12: CreateLinesSimple(pos1, pos4, pos2, pos3); break;

                    case 4: CreateLinesSimple(pos3, pos2, pos3, pos4); break;
                    case 11: CreateLinesSimple(pos3, pos2, pos3, pos4); break;

                    case 6: CreateLinesSimple(pos1, pos2, pos3, pos4); break;
                    case 9: CreateLinesSimple(pos1, pos2, pos3, pos4); break;

                    case 7: CreateLinesSimple(pos4, pos1, pos4, pos3); break;
                    case 8: CreateLinesSimple(pos4, pos1, pos4, pos3); break;

                    case 5: CreateLinesSimple(pos1, pos2, pos1, pos4); CreateLinesSimple(pos3, pos2, pos3, pos4); break; //1 + 4

                    case 10: CreateLinesSimple(pos2, pos1, pos2, pos3); CreateLinesSimple(pos4, pos1, pos4, pos3); break; //2 + 7
                }
            }
        }
    }

    private void CreateLinesSimple(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
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

    private void UpdatePointValue()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Vector2 pos = points[i][j].pos;
                float sum = 0f;

                foreach (var cir in CirclesController.circles)
                {
                    Vector2 center = cir.GetPos();
                    float size = cir.GetSize();

                    float val = CirFunc(pos, center, size / 2f);
                    sum += val;
                }
                points[i][j] = new point(points[i][j].pos, sum);
                if (debug) { pointsGame[i][j].SetVal(sum); }
            }
        }
    }

    private float CirFunc(Vector2 point, Vector2 center, float radius)
    {
        return Mathf.Pow(radius, 2) / (Mathf.Pow(point.x - center.x, 2) + Mathf.Pow(point.y - center.y, 2));
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
    private void CleanAndDestroy()
    {
        points = new List<List<point>>();
        for (int i = 0; i < rows; i++)
        {
            points.Add(new List<point>());
            for (int j = 0; j < cols; j++)
            {
                points[i].Add(new point(pointsGame[i][j].GetPos(), float.MaxValue));
            }
        }

        if (!debug)
        {
            pointsGame = new List<List<DebugPointController>>();
            foreach(Transform child in transform) {Destroy(child.gameObject);} //destroy all points
        }
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
        //p.GetComponent<DebugPointController>().TurnOffgraphics(); 

        return p.GetComponent<DebugPointController>();
    }
    #endregion
}

