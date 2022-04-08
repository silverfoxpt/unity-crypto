using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiffLimitedAgg : MonoBehaviour
{
    struct intVec2
    {
        public int x, y;

        public intVec2(int a, int b) {this.x = a; this.y = b;}
    }
    [SerializeField] private GameObject pointPref;

    [Header("Point options")]
    [SerializeField] private float pointSize = 0.2f;
    [SerializeField] private Color stickPointColor = Color.red;

    [Header("Walker options")]
    [SerializeField] private int numWalker = 1000;
    [SerializeField] [Range(0f, 1f)] private float moveMagnitude = 0.1f;

    [Space(10)]
    [SerializeField] private bool spawnLeftEdge = true;
    [SerializeField] private bool spawnRightEdge = true;
    [SerializeField] private bool spawnTopEdge = true;
    [SerializeField] private bool spawnBottomEdge = true;

    [Header("Other options")]
    [SerializeField] private float delay = 0.1f;

    private List<DLAPoint> tree;
    private List<Vector2> points;
    private List<int> allowEdges;
    private Dictionary<intVec2, List<DLAPoint>> grid = new Dictionary<intVec2, List<DLAPoint>>();
    private float leftBound, rightBound, topBound, bottomBound;

    public void SetPointSize(float sz) {pointSize = sz;}
    public void SetNumWalker(int nu) {numWalker = nu;}
    public void SetMoveMag(float mag) {moveMagnitude = mag;}

    public void SetLeftSpawn(bool sp) {spawnLeftEdge = sp;}
    public void SetRightSpawn(bool sp) {spawnRightEdge = sp;}
    public void SetTopSpawn(bool sp) {spawnBottomEdge = sp;}
    public void SetBottomSpawn(bool sp) {spawnLeftEdge = sp;}

    public void SetDelay(float del) {delay = del;}

    private void Start()
    {
        //StartAggregation();
    }

    public void StartAggregation()
    {
        DeleteEverything();
        GetBounds();
        InitializeStartPos();
        CreateStartPoint();
        StartWalking();
    }

    private void CreateStartPoint()
    {
        grid = new Dictionary<intVec2, List<DLAPoint>>();
        tree = new List<DLAPoint>();

        DLAPoint root = CreatePoint(new Vector2(0f, 0f));
        tree.Add(root);

        grid.Add(gridPos(root.GetPos()), new List<DLAPoint>());
        grid[gridPos(root.GetPos())].Add(root);
    }

    private void StartWalking()
    {
        points = new List<Vector2>();

        for (int idx = 0; idx < numWalker; idx++)
        {
            points.Add(GetRandomStartPoint());
        }
        StartCoroutine(UpdatePointPosition());
    }

    IEnumerator UpdatePointPosition()
    {
        while (points.Count > 0)
        {
            List<Vector2> newPos = new List<Vector2>();
            for (int idx = 0; idx < points.Count; idx++)
            {
                Vector2 ne = points[idx] + GetRandPush();
                ne.x = Mathf.Clamp(ne.x, leftBound, rightBound);
                ne.y = Mathf.Clamp(ne.y, bottomBound, topBound);

                bool sticky = false;
                intVec2 gPos = gridPos(ne);

                //check grid
                for (int m = -1; m <= 1; m++)
                {
                    for (int n = -1; n <= 1; n++)
                    {
                        intVec2 cur = new intVec2(gPos.x + m, gPos.y + n);
                        
                        //check dict
                        if (grid.ContainsKey(cur))
                        {
                            foreach(var other in grid[cur])
                            {
                                Vector2 oth = other.GetPos();
                                if (UtilityFunc.SqrDist(oth, ne) <= pointSize * pointSize)
                                {
                                    sticky = true;
                                    var dla = CreatePoint(ne); tree.Add(dla); //add to tree

                                    //add to grid
                                    if (grid.ContainsKey(gPos))  { grid[gPos].Add(dla); }
                                    else {grid.Add(gPos, new List<DLAPoint>() {dla});}

                                    break;
                                }
                            }
                        }
                        if (sticky) {break;}
                    }
                    if (sticky) {break;}
                }

                if (!sticky) {newPos.Add(ne);}
            }
            points = new List<Vector2>(newPos);
            yield return new WaitForSeconds(delay);
        }
    }

    #region helpers
    private intVec2 gridPos(Vector2 realPos)
    {
        intVec2 res = new intVec2((int) (realPos.x / pointSize), (int) (realPos.y / pointSize)); // grid of size 2R
        return res;
    }

    private Vector2 GetRandPush()
    {
        Vector2 res = Vector2.up * moveMagnitude;

        float spin = UnityEngine.Random.Range(0f, 360f);
        res = Quaternion.Euler(0f, 0f, spin) * res;
        return res;
    }
    private void GetBounds()
    {
        Vector2 bPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
        rightBound = bPos.x; leftBound = -rightBound;
        topBound = bPos.y; bottomBound = -topBound;
    }

    private void InitializeStartPos()
    {
        allowEdges = new List<int>();

        if (spawnLeftEdge) {allowEdges.Add(0);}
        if (spawnRightEdge) {allowEdges.Add(1);}
        if (spawnTopEdge) {allowEdges.Add(2);}
        if (spawnBottomEdge) {allowEdges.Add(3);}
    }

    private void DeleteEverything()
    {
        foreach(Transform child in transform) {Destroy(child.gameObject);}
    }

    private Vector2 GetRandomStartPoint()
    {
        int randIdx = UnityEngine.Random.Range(0, allowEdges.Count);
        Vector2 res = UtilityFunc.GetRandPos(leftBound, rightBound, bottomBound, topBound);
        
        switch(randIdx)
        {
            case 0: res.x = leftBound; break;
            case 1: res.x = rightBound; break;
            case 2: res.y = topBound; break;
            case 3: res.y = bottomBound; break;
        }

        return res;
    }

    private DLAPoint CreatePoint(Vector2 pos)
    {
        GameObject newPoint = Instantiate(pointPref, pos, Quaternion.identity, transform);

        var dla = newPoint.GetComponent<DLAPoint>();
        dla.SetSize(pointSize);
        dla.SetColor(stickPointColor);

        return dla;
    }
    #endregion
}
