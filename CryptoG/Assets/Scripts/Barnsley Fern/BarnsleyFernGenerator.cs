using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarnsleyFernGenerator : MonoBehaviour
{
    struct affineTrans
    {
        public float a,b,c,d,e,f;

        public affineTrans(float a1, float a2, float a3, float a4, float a5, float a6)
        {
            a = a1; b = a2; c = a3;
            d = a4; e = a5; f = a6;
        }
    }

    [Header("References")]
    [SerializeField] private GameObject pointPref;

    [Header("Main options")]
    [SerializeField] private float pointSize = 0.05f;
    [SerializeField] private int pointLimit = 10000;
    [SerializeField] private int pointMul = 5;
    [SerializeField] private float delay = 0.01f;
    [SerializeField] private float maxXRange = 3f;
    [SerializeField] private Color fernColor = Color.green;

    private List<BarnsleyFernPoint> points;
    private List<affineTrans> trans = new List<affineTrans>() {
        new affineTrans(0, 0, 0, 0.16f, 0, 0),
        new affineTrans(0.85f, 0.04f, -0.04f, 0.85f, 0, 1.6f),
        new affineTrans(0.20f, -0.26f, 0.23f, 0.22f, 0, 0.07f),
        new affineTrans(-0.15f, 0.28f, 0.26f, 0.24f, 0, 0.07f),
    };
    private float leftBound, rightBound, topBound, bottomBound;

    private void Start()
    {
        GenerateBarnsleyFern();
    }

    private void GenerateBarnsleyFern()
    {
        DestroyEvidence();
        points = new List<BarnsleyFernPoint>();
        GetBounds();
        StartCoroutine(GeneratePoints());
    }

    private void DestroyEvidence()
    {
        foreach(Transform child in transform) {Destroy(child.gameObject);}
    }

    private void GetBounds()
    {
        Vector2 bPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
        rightBound = bPos.x; leftBound = -rightBound;
        topBound = bPos.y; bottomBound = -topBound;

        maxXRange = Mathf.Clamp(maxXRange, leftBound, rightBound);
    }

    IEnumerator GeneratePoints()
    {
        Vector2 original = new Vector2(0f, 0f);
        for (int idx = 0; idx < pointLimit; idx++)
        {
            for (int j = 0; j < pointMul; j++)
            {
                CreatePoint(GetFixedPoint(original));
                original = GetTransformPoint(original);
            }
            yield return new WaitForSeconds(delay);
        }
    }

    private Vector2 GetFixedPoint(Vector2 original)
    {
        original.x = Mathf.Clamp(original.x, -2.1820f, 2.6558f);
        original.y = Mathf.Clamp(original.y, 0f, 9.9983f);

        original.x = UtilityFunc.Remap(original.x, -2.1820f, 2.6558f, -maxXRange, maxXRange);
        original.y = UtilityFunc.Remap(original.y, 0f, 9.9983f, bottomBound, topBound);

        return original;
    }

    private Vector2 GetTransformPoint(Vector2 pos)
    {
        float randPosi = UnityEngine.Random.Range(0f, 1f);
        if (randPosi < 0.01)  { return TransAff(pos, 0);}
        else if (randPosi < 0.86) { return TransAff(pos, 1);}
        else if (randPosi < 0.93) { return TransAff(pos, 2);}
        else { return TransAff(pos, 3);}
    }

    private Vector2 TransAff(Vector2 pos, int idx)
    {
        affineTrans cur = trans[idx];
        return new Vector2(pos.x * cur.a + pos.y * cur.b, pos.x * cur.c + pos.y * cur.d) + new Vector2(cur.e, cur.f);
    }

    private void CreatePoint(Vector2 pos)
    {
        GameObject newPoint = Instantiate(pointPref, pos, Quaternion.identity, transform);
        BarnsleyFernPoint po = newPoint.GetComponent<BarnsleyFernPoint>();

        po.SetSize(pointSize);
        po.SetColor(fernColor);

        points.Add(po);
    }
}
