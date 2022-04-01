using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class ASCIIProcessor : MonoBehaviour
{
    [SerializeField] private Sprite sprite;
    [SerializeField] private GameObject imgContainer;
    [SerializeField] private int neededWidth;
    [SerializeField] private int neededHeight;
    [SerializeField] private string densityString = "Ñ@#W$9876543210?!abc;:+=-,._ ";
    [SerializeField] private TextMeshProUGUI textBoard;

    private Texture2D tex, tex2;
    private Sprite newSprite;
    private int texWidth, texHeight;

    void Start()
    {
        StartCoroutine(CreateImage());
    }

    IEnumerator CreateImage()
    {
        //densityString = rev(densityString);
        GrayscaleImg(); yield return new WaitForSeconds(2f);
        ASCIIConvert();
    }

    public string rev( string s )
    {
        char[] charArray = s.ToCharArray();
        System.Array.Reverse( charArray );
        return new string( charArray );
    }

    private void ASCIIConvert()
    {
        texWidth = tex.width; texHeight = tex.height;

        int blockHeight = Mathf.FloorToInt((float)texHeight / neededHeight);
        int blockWidth = Mathf.FloorToInt((float)texWidth / neededWidth);
        int maxHeight = blockHeight * neededHeight;
        int maxWidth = blockWidth * neededWidth;
        int numBlockHeight = maxHeight/blockHeight;
        int numBlockWidth = maxWidth/blockWidth;

        /*List<List<float>> compress = new List<List<float>>();

        for (int idx = 0; idx < maxHeight; idx += blockHeight)
        {
            compress.Add(new List<float>( new float[numBlockWidth] )); //clumsy, probably
            int curBlockHeight = Mathf.FloorToInt((float) idx / blockHeight);

            for (int j = 0; j < maxWidth; j += blockWidth)
            {
                float sum = 0;
                int curBlockWidth = Mathf.FloorToInt((float) idx / blockWidth);

                //should be sum array
                for (int m = idx * blockHeight; m < idx * blockHeight + blockHeight; m++)
                {
                    for (int n = j * blockWidth; n < j * blockWidth + blockWidth; n++)
                    {
                        sum += (tex.GetPixel(m,n).r + tex.GetPixel(m,n).g + tex.GetPixel(m,n).b) / 3f;
                    }
                }

                //Debug.Log(curBlockWidth.ToString() + " " + compress[curBlockHeight].Count.ToString());
                //Debug.Log((blockWidth * blockHeight));

                compress[curBlockHeight][curBlockWidth] = sum / (blockWidth * blockHeight);
                Mathf.Clamp(compress[curBlockHeight][curBlockWidth], 0f, 1f);
            }
        }
        
        string res = "";
        foreach (var line in compress)
        {
            foreach(float sing in line)
            {
                res += FloatToASCII(sing);
            }
            res += '\n';
        }
        textBoard.text = res;*/

        tex2 = new Texture2D(numBlockWidth, numBlockHeight);
        for (int idx = 0; idx < maxHeight; idx += blockHeight)
        {
            int curBlockHeight = Mathf.FloorToInt((float) idx / blockHeight);

            for (int j = 0; j < maxWidth; j += blockWidth)
            {
                float sumr = 0, sumg = 0, sumb = 0;
                int curBlockWidth = Mathf.FloorToInt((float) idx / blockWidth);

                for (int m = idx * blockHeight; m < idx * blockHeight + blockHeight; m++)
                {
                    for (int n = j * blockWidth; n < j * blockWidth + blockWidth; n++)
                    {
                        sumr += tex.GetPixel(n,m).r;
                        sumg += tex.GetPixel(n,m).g;
                        sumb += tex.GetPixel(n,m).b;
                    }
                }

                sumr /= (blockWidth * blockHeight);
                sumg /= (blockWidth * blockHeight);
                sumb /= (blockWidth * blockHeight);

                tex2.SetPixel(curBlockWidth, curBlockHeight, new Color(sumr, sumg, sumb, 1f));
            }
        }
        tex2.Apply();

        //standard
        newSprite = Sprite.Create(tex2, new Rect(0.0f, 0.0f, tex2.width, tex2.height), new Vector2(0.5f, 0.5f), 100.0f);
        imgContainer.GetComponent<SpriteRenderer>().sprite = newSprite;
    }

    private void GrayscaleImg()
    {
        tex = sprite.texture;
        /*for (int idx = 0; idx < tex.height; idx++)
        {
            for (int j = 0; j < tex.width; j++)
            {
                Color col = tex.GetPixel(idx, j);
                float val = (col.r + col.r + col.b) / 3f;
                col.r = val;
                col.g = val;
                col.b = val;
                tex.SetPixel(idx, j, col);
            }
        }
        tex.Apply();*/

        newSprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        imgContainer.GetComponent<SpriteRenderer>().sprite = newSprite;
    }

    private char FloatToASCII(float x)
    {
        int len = densityString.Length;
        float div = 1f / len;

        return densityString[Mathf.FloorToInt(x / div)];
    }
}

