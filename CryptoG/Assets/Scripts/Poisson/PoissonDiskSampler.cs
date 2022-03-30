using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoissonDiskSampler : MonoBehaviour
{
    struct idxPair
    {
        public int x, y;

        public idxPair(int a, int b) {this.x = a; this.y = b;}
    }
    [SerializeField] private GameObject circlePref;
    [SerializeField] private int halfWidth = 10;
    [SerializeField] private int halfHeight = 10;
    [SerializeField] private float circleWidth;

    [SerializeField] private float r = 2f;
    [SerializeField] private int k = 30;
    private float cellSize;
    private float xBound, yBound;
    private Dictionary<idxPair, Vector2> circles = new Dictionary<idxPair, Vector2>();
    private Vector2 notFound = new Vector2(-1000000f, -1000000f);
    private List<Vector2> active = new List<Vector2>();

    private void Start()
    {
        //StartPoisson();
    }

    public void StartPoisson()
    {
        ClearOld();
        InitializeVars();
        RandomCircle();

        StartCoroutine(Poisson());
    }

    private void ClearOld()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    IEnumerator Poisson()
    {
        //STEP 2
        Vector2 pos; GameObject circle = null;
        while (active.Count > 0)
        {
            yield return new WaitForSeconds(0.01f);
            int idx = Mathf.FloorToInt(UnityEngine.Random.Range(0f, active.Count - 1)); pos = active[idx];

            bool found = false;
            for (int j = 0; j < k; j++) //try and choose
            {
                float angle = UnityEngine.Random.Range(0, 2 * Mathf.PI);
                float offX = Mathf.Cos(angle), offY = Mathf.Sin(angle);
                float mag = UnityEngine.Random.Range(r, 2 * r);
                Vector2 choosePos = (new Vector2(offX, offY).normalized * mag) + pos;

                int curX = Mathf.FloorToInt(choosePos.x / cellSize);
                int curY = Mathf.FloorToInt(choosePos.y / cellSize);
                if (!withinBorder(curX, curY)) { continue;}

                bool ok = true;
                for (int m = -1; m <= 1; m++)
                {
                    for (int n = -1; n <= 1; n++)
                    {
                        int newX = curX + m, newY = curY + n;
                        if (withinBorder(newX, newY))
                        {
                            Vector2 neighborPos = circles[new idxPair(newX, newY)];
                            if (neighborPos == notFound) {continue;}
                            if (Vector2.Distance(neighborPos, choosePos) < r) { ok = false; }
                        }
                    }
                }
                if (ok)
                {
                    found = true;
                    circles[new idxPair(curX, curY)] = choosePos; active.Add(choosePos);
                    
                    circle = Instantiate(circlePref, (Vector3)choosePos, Quaternion.identity, transform);
                    circle.GetComponent<CircleController>().ChangeSize(circleWidth);
                }
            }
            if (!found) { active.RemoveAt(idx); }
        }
    }

    private void RandomCircle()
    {
        //STEP 1
        float x = 0, y = 0;
        int idxI = Mathf.FloorToInt(x / cellSize);
        int idxJ = Mathf.FloorToInt(y / cellSize);

        Vector2 pos = new Vector2(x, y);
        GameObject circle = Instantiate(circlePref, (Vector3)pos, Quaternion.identity, transform); 
        circle.GetComponent<CircleController>().ChangeSize(circleWidth);
        circles[new idxPair(idxI, idxJ)]  = pos;
        active.Add(pos);
    }

    private bool withinBorder(int x, int y)
    {
        if (x < -halfWidth || x >= halfWidth || y < -halfHeight || y >= halfHeight) {return false;} return true;
    }

    private void InitializeVars()
    {
        cellSize = r / Mathf.Sqrt(2); 
        xBound = halfWidth * cellSize;
        yBound = halfHeight / 2.0f * cellSize;

        circles = new Dictionary<idxPair, Vector2>();
        active = new List<Vector2>();        

        for (int idx = -halfWidth; idx < halfWidth; idx++)
        {
            for (int j = -halfHeight; j < halfHeight; j++)
            {
                circles.Add(new idxPair(idx, j), notFound);
            }
        }
    }

    public void SetWidth(float w) {halfWidth = (int) w;}
    public void SetHeight(float h) {halfHeight = (int) h;}
    public void SetRad(float r) {circleWidth = r;}
    public void SetDist(float d) {r = d;}
    public void SetSample(float s) {k = (int) s;}
}

