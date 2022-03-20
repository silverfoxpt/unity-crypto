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
}
