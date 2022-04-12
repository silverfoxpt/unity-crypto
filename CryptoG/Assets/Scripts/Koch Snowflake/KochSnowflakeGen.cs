using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KochSnowflakeGen : MonoBehaviour
{
    struct line
    {
        public Vector2 first, second;

        public line(Vector2 a, Vector2 b) {this.first = a; this.second = b;}
    }

    [Header("References")]
    [SerializeField] private GameObject linePref;

    [Header("Main options")]
    [SerializeField] [Range(1, 10)] private int depth = 2;
    [SerializeField] [Range(0.5f, 5f)] private float centerDist = 4.5f;

    [Header("Animations")]
    [SerializeField] private bool useAnim = true;
    [SerializeField] private float delay = 1f;
    
    [Header("Line options")]
    [SerializeField] private float lineWidth = 0.03f;

    private List<line> currentLines;
    private LineRenderer rend;

    void Start()
    {
        GenerateKochSnowflake();
    }

    private void GenerateKochSnowflake()
    {
        ClearEverything();
        InitializeVars();
        CreateInitialDepth();

        if (!useAnim) { GenerateSnowflakeNonAnim(); }
        else { StartCoroutine(GenerateSnowflakeAnim()); }
    }

    private void ClearEverything()
    {
        foreach(Transform child in transform) {Destroy(child.gameObject);}
        rend = null;
    }

    IEnumerator GenerateSnowflakeAnim()
    {
        ShowLines();
        for (int dep = 2; dep <= depth; dep++)
        {
            List<line> newLines = new List<line>();
            foreach(var li in currentLines)
            {
                Vector2 fi = li.first, se = li.second; float len = (fi-se).magnitude;
                Vector2 f1 = new Vector2(fi.x * (2f/3) + se.x*(1f/3), fi.y * (2f/3) + se.y*(1f/3));
                Vector2 f2 = UtilityFunc.RotatePointCenter(fi, f1, 120f);
                Vector2 f3 = UtilityFunc.RotatePointCenter(fi, f1, 180f);

                newLines.Add(new line(fi, f1));
                newLines.Add(new line(f1, f2));
                newLines.Add(new line(f2, f3));
                newLines.Add(new line(f3, se));
            }
            currentLines = new List<line>(newLines);
            yield return new WaitForSeconds(delay);
            ShowLines();
        }
    }

    private void GenerateSnowflakeNonAnim()
    {
        for (int dep = 2; dep <= depth; dep++)
        {
            List<line> newLines = new List<line>();
            foreach(var li in currentLines)
            {
                Vector2 fi = li.first, se = li.second; float len = (fi-se).magnitude;
                Vector2 f1 = new Vector2(fi.x * (2f/3) + se.x*(1f/3), fi.y * (2f/3) + se.y*(1f/3));
                Vector2 f2 = UtilityFunc.RotatePointCenter(fi, f1, 120f);
                Vector2 f3 = UtilityFunc.RotatePointCenter(fi, f1, 180f);

                newLines.Add(new line(fi, f1));
                newLines.Add(new line(f1, f2));
                newLines.Add(new line(f2, f3));
                newLines.Add(new line(f3, se));
            }
            currentLines = new List<line>(newLines);
        }
        ShowLines();
    }

    private void InitializeVars()
    {
        GameObject re = Instantiate(linePref, new Vector3(0f, 0f, 0f), Quaternion.identity, transform);
        rend = re.GetComponent<LineRenderer>();

        rend.startWidth = lineWidth;
        rend.endWidth = lineWidth;
    }

    private void CreateInitialDepth()
    {
        currentLines = new List<line>();

        Vector2 f1 = Vector2.up * centerDist;
        Vector2 f2 = UtilityFunc.RotatePoint(f1, 120);
        Vector2 f3 = UtilityFunc.RotatePoint(f2, 120);
        currentLines.Add(new line(f1, f2));
        currentLines.Add(new line(f2, f3));
        currentLines.Add(new line(f3, f1));
    }

    private void ShowLines()
    {
        rend.positionCount = 0; int counter = 0;
        for (int idx = 0; idx < currentLines.Count; idx++)
        {
            rend.positionCount++;
            rend.SetPosition(counter, currentLines[idx].first);
            counter++;
        }
        rend.positionCount++;
        rend.SetPosition(counter, currentLines[0].first);
    }
}
