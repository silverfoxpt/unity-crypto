using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaosGame : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject pointPref;

    [Header("Seed points distributions")]
    [SerializeField] private int numSeeds = 3;
    [SerializeField] private bool distributeEven = true;
    [SerializeField] private float centerDist = 2f;

    [Header("Points options")]
    [SerializeField] private float pointSize = 0.2f;
    [SerializeField] private Color pointColor;

    [Header("Chaos options")]
    [SerializeField] private float delay = 0.01f;
    [SerializeField] private int pointLimit = 10000;
    [SerializeField] private int pointDropMultiplier = 15;
    [SerializeField] [Range(0f, 1f)] private float jumper = 0.5f;

    [Header("Advanced options")]
    [SerializeField] private bool avoidPreviousVertice = false;
    [SerializeField] private bool allowMidpointJumping = true;
    [SerializeField] private bool allowCenterJumping = false;

    private List<ChaosPointController> seeds;
    private List<ChaosPointController> points;
    private float leftBound, rightBound, topBound, bottomBound;

    private void Start()
    {
        GetBounds();
        DistributeSeeds();
        GenerateAllPoints();
    }

    private void GenerateAllPoints()
    {
        points = new List<ChaosPointController>();
        StartCoroutine(GeneratePoints());
    }

    IEnumerator GeneratePoints()
    {
        Vector2 start = UtilityFunc.GetRandPos(leftBound, rightBound, bottomBound, topBound);
        Vector2 cur = start;
        int prevVer = -1;

        List<Vector2> allowedPoints = new List<Vector2>();
        foreach(var p in seeds) {allowedPoints.Add(p.GetPos());}
        if (allowMidpointJumping)
        {
            for (int idx = 0; idx < numSeeds-1; idx++)
            {
                allowedPoints.Add(UtilityFunc.GetMidPoint(seeds[idx].GetPos(), seeds[idx+1].GetPos()));
            }
            allowedPoints.Add(UtilityFunc.GetMidPoint(seeds[numSeeds-1].GetPos(), seeds[0].GetPos()));
        }
        if (allowCenterJumping)
        {
            allowedPoints.Add(new Vector2(0, 0));
        }

        for (int idx = 0; idx < pointLimit; idx++)
        {
            for (int j = 0; j < pointDropMultiplier; j++)
            {
                var newPoint = CreateNewPoint(cur); points.Add(newPoint);

                int randSeed = UnityEngine.Random.Range(0, allowedPoints.Count);
                if (avoidPreviousVertice)
                {
                    while(randSeed == prevVer) { randSeed = UnityEngine.Random.Range(0, allowedPoints.Count); }
                }

                cur = Vector2.Lerp(cur, allowedPoints[randSeed], jumper);
                prevVer = randSeed;
                
            }
            yield return new WaitForSeconds(delay);
        }
    }

    private void GetBounds()
    {
        Vector2 bPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
        rightBound = bPos.x; leftBound = -rightBound;
        topBound = bPos.y; bottomBound = -topBound;
    }

    private void DistributeSeeds()
    {
        seeds = new List<ChaosPointController>();
        for (int idx = 0; idx < numSeeds; idx++)
        {
            Vector2 pos;
            if (distributeEven)
            {
                pos = Vector2.up * centerDist;
                pos = Quaternion.Euler(0f, 0f, -idx * (360f/numSeeds)) * pos;
            }
            else { pos = UtilityFunc.GetRandPos(leftBound, rightBound, bottomBound, topBound);}

            var newSeed = CreateNewPoint(pos);
            seeds.Add(newSeed);
        }
    }

    private ChaosPointController CreateNewPoint(Vector2 pos)
    {
        var newPoint = Instantiate(pointPref,
                            pos,
                            Quaternion.identity,
                            transform);

        ChaosPointController ch = newPoint.GetComponent<ChaosPointController>();
        ch.SetSize(pointSize);
        ch.SetColor(pointColor);

        return ch;
    }
}

