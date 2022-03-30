using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactionDiffusion : MonoBehaviour
{
    [SerializeField] private ScreenController screenController;
    private List<List<Vector2> > cur, next;
    private int width, height;

    [SerializeField] private float dA = 1;
    [SerializeField] private float dB = 0.5f;
    [SerializeField] private float f = 0.055f;
    [SerializeField] private float k = 0.062f;
    [SerializeField] private int offset = 1;
    [SerializeField] private float timeScale = 5f;

    public bool shouldRun = false;

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
        if (shouldRun) {RenderCurrentReaction(); ReactDiffuse();}
    }

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
}
