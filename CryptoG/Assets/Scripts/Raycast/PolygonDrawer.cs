using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolygonDrawer : DrawerBase
{
    [SerializeField] private GameObject linePref;
    [SerializeField] private List<Vector2> points;
    private LineRenderer myRend;

    private void Awake()
    {
        DrawPolygon();
    }

    private void DrawPolygon()
    {
        GameObject pol = Instantiate(linePref, new Vector3(0f, 0f, 0f), Quaternion.identity, transform);
        LineRenderer rend = pol.GetComponent<LineRenderer>(); myRend = rend;

        int idx = 0; rend.positionCount = 0;
        foreach(var vec in points)
        {
            rend.positionCount++;
            rend.SetPosition(idx, vec);
            idx++;
        }
        rend.positionCount++;
        rend.SetPosition(rend.positionCount-1, points[0]);
    }

    public override List<LineRenderer> GetAllLineRend()
    {
        return new List<LineRenderer>() {myRend};
    }

    public override List<singleLine> GetAllLineCoordinates()
    {
        List<singleLine> res = new List<singleLine>();
        for (int idx = 0; idx < points.Count-1; idx++)
        {
            singleLine newLine = new singleLine();
            newLine.firstPoint = points[idx];
            newLine.secPoint = points[idx+1];
            res.Add(newLine);
        }
        singleLine per = new singleLine();
        per.firstPoint = points[points.Count-1];
        per.secPoint = points[0];
        res.Add(per);
        
        return res;
    }
}
