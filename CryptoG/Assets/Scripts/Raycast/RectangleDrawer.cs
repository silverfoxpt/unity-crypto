using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectangleDrawer : DrawerBase
{
    [SerializeField] private GameObject linePref;
    [SerializeField] private Vector2 topLeftCorner;
    [SerializeField] private float width;
    [SerializeField] private float height;
    private LineRenderer myRend;

    void Start()
    {
        DrawRect();   
    }

    private void DrawRect()
    {
        GameObject pol = Instantiate(linePref, new Vector3(0f, 0f, 0f), Quaternion.identity, transform);
        LineRenderer rend = pol.GetComponent<LineRenderer>(); myRend = rend;

        rend.positionCount = 5;
        rend.SetPosition(0, topLeftCorner);
        rend.SetPosition(1, topLeftCorner + new Vector2(width, 0f));
        rend.SetPosition(2, topLeftCorner + new Vector2(width, -height));
        rend.SetPosition(3, topLeftCorner + new Vector2(0f, -height));
        rend.SetPosition(4, topLeftCorner);
    }

    public Vector2 GetCenterRect()
    {
        Vector2 bottomRight = topLeftCorner + new Vector2(width, -height);
        return (topLeftCorner + bottomRight)/2f;
    }
    
    public float GetHeight() {return height;}
    public float GetWidth() {return width;}

    public override List<LineRenderer> GetAllLineRend()
    {
        return new List<LineRenderer>(){myRend};
    }
}
