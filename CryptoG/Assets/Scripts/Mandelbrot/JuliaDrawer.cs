using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuliaDrawer : MonoBehaviour
{
    [SerializeField] private ScreenController screenController;
    [SerializeField] private MandelbrotDrawer mandelbrotDrawer;

    private Vector2 rangeX, rangeY;
    private int maxIteration;
    private float infinity;
    private int width, height; 
    private Texture2D text;

    private float xc = 0f, yc = -0.8f;

    private void Start()
    {
        GetNewVars();
        DrawJulia();
    }

    private void DrawJulia()
    {
        text = screenController.mainTexture;
        width = text.width; height = text.height;
        
        for (int idx = 0; idx < height; idx++)
        {
            for (int j = 0; j < width; j++)
            {
                float x0 = UtilityFunc.Remap(j, 0, width, rangeX.x, rangeX.y);
                float y0 = UtilityFunc.Remap(idx, 0, height, rangeY.x, rangeY.y);

                float x = x0, y = y0; int maxi = 0;
                for (int k = 0; k < maxIteration; k++)
                {
                    maxi++;
                    float xx = x * x - y * y;
                    float yy = 2 * x * y;

                    x = xx + xc;
                    y = yy + yc;
                    if (x * x + y * y > infinity) { break; }
                }

                float bright = UtilityFunc.Remap(maxi, 0, maxIteration, 0.5f, 1f);
                if (maxi == maxIteration) { bright = 0f; }

                text.SetPixel(j, idx, new Color(bright, bright, bright, 1f));
            }
        }
        text.Apply();
    }

    public void GetNewVars()
    {
        rangeX = mandelbrotDrawer.GetRangeX();
        rangeY = mandelbrotDrawer.GetRangeY();
        maxIteration = mandelbrotDrawer.GetMaxIteration();
        infinity = mandelbrotDrawer.GetInf();
    }

    public void UpdateJulia(Vector2 newPos, bool change = true)
    {
        if (change) { xc = newPos.x; yc = newPos.y; }
        DrawJulia();
    }
}
