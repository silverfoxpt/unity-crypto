using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPointsScreen : MoireTransparent
{
    [SerializeField] private GameObject pointPref;
    [SerializeField] private int numPoints = 2000;
    private float leftBound, rightBound, topBound, bottomBound;

    private void Start()
    {
        GetBounds();
        CreateScreen();
    }

    private void CreateScreen()
    {
        for (int i = 0; i < numPoints; i++)
        {
            CreatePoint();
        }
    }

    private void CreatePoint()
    {
        Instantiate(pointPref, GetRandPos(), Quaternion.identity, transform);
    }

    private Vector2 GetRandPos()
    {
        float x = UnityEngine.Random.Range(leftBound, rightBound);
        float y = UnityEngine.Random.Range(bottomBound, topBound);

        return new Vector2(x, y);
    }

    private void GetBounds()
    {
        Vector2 bPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
        rightBound = bPos.x; leftBound = -rightBound;
        topBound = bPos.y; bottomBound = -topBound;
    }

    public override void MakeTransparent(float trans)
    {
        foreach(Transform child in transform)
        {
            Color tmp = child.GetComponent<SpriteRenderer>().color;
            child.GetComponent<SpriteRenderer>().color = new Color(tmp.r, tmp.g, tmp.b, trans);
        }
    }
}
