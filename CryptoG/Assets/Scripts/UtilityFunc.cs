using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using SFB;

public class UtilityFunc : MonoBehaviour
{
    #region maths

    public static Vector2 nullVec = new Vector2(-100000f, -100000f);
    public static float Remap(float value, float from1, float to1, float from2, float to2) 
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static float dist(Vector2 a, Vector2 b)
    {
        return Mathf.Sqrt((a.x-b.x) * (a.x-b.x) + (a.y-b.y)*(a.y-b.y));
    }

    public static Color GetRandColor()
    {
        return new Color(UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f),
        1f);
    }

    public static Vector2 GetRandPos(float leftBound, float rightBound, float bottomBound, float topBound)
    {
        float x = UnityEngine.Random.Range(leftBound, rightBound);
        float y = UnityEngine.Random.Range(bottomBound, topBound);
        return new Vector2(x, y);
    }

    public static Vector2 GetMidPoint(Vector2 a, Vector2 b)
    {
        return (a+b)/2f;
    }
    #endregion

    #region image/texture2d
    public static Texture2D LoadImage(string filePath) 
    {
        Texture2D tex = null;
        byte[] fileData;
    
        if (File.Exists(filePath))    
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2, TextureFormat.BGRA32, false);
            tex.LoadImage(fileData);
        }
        return tex;
    }

    public static Sprite Texture2DToSprite(Texture2D tex)
    {
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
    }

    public static string OpenImageFile()
    {
        var extensions = new [] {
            new ExtensionFilter("Image Files", "png", "jpg", "jpeg" ),
        };
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, true);

        string actualPath = "";
        foreach(var x in paths) { actualPath += x; } 

        return actualPath;
    }
    #endregion
}
