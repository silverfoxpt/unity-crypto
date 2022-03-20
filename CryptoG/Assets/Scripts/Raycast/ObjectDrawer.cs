using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct singleLine
{
    public Vector2 firstPoint, secPoint;
}

public class ObjectDrawer : DrawerBase
{
    [SerializeField] private GameObject linePref;
    [SerializeField] private List<singleLine> points;
    private List<LineRenderer> lineRends;

    void Awake()
    {
        DrawLines();   
    }

    private void DrawLines()
    {
        foreach(var vec in points)
        {
            GameObject line = Instantiate(linePref, new Vector3(0f, 0f, 0f), Quaternion.identity, transform);
            LineRenderer rend = line.GetComponent<LineRenderer>();
            lineRends.Add(rend);

            rend.positionCount = 2;
            rend.SetPosition(0, vec.firstPoint);
            rend.SetPosition(1, vec.secPoint);
        }
    }

    public override List<LineRenderer> GetAllLineRend()
    {
        return new List<LineRenderer>(lineRends);
    }
}
