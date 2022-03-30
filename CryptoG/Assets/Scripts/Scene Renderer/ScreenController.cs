using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenController : MonoBehaviour
{
    [Header("Deprecated")]
    [SerializeField] private GameObject pixelPref;
    [SerializeField] private float pixelSize;
    [SerializeField] private bool centralOverride;
    private List<List<PixelController>> pixels = new List<List<PixelController>>();
    
    [Header("New")]
    [SerializeField] private int width;
    [SerializeField] private int height;

    private Sprite mySprite;    
    private Texture2D mainTexture;

    private void Awake() 
    {
        CreateNewScreen();
    }

    private void CreateNewScreen()
    {
        var sr = gameObject.AddComponent<SpriteRenderer>() as SpriteRenderer;
        mainTexture = new Texture2D(width, height);

        RefreshScreen();
    }

    public void RefreshScreen()
    {
        for (int y = 0; y < mainTexture.height; y++)
        {
            for (int x = 0; x < mainTexture.width; x++)
            {
                //Color color = ((x & y) != 0 ? Color.white : Color.gray); //test
                Color color = Color.white;
                mainTexture.SetPixel(x, y, color);
            }
        }
        mainTexture.Apply();

        mySprite = Sprite.Create(mainTexture, new Rect(0.0f, 0.0f, mainTexture.width, mainTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
        GetComponent<SpriteRenderer>().sprite = mySprite;
    }

    public void SetPixelScreen(int x, int y, Color col, bool apply = false)
    {
        mainTexture.SetPixel(x, y, col);
        if (apply) {mainTexture.Apply();}
    }

    public void SetFullPixelScreen(Color[] cols)
    {
        mainTexture.SetPixels(cols);
    }

    public void ScreenApply()
    {
        mainTexture.Apply();
    }

    private void CreateScreen()
    {
        float startY = (height/2f)*pixelSize;
        float startX = (-width/2f)*pixelSize;
        Vector2 off = new Vector2(startX, startY);

        for (int idx = 0; idx < height; idx++)
        {
            pixels.Add(new List<PixelController>());
            for (int j = 0; j < width; j++)
            {
                GameObject newPix = Instantiate(pixelPref, new Vector3(0f, 0f, 0f), Quaternion.identity, transform);
                PixelController pixControl = newPix.GetComponent<PixelController>();

                pixControl.ChangeSize(pixelSize);
                pixControl.SetTopLeftPosition(new Vector3(j * pixelSize, -idx * pixelSize, 0f) + (Vector3) off);
                pixControl.ChangeColor(new Color(UnityEngine.Random.Range(0f, 1f), 
                    UnityEngine.Random.Range(0f, 1f), 
                    UnityEngine.Random.Range(0f, 1f), 
                    1f));
            }
        }
    }

    public int GetWidth() {return width;}
    public int GetHeight() {return height;}
}

