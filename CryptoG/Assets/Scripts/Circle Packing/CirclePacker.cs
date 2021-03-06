using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CirclePacker : MonoBehaviour
{
    [SerializeField] private GameObject circlePref;
    [SerializeField] private int circleLimit;
    [SerializeField] private float circleGrowthRate = 0.1f;
    [SerializeField] private float circleRefreshRate = 0.01f;
    [SerializeField] private float maxCircleSize = 2f;

    [Header("Image")]
    [SerializeField] private bool useImage = false;
    [SerializeField] private Sprite image;

    [Header("Other options")]
    [SerializeField] private bool fastFill = false;
    [SerializeField] private bool allowOverlap = false;

    private List<CircleControllerPack> circles = new List<CircleControllerPack>();
    private float leftBound, rightBound, topBound, bottomBound;
    private Vector2 notFound = new Vector2(-100000f, -100000f);
    private Texture2D imgTexture; private int imgWidth, imgHeight;

    public void SetCircleLimit(int circleLim) {circleLimit = circleLim;}
    public void SetCircleGrowth(float gr) {circleGrowthRate = gr;}
    public void SetCircleRefresh(float rf) {circleRefreshRate = rf;}
    public void SetMaxCircle(float mx) {maxCircleSize = mx;}

    public void SetUseImg(bool im) {useImage = im;}
    public void SetImg(Sprite img) {image = img;}

    public void SetFastFill(bool fast) {fastFill = fast;}
    public void SetOverlap(bool lap) {allowOverlap = lap;}


    void Start()
    {
        //CirclePack();
    }

    public void CirclePack()
    {
        DeleteEverything();
        GetBounds();
        if (image) {SetupImage();}

        if (!fastFill) { StartCoroutine(GenerateAllCirclesSingle()); }
        else { GenerateAllRandomCircles(); }
    }

    private void DeleteEverything()
    {
        circles = new List<CircleControllerPack>();
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void SetupImage()
    {
        imgTexture = image.texture;
        imgWidth = imgTexture.width;
        imgHeight = imgTexture.height;
    }

    private void GenerateAllRandomCircles()
    {
        for (int idx = 0; idx < circleLimit; idx++)
        {
            GenerateSingleCircle(UtilityFunc.GetRandPos(leftBound, rightBound, bottomBound, topBound));
        }
    }

    private void Update() 
    {
        for (int idx = 0; idx < circles.Count; idx++)
        {
            if (!CheckScreenBounds(idx) || !CheckOtherCirclesBounds(idx) || circles[idx].GetSize() > maxCircleSize) {circles[idx].TurnOffGrowing();}
        }
    }

    private bool CheckScreenBounds(int cirIdx)
    {
        Vector2 center = circles[cirIdx].GetCenter();
        float size = circles[cirIdx].GetSize();

        float leftSide = center.x - size/2f, rightSize = center.x + size/2f;
        float topSide = center.y + size/2f, bottomSize = center.y - size/2f;

        if (leftSide < leftBound || rightSize > rightBound || topSide > topBound || bottomSize < bottomBound) {return false;}
        return true;
    }

    private bool CheckOtherCirclesBounds(int cirIdx)
    {
        Vector2 center = circles[cirIdx].GetCenter();
        float size = circles[cirIdx].GetSize();

        for (int idx = 0; idx < circles.Count; idx++)
        {
            if (idx == cirIdx) {continue;}

            Vector2 otherCenter = circles[idx].GetCenter();
            float otherSize = circles[idx].GetSize();

            float dist = Vector2.Distance(center, otherCenter);
            float r0 = size/2f, r1 = otherSize/2f;

            if (Mathf.Abs(r0 - r1) <= dist && dist <= r0 + r1) //if r + R > d
            {
                return false;
            }
        }

        return true;
    }

    IEnumerator GenerateAllCirclesSingle()
    {
        while(circles.Count <= circleLimit)
        {
            Vector2 pos = GetSinglePos();
            if (pos == notFound) {yield break;}

            var cir = GenerateSingleCircle(pos);
            yield return new WaitWhile(() => cir.Growing());
        }
    }

    private CircleControllerPack GenerateSingleCircle(Vector2 pos)
    {
        var cir = Instantiate(circlePref, pos, Quaternion.identity, transform).GetComponent<CircleControllerPack>();
        cir.SetSize(0f);
        cir.SetGrowthRate(circleGrowthRate);
        if (!useImage) { cir.SetColor(UtilityFunc.GetRandColor());}
        else {cir.SetColor(GetImageColor(pos));}
        cir.SetRefresh(circleRefreshRate);

        circles.Add(cir); return cir;
    }

    private Color GetImageColor(Vector2 pos)
    {
        int xPos = Mathf.Clamp((int) UtilityFunc.Remap(pos.x, leftBound, rightBound, 0f, imgWidth), 0, imgWidth);
        int yPos = Mathf.Clamp((int) UtilityFunc.Remap(pos.y, bottomBound, topBound, 0f, imgHeight), 0, imgHeight);

        return imgTexture.GetPixel(xPos, yPos);        
    }

    private void GetBounds()
    {
        Vector2 maxPos = Camera.main.ViewportToWorldPoint(new Vector3(1,1,Camera.main.nearClipPlane));
        rightBound = Mathf.Abs(maxPos.x); leftBound = -rightBound;
        topBound = Mathf.Abs(maxPos.y); bottomBound = -topBound;
        Debug.Log(rightBound);
        Debug.Log(topBound);
    }

    private Vector2 GetSinglePos()
    {
        Vector2 pos = notFound; int limit = 100000;
        int counter = 0; bool ok = true;
        while(counter <= limit)
        {
            counter++;
            pos = UtilityFunc.GetRandPos(leftBound, rightBound, bottomBound, topBound); ok = true; 

            for (int idx = 0; idx < circles.Count; idx++)
            {
                if (allowOverlap) {break;}
                Vector2 cent = circles[idx].GetCenter();
                float size = circles[idx].GetSize()/2f; 

                if (UtilityFunc.Dist(pos, cent) <= size) { ok = false; break;}
            }
            if (ok) {return pos;}
        }
        return notFound;
    }    
}

