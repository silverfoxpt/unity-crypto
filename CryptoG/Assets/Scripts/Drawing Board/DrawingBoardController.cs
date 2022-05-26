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

    private Texture2D imageTex;

    [Header("References")]
    public Image image;

    private void Start()
    {
        SetImageSize();
    }

    private void SetImageSize()
    {
        RectTransform trans = image.GetComponent<RectTransform>();
        trans.sizeDelta = new Vector2(size.x, size.y) * multiplier;

        imageTex = new Texture2D(size.x, size.y, TextureFormat.RGBA32, false);
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                imageTex.SetPixel(i, j, Color.white);
            }
        }
        imageTex.Apply();

        image.material.mainTexture = imageTex;
    }

    public void SetTexture(RenderTexture tex)
    {
        RenderTexture.active = tex;

        imageTex.ReadPixels(new Rect(0, 0, size.x, size.y), 0, 0);
        imageTex.Apply();

        image.material.mainTexture = imageTex;
    }

    public void SetPixelDirect(Vector2Int pos)
    {
        imageTex.SetPixel(pos.x, pos.y, Color.black);
        imageTex.Apply();

        image.material.mainTexture = imageTex;
    }
}
