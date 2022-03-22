using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct singleLine
{
    public Vector2 firstPoint, secPoint;
    public singleLine(Vector2 a, Vector2 b)
    {
        this.firstPoint = a; this.secPoint = b;
    }
}

public struct actualLine
{
    public float ax, bx;
    public float sampleX, sampleY;

    public actualLine(float x, float y, float a, float b)
    {
        this.ax = x; this.bx = y; this.sampleX = a; this.sampleY = b;
    }

    public Vector2 intersec(actualLine l2)
    {
        float a1 = this.ax, b1 = this.bx, a2 = l2.ax, b2 = l2.bx;
        float x = ((float) (b2-b1))/(a1-a2);
        float y = a1 * x + b1;

        //check for parallels
        if (float.IsInfinity(a1) || float.IsNegativeInfinity(a1)) //parallel to y axis
        {
            if (float.IsInfinity(a2) || float.IsNegativeInfinity(a2))  { return new Vector2(float.MaxValue, float.MinValue); }//parallel to y axis, not intersect
            else //mouse line y but seg isn't
            {
                y = this.sampleX * a2 + b2;
                x = this.sampleX;
            }
        }
        else
        {
            if (float.IsInfinity(a2) || float.IsNegativeInfinity(a2)) //seg y but mous line isn't
            {
                y = l2.sampleX * a1 + b1;
                x = l2.sampleX;
            }
            else { } //nothing happened, 2 line intersect and already calculated
        }

        return new Vector2(x, y);
    }
}

public abstract class DrawerBase : MonoBehaviour
{
    public abstract List<LineRenderer> GetAllLineRend();
    public abstract List<singleLine> GetAllLineCoordinates();
}
