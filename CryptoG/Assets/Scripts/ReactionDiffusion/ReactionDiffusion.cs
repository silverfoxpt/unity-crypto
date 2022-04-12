using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;

public class ReactionDiffusion : MonoBehaviour
{
    [Header("Use with precaution")]
    [SerializeField] private bool useJob = false;

    [Header("References")]
    [SerializeField] private ScreenController screenController;

    [Header("Options")]
    [SerializeField] private float dA = 1;
    [SerializeField] private float dB = 0.5f;
    [SerializeField] private float f = 0.055f;
    [SerializeField] private float k = 0.062f;
    [SerializeField] private int offset = 1;
    [SerializeField] private float timeScale = 5f;

    public bool shouldRun = false;
    private List<List<Vector2> > cur, next;
    private int width, height;

    void Awake()
    {
        //InitializeConditions();
    }

    public void ChangeOffset(float off) {offset = (int) off;}
    public void ChangeFeed(float feed) {f = feed;}
    public void ChangeKill(float kill) {k = kill;}
    public void ChangeDA(float da) {dA = da;}
    public void ChangeDB(float db) {dB = db;}

    public void InitializeConditions()
    {
        width = screenController.GetWidth();
        height = screenController.GetHeight();
        cur = new List<List<Vector2>>();

        for (int idx = 0; idx < height; idx++)
        {
            cur.Add(new List<Vector2>());
            for (int j = 0; j < width; j++)
            {
                cur[idx].Add(new Vector2(1.0f, 0f)); //full of A
            }
        }

        for (int idx = height/2 - offset/2; idx < height / 2 + offset/2; idx++)
        {
            for (int j = width/2 - offset/2; j < width/2 + offset/2; j++)
            {
                cur[idx][j] = new Vector2(0.1f, 1.0f); //drop of B
            }
        }

        next = new List<List<Vector2>>(cur);
    }

    void Update()
    {
        if (shouldRun) 
        {
            RenderCurrentReaction(); 

            if (!useJob) { ReactDiffuse(); }
            else {ReactDiffuseJob();}
        }
    }

    private void ReactDiffuseJob()
    {
        /*NativeArray<float2> curNative = new NativeArray<float2>(width, Allocator.TempJob);
        NativeArray<float2> nextNative = new NativeArray<float2>(width, Allocator.TempJob);

        for (int idx = 0; idx < height; idx++)
        {
            for (int j = 0; j < width; j++) {curNative[idx * height + j] = cur[idx][j];} //flatten to nativeArr
        }*/

        for (int idx = 0; idx < height; idx++)
        {
            NativeArray<float2> curNative = new NativeArray<float2>(height, Allocator.Temp);
            NativeArray<float2> nextNative = new NativeArray<float2>(height, Allocator.Temp);

            CalculationDiffuse calculationDiffuse = new CalculationDiffuse {
                width = width, height = height,
                dA = dA, dB = dB, k = k, f = f, timeScale = timeScale,
                cur = curNative, next = nextNative    
            };

            JobHandle jobHandle = calculationDiffuse.Schedule();

            curNative.Dispose();
            nextNative.Dispose();
        }
    }

    public struct CalculationDiffuse : IJob
    {
        public int width, height;
        public float dA, dB, k, f, timeScale;
        public NativeArray<float2> cur;
        public NativeArray<float2> next;

        public void Execute()
        {
            //if (index <= 0 || index >= height-1) {return;} //skip
            //Debug.Log(index.ToString());
            
            /*for (int j = 1; j < width-1; j++)
            {   
                float curA = cur[index*height + j].x, curB = cur[index*height + j].y; 

                float newA = curA +
                            (dA * laplaceA(index, j) -
                            curA * curB * curB +
                            f * (1 - curA)) * timeScale;

                float newB = curB +
                            (dB * laplaceB(index, j) +
                            curA * curB * curB -
                            (k + f) * curB) * timeScale;

                newA = Clampy(newA, 0, 1);
                newB = Clampy(newB, 0, 1);

                next[index*height + j] = new float2(newA, newB);
            }*/
            
        }

        private float Clampy(float val, int min, int max)
        {
            if (val < min) {return min;}
            if (val > max) {return max;}
            return val;
        }

        private float Absy(float x)
        {
            if (x < 0) {return -x;} return x;
        }

        private float laplaceA(int x, int y)
        {
            float sum = 0f; 
            for (int idx = -1; idx <= 1; idx++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int newX = x + idx, newY = y + j;
                    if (withinBorder(newX, newY))
                    {
                        float val = cur[newX*height + newY].x;

                        if (idx == 0 && j == 0) { sum += val * -1f;} //center
                        else if (Absy(idx) == 1 && Absy(j) == 1) {sum += val * 0.05f;} //diagonal
                        else { sum += val * 0.2f;} //adjacent
                    }
                }
            }
            return sum;
        }

        private float laplaceB(int x, int y)
        {
            float sum = 0f; 
            for (int idx = -1; idx <= 1; idx++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int newX = x + idx, newY = y + j;
                    if (withinBorder(newX, newY))
                    {
                        float val = cur[newX*height + newY].y;

                        if (idx == 0 && j == 0) { sum += val * -1f;} //center
                        else if (Absy(idx) == 1 && Absy(j) == 1) {sum += val * 0.05f;} //diagonal
                        else { sum += val * 0.2f;} //adjacent
                    }
                }
            }
            return sum;
        }

        private bool withinBorder(int x, int y)
        {
            if (x < 0 || x >= height || y < 0 || y >= height) {return false;} return true;
        }
    }
    #region notJob
    private void ReactDiffuse()
    {
        for (int idx = 1; idx < height-1; idx++)
        {
            for (int j = 1; j < width-1; j++)
            {
                float curA = cur[idx][j].x, curB = cur[idx][j].y; 

                float newA = curA +
                             (dA * laplaceA(idx, j) -
                             curA * curB * curB +
                             f * (1 - curA)) * timeScale;

                float newB = curB +
                             (dB * laplaceB(idx, j) +
                             curA * curB * curB -
                             (k + f) * curB) * timeScale;

                newA = Mathf.Clamp(newA, 0, 1);
                newB = Mathf.Clamp(newB, 0, 1);

                next[idx][j] = new Vector2(newA, newB);
            }
        }
        cur = new List<List<Vector2>>(next);
    }

    private void RenderCurrentReaction()
    {
        List<Color> colors = new List<Color>();
        for (int idx = 0; idx < height; idx++)
        {
            for (int j = 0; j < width; j++)
            {
                float val = cur[idx][j].x - cur[idx][j].y; 
                val = Mathf.Clamp(val, 0f, 1f); 
                //screenController.SetPixelScreen(idx, j, new Color(val, val, val, 1f), false); //grayscale
                colors.Add(new Color(val, val, val, 1f));
            }
        }
        screenController.SetFullPixelScreen(colors.ToArray());
        screenController.ScreenApply();
    }

    private float laplaceA(int x, int y)
    {
        float sum = 0f; 
        for (int idx = -1; idx <= 1; idx++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int newX = x + idx, newY = y + j;
                if (withinBorder(newX, newY))
                {
                    float val = cur[newX][newY].x;

                    if (idx == 0 && j == 0) { sum += val * -1f;} //center
                    else if (Mathf.Abs(idx) == 1 && Mathf.Abs(j) == 1) {sum += val * 0.05f;} //diagonal
                    else { sum += val * 0.2f;} //adjacent
                }
            }
        }
        return sum;
    }

    private float laplaceB(int x, int y)
    {
        float sum = 0f; 
        for (int idx = -1; idx <= 1; idx++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int newX = x + idx, newY = y + j;
                if (withinBorder(newX, newY))
                {
                    float val = cur[newX][newY].y;

                    if (idx == 0 && j == 0) { sum += val * -1f;} //center
                    else if (Mathf.Abs(idx) == 1 && Mathf.Abs(j) == 1) {sum += val * 0.05f;} //diagonal
                    else { sum += val * 0.2f;} //adjacent
                }
            }
        }
        return sum;
    }

    private bool withinBorder(int x, int y)
    {
        if (x < 0 || x >= height || y < 0 || y >= height) {return false;} return true;
    }
    #endregion
}
