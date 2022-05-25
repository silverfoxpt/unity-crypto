using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct cube
{
    public Vector3 pos;
    public cube(float rand) {this.pos = new Vector3(rand, rand, rand);}
}

public class ComputeShaderTest : MonoBehaviour
{
    public ComputeShader computeShader;
    public RenderTexture renderTexture;
    public RenderTexture copyTex;

    [SerializeField] private Image image;
    [SerializeField] private Vector2Int size = new Vector2Int(512, 512);

    [Header("Another")]
    [SerializeField] private int num = 100000;
    private Texture2D tex;
    private cube[] cubes;

    void Start()
    {
        //CompShader();
        //CompCube();

        InitializeRenderTexture();

        //TouchTest();
        StartCoroutine(SlowTest());
    }

    private void InitializeRenderTexture()
    {
        if (!renderTexture)
        {
            renderTexture = new RenderTexture(size.x, size.y, 24);
            renderTexture.enableRandomWrite = true;
            renderTexture.Create();

            copyTex = new RenderTexture(size.x, size.y, 24);
            //copyTex.enableRandomWrite = true;
            copyTex.Create();
        }

        Vector2Int pos = new Vector2Int(size.x / 2, size.y / 2);
        tex = new Texture2D(size.x, size.y, TextureFormat.RGB24, false);
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                tex.SetPixel(i, j, Color.black);
            }
        }
        for (int i = pos.x-100; i < pos.x+100; i++)
        {
            for (int j = pos.y-100; j < pos.y+100; j++)
            {
                tex.SetPixel(i, j, Color.white);
            }
        }
        tex.Apply();

        RenderTexture.active = renderTexture;
        Graphics.Blit(tex, renderTexture);
    }

    #region test
    private void CompShader()
    {
        if (!renderTexture)
        {
            renderTexture = new RenderTexture(size.x, size.y, 24);
            renderTexture.enableRandomWrite = true;
            renderTexture.Create();
        }
        computeShader.SetTexture(0, "Result", renderTexture);
        computeShader.SetFloat("Resolution", renderTexture.width);
        computeShader.Dispatch(0, renderTexture.width / 8, renderTexture.height / 8, 1);

        tex = new Texture2D(size.x, size.y, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        tex.Apply();

        image.material.mainTexture = tex;
    }

    private void CompCube()
    {
        cubes = new cube[num];
        for (int i = 0; i < num; i++)
        {
            cubes[i] = new cube(1f);
        }

        //compute
        int vec3size = sizeof(float) * 3;
        ComputeBuffer cubeBuffer = new ComputeBuffer(num, vec3size); 
        cubeBuffer.SetData(cubes); //set data inside for transfering to compute shader

        computeShader.SetBuffer(0, "cubePos", cubeBuffer);
        computeShader.Dispatch(0, num/64, 1, 1);

        cubeBuffer.GetData(cubes); //get data back
        //for (int i = 0; i < num; i++) {Debug.Log(cubes[i].pos);}
        cubeBuffer.Dispose();
        
    }
    #endregion

    private void Update()
    {
        //TouchTest();
    }

    private void LateUpdate()
    {
        //UpdateTexture2D();
    }

    IEnumerator SlowTest()
    {
        UpdateTexture2D();
        while(true)
        {
            yield return new WaitForSeconds(1f);
            TouchTest();        
            UpdateTexture2D();
        }  
    }

    private void UpdateTexture2D()
    {
        RenderTexture.active = renderTexture;

        tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        tex.Apply();
        image.material.mainTexture = tex;
    }

    private void TouchTest()
    {
        Vector2Int pos = new Vector2Int(size.x/2, size.y/2);

        copyTex = new RenderTexture(size.x, size.y, 24);
        copyTex.enableRandomWrite = true;
        copyTex.Create();

        Graphics.CopyTexture(renderTexture, copyTex);
        computeShader.SetTexture(0, "Copy", copyTex);
        computeShader.SetTexture(0, "Result", renderTexture);      

        computeShader.Dispatch(0, renderTexture.width / 8, renderTexture.height / 8, 1);
    }
}
