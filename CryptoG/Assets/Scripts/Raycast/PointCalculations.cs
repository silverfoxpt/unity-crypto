using System;
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

    public static bool onSameSide(Vector2 root, Vector2 fi, Vector2 se)
    {
        Vector2 ax = new Vector2(fi.x, fi.y), bx = new Vector2(se.x, se.y);
        ax -= root; bx -= root;

        //check
        if (Mathf.Sign(ax.x) != Mathf.Sign(bx.x) || Mathf.Sign(ax.y) != Mathf.Sign(bx.y)) {return false;}
        return true;
    }

    public static bool onLine(Vector2 first, Vector2 last, Vector2 check)
    {
        float e = 0.00001f;
        Vector2 AB = last-first, AC = check-first, BC = check - last;
        
        float sum = AC.magnitude + BC.magnitude; 
        if (Mathf.Abs(AB.magnitude - sum) <= e) {return true;} return false;
    }

    public static float crossProd(Vector2 aB, Vector2 aC)
    {
        return aB.x*aC.y - aC.x*aB.y;
    }
}
