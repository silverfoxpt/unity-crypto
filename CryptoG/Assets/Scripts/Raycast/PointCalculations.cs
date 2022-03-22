using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCalculations : MonoBehaviour
{
    public static float dist(Vector2 first, Vector2 sec)
    {
        float x1 = first.x, y1 = first.y, x2 = sec.x, y2 = sec.y;
        return Mathf.Sqrt((x2-x1)*(x2-x1) + (y2-y1)*(y2-y1));
    }
}
