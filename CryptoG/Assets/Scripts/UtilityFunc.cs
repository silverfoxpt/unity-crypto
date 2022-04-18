using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using SFB;

public struct IntVec2
{
    public int x,y;

    public IntVec2(int a, int b) {this.x = a; this.y = b;}
}

public class UtilityFunc : MonoBehaviour
{
    #region maths

    public static Vector2 nullVec = new Vector2(-100000f, -100000f);

    public static uint RotateBitLeft(uint original, int bits)
    {
        return (original << bits) | (original >> (32 - bits));
    }

    public static Vector2 RotatePoint(Vector2 point, float angle)
    {
        return Quaternion.Euler(0f, 0f, 360f - angle) * point;
    }
    
    public static Vector2 RotatePointCenter(Vector2 pointToRotate, Vector2 centerPoint, float angleInDegrees)
    {
        angleInDegrees = 360f - angleInDegrees;
        float angleInRadians = angleInDegrees * (Mathf.PI / 180f);
        float cosTheta = Mathf.Cos(angleInRadians);
        float sinTheta = Mathf.Sin(angleInRadians);

        return new Vector2
        (
            (cosTheta * (pointToRotate.x - centerPoint.x) -
                sinTheta * (pointToRotate.y - centerPoint.y) + centerPoint.x),

            (sinTheta * (pointToRotate.x - centerPoint.x) +
                cosTheta * (pointToRotate.y - centerPoint.y) + centerPoint.y)
        );
    }

    public static float VectorAngleFromY(Vector2 v)
    {
        Vector2 v2 = Vector2.up;
        return Mathf.Acos(dotProduct(v, v2) / (v.magnitude * v2.magnitude)) * Mathf.Rad2Deg;
    }

    public static float dotProduct(Vector2 a, Vector2 b)
    {
        return a.x * b.x + a.y * b.y;
    }

    public static float SqrDist(Vector2 a, Vector2 b)
    {
        return (a.x-b.x) * (a.x-b.x) + (a.y-b.y)*(a.y-b.y);
    }

    public static float Remap(float value, float from1, float to1, float from2, float to2) 
    {
        return (value - from1) * (to2 - from2) / (to1 - from1) + from2;
    }

    public static float Dist(Vector2 a, Vector2 b)
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

    #region stringRelated

    public static string BinaryToLittleEndian(string bin)
    {
        string s = ""; 
        for (int idx = 0; idx < bin.Length; idx+=8)
        {
            string a = "";
            for (int j = idx; j < idx+8; j++) {a+=bin[j];}
            s = a + s;
        }
        return s;
    }

    public static string BinaryToHex(string binary)
    {
        if (string.IsNullOrEmpty(binary))
            return binary;

        StringBuilder result = new StringBuilder(binary.Length / 8 + 1);

        int mod4Len = binary.Length % 8;
        if (mod4Len != 0)
        {
            // pad to length multiple of 8
            binary = binary.PadLeft(((binary.Length / 8) + 1) * 8, '0');
        }

        for (int i = 0; i < binary.Length; i += 8)
        {
            string eightBits = binary.Substring(i, 8);
            result.AppendFormat("{0:X2}", System.Convert.ToByte(eightBits, 2));
        }
        return result.ToString();
    }
    public static string ReverseString( string s )
    {
        char[] charArray = s.ToCharArray();
        System.Array.Reverse(charArray);
        return new string(charArray);
    }

    public static string UintToBinary(uint x, int len = 0)
    {
        string bin = "";
        while(x > 0)
        {
            bin += (char) ('0' + x%2); x /= 2;
        }
        while(bin.Length < len) {bin += '0';}
        return UtilityFunc.ReverseString(bin);
    }

    public static uint BinaryToUint(string x)
    {
        x = ReverseString(x);
        uint res = 0; int counter = 0;

        foreach(char c in x)
        {   
            if (c == '1') { res |= (1u << counter); }
            counter++;
        }
        return res;
    }

    public static string UintToHex(uint value) 
    {
        return string.Format("0x{0:X}", value);
    }
    #endregion
}
