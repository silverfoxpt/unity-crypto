using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TEST4 : MonoBehaviour
{
    [SerializeField] private Image im1;
    [SerializeField] private Image im2;
    public RenderTexture tex1, tex2;
    private Texture2D imgTex1, imgTex2;

    void Start()
    {
        InitializeImages();

        for (int i = 0; i < 512; i++)
        {
            for (int j = 0; j < 512; j++)
            {
                imgTex1.SetPixel(i, j, UtilityFunc.GetRandColor());
            }
        }
        imgTex1.Apply();
        im1.material.mainTexture = imgTex1;

        Graphics.CopyTexture(imgTex1, imgTex2);
        im2.material.mainTexture = imgTex2;

        Graphics.Blit(imgTex1, tex1);
    }

    private void InitializeImages()
    {
        tex1 = new RenderTexture(512, 512, 24);
        tex1.enableRandomWrite = true;
        tex1.Create();

        tex2 = new RenderTexture(512, 512, 24);
        tex2.enableRandomWrite = true;
        tex2.Create();

        imgTex1 = new Texture2D(512, 512,
            TextureFormat.RGB24, false);

        imgTex2 = new Texture2D(512, 512,
            TextureFormat.RGB24, false);
    }
}
