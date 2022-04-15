using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using System.Linq;

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
    [SerializeField] private float delay = 0.2f;

    public bool shouldRun = false;
    private bool startJob = false;
    private List<List<Vector2> > cur, next;
    private int width, height;

    private NativeArray<float2> curArr, nextArr; //test with job, allocator persistent, dispose onApplicationQuit()

    private bool bufferFlipped = false;
    private NativeSlice<float2> frontBuffer { get => bufferFlipped ? nextArr : curArr; }
    private NativeSlice<float2> backBuffer  { get => bufferFlipped ? curArr : nextArr; }

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
        startJob = false;

        width = screenController.GetWidth();
        height = screenController.GetHeight();

        cur = new List<List<Vector2>>();
        next = new List<List<Vector2>>();

        try
        {
            curArr.Dispose();
            nextArr.Dispose();
        }
        catch { Debug.Log("Non-disposable"); }

        curArr = new NativeArray<float2>(height*width, Allocator.Persistent);
        nextArr = new NativeArray<float2>(height*width, Allocator.Persistent);

        for (int idx = 0; idx < height; idx++)
        {
            cur.Add(new List<Vector2>());
            next.Add(new List<Vector2>());

            for (int j = 0; j < width; j++)
            {
                cur[idx].Add(new Vector2(1.0f, 0f)); //full of A
                next[idx].Add(new Vector2(0f, 0f));

                curArr[idx * height + j] = cur[idx][j];
                nextArr[idx * height + j] = cur[idx][j];
            }
        }

        for (int idx = height/2 - offset/2; idx < height / 2 + offset/2; idx++)
        {
            for (int j = width/2 - offset/2; j < width/2 + offset/2; j++)
            {
                cur[idx][j] = new Vector2(0.1f, 1.0f); //drop of B

                curArr[idx * height + j] = cur[idx][j];
                nextArr[idx * height + j] = cur[idx][j];
            }
        }
    }

    void Update()
    {
        if (shouldRun) 
        {
            if (!useJob) { RenderCurrentReaction(); ReactDiffuse(); }
            else 
            {
                if (!startJob)
                {
                    startJob = true;
                    StartCoroutine(StartADiffuseWithJob());
                }
            }
        }
    }

    IEnumerator StartADiffuseWithJob()
    {
        while(shouldRun)
        {
            RenderCurrentReactionJob(); ReactDiffuseJob();
            yield return new WaitForSeconds(delay);
        }
    }

    private void ReactDiffuseJob()
    {
        try
        {
            var bulkDiffuseJob = new CalculationDiffuse {
                width = width, height = height,
                dA = dA, dB = dB, k = k, f = f, timeScale = timeScale,
                cur = frontBuffer, next = backBuffer,
            };

            JobHandle job = bulkDiffuseJob.Schedule(height*width, 64);
            job.Complete();

            bufferFlipped = !bufferFlipped;
        }
        catch(Exception e)
        {
            Debug.LogError("Job failed : " + e);
        }
    }

    [BurstCompile]
    public struct CalculationDiffuse : IJobParallelFor
    {
        public int width, height;
        public float dA, dB, k, f, timeScale;
        [ReadOnly] public NativeSlice<float2> cur;
        [WriteOnly] public NativeSlice<float2> next;

        public void Execute(int index)
        {
            var y = index % width;
            var x = index / width;

            if (x <= 0 || x >= height-1 || y <= 0 || y >= width-1) { return;} //skip

            float curA = cur[x*height + y].x, curB = cur[x*height + y].y; 
            float newA = curA +
                        (dA * laplaceA(x, y) -
                        curA * curB * curB +
                        f * (1 - curA)) * timeScale;

            float newB = curB +
                        (dB * laplaceB(x, y) +
                        curA * curB * curB -
                        (k + f) * curB) * timeScale;

            newA = Clampy(newA, 0, 1);
            newB = Clampy(newB, 0, 1);

            next[x*height + y] = new float2(newA, newB);
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

    private void RenderCurrentReactionJob()
    {
        List<Color> colors = new List<Color>();
        for (int idx = 0; idx < height; idx++)
        {
            for (int j = 0; j < width; j++)
            {
                float2 tmp = frontBuffer[idx * height + j]; float val = tmp.x - tmp.y;
                val = Mathf.Clamp(val, 0f, 1f); 

                colors.Add(new Color(val, val, val, 1f));
            }
        }
        screenController.SetFullPixelScreen(colors.ToArray());
        screenController.ScreenApply();
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

    private void OnApplicationQuit() 
    {
        curArr.Dispose();
        nextArr.Dispose();
    }
}
