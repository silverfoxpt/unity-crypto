using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitomezashiPattern : MonoBehaviour
{
    [SerializeField] private GameObject pointPref;
    [SerializeField] private GameObject linePref;
    [SerializeField] private float spacing  = 0.25f;
    [SerializeField] private float lineWidth = 0.01f;

    [Header("Main options")]
    [SerializeField] private bool useProbability = false;
    [SerializeField] private int num0;
    [SerializeField] private int num1;
    [SerializeField] [Range(0f, 100f)] private float pushProbability = 50f;

    private float leftBound, rightBound, topBound, bottomBound;

    void Start()
    {
        GetBounds();
        iniGrid();
    }

    private void iniGrid()
    {
        if (!useProbability)
        {
            if (num0 <= 0 && num1 <= 0) { Debug.LogError("No pattern drawable"); return;}

            int counter = 0, cur = 0;

            //left-to-right stitches
            for (float x = Mathf.Ceil(bottomBound); x <= Mathf.Floor(topBound); x += spacing)
            {
                if ((cur == 0 && num0 <= 0) || (cur == 1 && num1 <= 0)) //skip this part
                {
                    counter = 0;
                    cur = (cur == 0) ? 1 : 0;
                    continue;
                } 
                counter++;

                float start = Mathf.Ceil(leftBound), end = Mathf.Floor(rightBound);
                if (cur == 1) {start += spacing;}

                for (float y = start; y <= end; y += spacing * 2)
                {
                    //Instantiate(pointPref, new Vector2(x, y), Quaternion.identity, transform); //debug
                    Vector2 f = new Vector2(y, x), s = f + new Vector2(spacing, 0f);
                    CreateLine(f, s);
                }

                if ((cur == 0 && counter >= num0) || (cur == 1 && counter >= num1)) //skip this part
                {
                    counter = 0;
                    cur = (cur == 0) ? 1 : 0;
                }
            }

            //top down stitches
            for (float x = Mathf.Ceil(leftBound); x <= Mathf.Floor(rightBound); x += spacing)
            {
                if ((cur == 0 && num0 <= 0) || (cur == 1 && num1 <= 0)) //skip this part
                {
                    counter = 0;
                    cur = (cur == 0) ? 1 : 0;
                    continue;
                } 
                counter++;

                float start = Mathf.Ceil(bottomBound), end = Mathf.Floor(topBound);
                if (cur == 1) {start += spacing;}

                for (float y = start; y <= end; y += spacing * 2)
                {
                    //Instantiate(pointPref, new Vector2(x, y), Quaternion.identity, transform); //debug
                    Vector2 f = new Vector2(x, y), s = f + new Vector2(0f, spacing);
                    CreateLine(f, s);
                }

                if ((cur == 0 && counter >= num0) || (cur == 1 && counter >= num1)) //skip this part
                {
                    counter = 0;
                    cur = (cur == 0) ? 1 : 0;
                }
            }
        }
        else
        {
            //left-to-right stitches
            for (float x = Mathf.Ceil(bottomBound); x <= Mathf.Floor(topBound); x += spacing)
            {
                float start = Mathf.Ceil(leftBound), end = Mathf.Floor(rightBound);
                if (UnityEngine.Random.Range(0f, 100f) <= pushProbability) {start += spacing;}

                for (float y = start; y <= end; y += spacing * 2)
                {
                    //Instantiate(pointPref, new Vector2(x, y), Quaternion.identity, transform); //debug
                    Vector2 f = new Vector2(y, x), s = f + new Vector2(spacing, 0f);
                    CreateLine(f, s);
                }
            }

            //top down stitches
            for (float x = Mathf.Ceil(leftBound); x <= Mathf.Floor(rightBound); x += spacing)
            {
                float start = Mathf.Ceil(bottomBound), end = Mathf.Floor(topBound);
                if (UnityEngine.Random.Range(0f, 100f) <= pushProbability) {start += spacing;}

                for (float y = start; y <= end; y += spacing * 2)
                {
                    //Instantiate(pointPref, new Vector2(x, y), Quaternion.identity, transform); //debug
                    Vector2 f = new Vector2(x, y), s = f + new Vector2(0f, spacing);
                    CreateLine(f, s);
                }
            }
        }
    }

    private void CreateLine(Vector2 start, Vector2 end)
    {
        var newLine = Instantiate(linePref, new Vector3(0f, 0f, 0f), Quaternion.identity, transform);

        var rend = newLine.GetComponent<LineRenderer>();
        rend.positionCount = 2;
        rend.SetPosition(0, start);
        rend.SetPosition(1, end);

        rend.startWidth = lineWidth;
        rend.endWidth = lineWidth;
    }

    private void GetBounds()
    {
        Vector2 bPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
        rightBound = bPos.x; leftBound = -rightBound;
        topBound = bPos.y; bottomBound = -topBound;
    }
}
