using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrownianSnowflakeGen : MonoBehaviour
{
    
    [SerializeField] private GameObject pointPref;

    [Header("General options")]
    [SerializeField] private int numWings = 6;
    [SerializeField] private int numWalkerInd = 500;
    [SerializeField] private float centerDist = 4.8f;
    [SerializeField] [Range(0f, 1f)] private float moveMagnitude = 0.1f;

    [Header("Point options")]
    [SerializeField] private Color pointColor = Color.black;
    [SerializeField] private float pointSize = 0.1f;

    [Header("Other options")]
    [SerializeField] private float delay = 0.01f;

    private List<DLAPoint> tree;
    private List<Vector2> points;
    private Dictionary<IntVec2, List<Vector2> > grid;

    private void Start()
    {
        CreateRootPoint();
        CreateRandomStart();
        RandomWalkLimited();
    }

    private void CreateRandomStart()
    {
        points = new List<Vector2>();

        //original position
        float halfSpace = 360f / numWings / 4f;
        Vector2 pos = UtilityFunc.RotatePoint(Vector2.up*centerDist, halfSpace); 

        for (int idx = 0; idx < numWalkerInd; idx++)
        {
            points.Add(pos);
        }
    }

    private void RandomWalkLimited()
    {
        StartCoroutine(RandomWalk());
    }

    IEnumerator RandomWalk()
    {
        float halfSpace = 360f / numWings / 2f;
        Vector2 limiterRay = UtilityFunc.RotatePoint(Vector2.up*centerDist, halfSpace); 
        float slope = limiterRay.y / limiterRay.x;

        while(points.Count > 0)
        {
            List<Vector2> newPoints = new List<Vector2>();
            foreach (Vector2 pos in points)
            {
                Vector2 ne = pos + GetRandVectorDown(); 
                float xLim = ne.y / slope; // b is 0 as limiterRay pass through (0, 0)
                ne.x = Mathf.Clamp(ne.x, 0f, xLim); 
                ne.y = Mathf.Clamp(ne.y, 0f, centerDist);

                bool sticky = false; 
                IntVec2 gPos = GetGridPosition(ne);

                for (int m = -1; m <= 1; m++)
                {
                    for (int n = -1; n <= 1; n++)
                    {
                        IntVec2 cur = new IntVec2(gPos.x + m, gPos.y + n);
                        
                        //check dict
                        if (grid.ContainsKey(cur))
                        {
                            foreach(var other in grid[cur])
                            {
                                if (UtilityFunc.SqrDist(other, ne) <= pointSize * pointSize)
                                {
                                    sticky = true;
                                    var dla = CreatePoint(ne); tree.Add(dla); //add to tree

                                    //add to grid
                                    if (grid.ContainsKey(gPos))  { grid[gPos].Add(dla.GetPos()); }
                                    else {grid.Add(gPos, new List<Vector2>() {dla.GetPos()});}

                                    LoopPointSnowFlake(ne);

                                    break;
                                }
                            }
                        }
                        if (sticky) {break;}
                    }
                    if (sticky) {break;}
                }
                if (!sticky) {newPoints.Add(ne);}
                
            }
            points = new List<Vector2>(newPoints);
            yield return new WaitForSeconds(delay); //placeholder, ensure infinite loop closeable
        }
    }

    private void LoopPointSnowFlake(Vector2 ne)
    {
        float halfSpace = 360f / numWings / 2f;
        float fullSpace = 360f / numWings;

        float angle = UtilityFunc.VectorAngleFromY(ne);
        Vector2 ne2 = UtilityFunc.RotatePoint(Vector2.up*ne.magnitude, 360f - angle);
        CreatePoint(ne2);

        //copy to other wings
        for (int idx = 1; idx < numWings; idx++)
        {
            Vector2 otherNe = UtilityFunc.RotatePoint(ne, fullSpace * idx);
            Vector2 otherNe2 = UtilityFunc.RotatePoint(ne2, fullSpace * idx);

            CreatePoint(otherNe); CreatePoint(otherNe2);
        }
    }

    private void CreateRootPoint()
    {
        tree = new List<DLAPoint>();
        grid = new Dictionary<IntVec2, List<Vector2>>();

        var dlaRoot = CreatePoint(new Vector2(0f, 0f));
        tree.Add(dlaRoot);
        grid.Add(GetGridPosition(dlaRoot.GetPos()), new List<Vector2>() {dlaRoot.GetPos()});
    }

    #region helpers
    private Vector2 GetRandVectorDown()
    {
        Vector2 res = Vector2.up * moveMagnitude;
        float angle = UnityEngine.Random.Range(0f, 360f); //Debug.Log(angle);
        res = UtilityFunc.RotatePoint(res, angle); //only point down -> no wandering
        return res;
    }

    private IntVec2 GetGridPosition(Vector2 pos)
    {
        return new IntVec2((int) (pos.x / pointSize), (int) (pos.y / pointSize));
    }

    private DLAPoint CreatePoint(Vector2 pos)
    {
        var newPoint = Instantiate(pointPref, pos, Quaternion.identity, transform);

        var dla = newPoint.GetComponent<DLAPoint>();
        dla.SetColor(pointColor);
        dla.SetSize(pointSize);

        return dla;
    }
    #endregion
}


