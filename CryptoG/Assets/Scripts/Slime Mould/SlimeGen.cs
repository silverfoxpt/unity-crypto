using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlimeGen : MonoBehaviour
{
    struct agent
    {
        public Vector2 pos;
        public float angle;
    }

    [Header("Options")]
    [SerializeField] private Vector2Int size = new Vector2Int(1024, 512);
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private int numAgents = 128;
    [SerializeField] private float diffuseSpeed = 1;
    [SerializeField] private float evaporateSpeed = 1;

    [Header("Sensing")]
    [SerializeField] private float sensorAngle = 0f;
    [SerializeField] private float sensorOffsetDist = 0f;
    [SerializeField] private int sensorSize = 5;
    [SerializeField] private float turnSpeed = 1f;

    [Header("References")]
    [SerializeField] private Image img;
    [SerializeField] private ComputeShader computeShader;
    [SerializeField] private ComputeShader diffuse;

    private RenderTexture tex, copyTex;
    private Texture2D texImg;
    private agent[] dat;
    private float counter;

    private void Start()
    {
        counter = 0;

        tex = new RenderTexture(size.x, size.y, 24);
        tex.enableRandomWrite = true;
        tex.Create();

        copyTex = new RenderTexture(size.x, size.y, 24);
        copyTex.enableRandomWrite = true;
        copyTex.Create();

        texImg = new Texture2D(size.x, size.y, TextureFormat.RGBA32, false);

        InitializeAllAgents();
    }

    private void InitializeAllAgents()
    {
        dat = new agent[numAgents];
        Vector2 center =  new Vector2(size.x / 2, size.y / 2);
        for (int i = 0; i < numAgents; i++)
        {
            var x = new agent();

            //x.pos = center + UnityEngine.Random.insideUnitCircle * size.y * 0.5f;
            //x.angle = Mathf.Atan2((center - x.pos).normalized.y, (center - x.pos).normalized.x);

            x.pos = center;
            x.angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            dat[i] = x;
        }
    }

    private void Update()
    {
        UpdateAllAgents();
        DiffuseMap();
    }

    private void DiffuseMap()
    {
        int idx = diffuse.FindKernel("diff");
        diffuse.SetTexture(idx, "old", tex);
        diffuse.SetTexture(idx, "newMap", copyTex);    

        diffuse.SetInt("width", size.x);
        diffuse.SetInt("height", size.y);

        diffuse.SetFloat("diffuseSpeed", diffuseSpeed);
        diffuse.SetFloat("evaSpeed", evaporateSpeed);
        diffuse.SetFloat("deltaTime", Time.deltaTime);

        diffuse.Dispatch(idx, size.x/8, size.y / 8, 1);

        //copy back
        Graphics.CopyTexture(copyTex, tex);
    }

    private void UpdateAllAgents()
    {
        int dif = sizeof(float) * 3;
        ComputeBuffer comp = new ComputeBuffer(numAgents, dif);
        comp.SetData(dat);

        int idx = computeShader.FindKernel("CSMain");
        computeShader.SetBuffer(idx, "agents", comp);
        computeShader.SetFloat("moveSpeed", moveSpeed);
        computeShader.SetFloat("deltaTime", Time.deltaTime);
        computeShader.SetInt("width", size.x);
        computeShader.SetInt("height", size.y);
        computeShader.SetTexture(idx, "map", tex);

        computeShader.SetFloat("sensorAngleSpacing", sensorAngle);
        computeShader.SetFloat("sensorOffsetDist", sensorOffsetDist);
        computeShader.SetFloat("turnSpeed", turnSpeed);
        computeShader.SetInt("sensorSize", sensorSize);

        computeShader.Dispatch(idx, numAgents/16, 1, 1);

        comp.GetData(dat);
        comp.Dispose();

        //update to image
        RenderTexture.active = tex;

        texImg.ReadPixels(new Rect(0, 0, size.x, size.y), 0, 0);
        texImg.Apply();
        img.material.mainTexture = texImg;
    }
}
