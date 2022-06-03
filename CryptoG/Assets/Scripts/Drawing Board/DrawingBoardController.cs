using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawingBoardController : MonoBehaviour
{
    [Header("Options")]
    public Vector2Int size;
    public float multiplier = 1f;
    public Texture2D imageTex;
    public Color originalColor = Color.white;

    [Header("References")]
    public Image image;

    private void Start()
    {
        //CreateNewBoard();
    }

    public void CreateNewBoard()
    {
        RectTransform trans = image.GetComponent<RectTransform>();
        trans.sizeDelta = new Vector2(size.x, size.y) * multiplier;

        Material tmp = new Material(Shader.Find("Sprites/Default")); tmp.name = "Blank Material";
        image.material = tmp;

        imageTex = new Texture2D(size.x, size.y, TextureFormat.RGBA32, false);
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                imageTex.SetPixel(i, j, originalColor);
            }
        }
        imageTex.Apply();

        image.material.mainTexture = imageTex;
        imageTex.filterMode = FilterMode.Point;
    }

    public void SetTexture(RenderTexture tex)
    {
        RenderTexture.active = tex;

        imageTex.ReadPixels(new Rect(0, 0, size.x, size.y), 0, 0);
        imageTex.Apply();

        image.material.mainTexture = imageTex;
    }

    public void SetPixelDirect(Vector2Int pos, Color col, bool apply = false)
    {
        imageTex.SetPixel(pos.x, pos.y, col);
        if (apply) { imageTex.Apply(); }

        image.material.mainTexture = imageTex;
    }

    public Color GetPixelDirect(Vector2Int pos)
    {
        if (pos == UtilityFunc.nullVecInt) {return new Color(0f, 0f, 0f, 0f);}
        return imageTex.GetPixel(pos.x, pos.y);
    }
}
