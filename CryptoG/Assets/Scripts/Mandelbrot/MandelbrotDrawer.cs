using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MandelbrotDrawer : MonoBehaviour
{
    [SerializeField] private Vector2 rangeX = new Vector2(-2f, 2f);
    [SerializeField] private Vector2 rangeY = new Vector2(-2f, 2f);
    [SerializeField] private int maxIteration = 30;
    [SerializeField] private float infinity = 20;
    [SerializeField] private float pixelUnit = 100f;

    [SerializeField] private ScreenController screenController;
    [SerializeField] private JuliaDrawer julia;

    private Texture2D text;
    private int width, height;
    private float topBound, botBound, leftBound, rightBound;

    public Vector2 GetRangeX() {return rangeX;}
    public Vector2 GetRangeY() {return rangeY;}
    public int GetMaxIteration() {return maxIteration;}
    public float GetInf() {return infinity;}

    public void SetMaxIteration(int newMax) {maxIteration = newMax;}
    public void SetInf(float newInf) {infinity = newInf;}

    private void Start()
    {
        DrawMandelbrot();
        AddOverlay();
    }

    public void DrawMandelbrot()
    {
        text = screenController.mainTexture;
        width = text.width; height = text.height;

        for (int idx = 0; idx < height; idx++)
        {
            for (int j = 0; j < width; j++)
            {
                float x0 = UtilityFunc.Remap(j, 0, width, rangeX.x, rangeX.y);
                float y0 = UtilityFunc.Remap(idx, 0, height, rangeY.x, rangeY.y);

                //float xc = -0f;
                //float yc = -0.8f;

                float x = x0, y = y0; int maxi = 0;
                for (int k = 0; k < maxIteration; k++)
                {
                    maxi++;
                    float xx = x * x - y * y;
                    float yy = 2 * x * y;

                    x = xx + x0;
                    y = yy + y0;
                    if (x * x + y * y > infinity) { break; }
                }

                float bright = UtilityFunc.Remap(maxi, 0, maxIteration, 0.5f, 1f);
                if (maxi == maxIteration) { bright = 0f; }

                text.SetPixel(j, idx, new Color(bright, bright, bright, 1f));
            }
        }
        text.Apply();
    }

    public void DeleteOverlay()
    {
        Destroy(this.GetComponent<BoxCollider2D>());
    }

    public void AddOverlay()
    {
        BoxCollider2D box = gameObject.AddComponent<BoxCollider2D>();
        box.offset = screenController.gameObject.transform.position;
        box.size = new Vector2(screenController.GetWidth() / pixelUnit, screenController.GetHeight() / pixelUnit);

        leftBound   = box.offset.x      - box.size.x / 2f;
        rightBound  = box.offset.x      + box.size.x / 2f;

        topBound    = box.offset.y     + box.size.y / 2f;
        botBound    = box.offset.y     - box.size.y / 2f;
    }

    private void OnMouseOver() 
    {
        //Do some shit
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - screenController.gameObject.transform.position;
        mousePos.x = UtilityFunc.Remap(mousePos.x, leftBound, rightBound, rangeX.x, rangeX.y);
        mousePos.y = UtilityFunc.Remap(mousePos.y, botBound, topBound, rangeY.x, rangeY.y);

        julia.UpdateJulia(mousePos);
    }
}



