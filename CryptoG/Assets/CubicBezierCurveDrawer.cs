using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubicBezierCurveDrawer : MonoBehaviour
{
    [SerializeField] private GameObject linePref;
    [SerializeField] private GameObject pointPref;

    [Header("Points")]
    [SerializeField] private GameObject point1;
    [SerializeField] private GameObject point2;
    [SerializeField] private GameObject point3;
    [SerializeField] private GameObject point4;

    [Header("Animation options")]
    [SerializeField] private float delay = 0.05f;
    [SerializeField] private Color mainGuideColor;
    [SerializeField] private Color extraGuidecolor;

    [Header("Other options")]
    [SerializeField] private float interpolationIncrements = 0.01f;
    [SerializeField] private float lineWidth;
    [SerializeField] private Vector2 topRightBound;
    
    private LineRenderer myRend;
    private LineRenderer guide1, guide2, guide3, guidex1, guidex2, mainGuide;
    private GameObject guidePoint;
    private bool animated = false;

    public void SetAnimationDelay(float an) {delay = an;}
    public void SetIncrements(float inc) {interpolationIncrements = inc;}
    public void SetLineWidth(float li) {lineWidth = li;}

    private void Start()
    {
        RefreshCurve();

        //StartCoroutine(DrawBezierCurveVisualGuide());
    }

    public void RefreshCurve()
    {
        foreach(Transform child in transform) {Destroy(child.gameObject);}
        InitializeDrawing();
    }

    private void DestroyCurve()
    {
        myRend.positionCount = 0;
    }

    private void InitializeDrawing()
    {
        Vector2 p1 = GetPointPos(point1), p2 = GetPointPos(point2), p3 = GetPointPos(point3), p4 = GetPointPos(point4);
        point1.GetComponent<BezierPointController>().SetBounds(topRightBound);
        point2.GetComponent<BezierPointController>().SetBounds(topRightBound);
        point3.GetComponent<BezierPointController>().SetBounds(topRightBound);
        point4.GetComponent<BezierPointController>().SetBounds(topRightBound);

        myRend = gameObject.GetComponent<LineRenderer>();
        myRend.positionCount = 0;
        myRend.startWidth = lineWidth;
        myRend.endWidth = lineWidth;

        GameObject newLine = Instantiate(linePref, new Vector3(0f, 0f, 0f), Quaternion.identity, transform);
        guide1 = newLine.GetComponent<LineRenderer>();

        newLine = Instantiate(linePref, new Vector3(0f, 0f, 0f), Quaternion.identity, transform);
        guide2 = newLine.GetComponent<LineRenderer>();
        
        newLine = Instantiate(linePref, new Vector3(0f, 0f, 0f), Quaternion.identity, transform);
        guide3 = newLine.GetComponent<LineRenderer>();

        newLine = Instantiate(linePref, new Vector3(0f, 0f, 0f), Quaternion.identity, transform);
        guidex1 = newLine.GetComponent<LineRenderer>();

        newLine = Instantiate(linePref, new Vector3(0f, 0f, 0f), Quaternion.identity, transform);
        guidex2 = newLine.GetComponent<LineRenderer>();

        newLine = Instantiate(linePref, new Vector3(0f, 0f, 0f), Quaternion.identity, transform);
        mainGuide = newLine.GetComponent<LineRenderer>();

        //set up new guidelines
        guide1.positionCount = 2;
        guide1.startWidth = lineWidth;
        guide1.endWidth = lineWidth;
        guide1.SetPosition(0, p1);
        guide1.SetPosition(1, p2);

        guide2.positionCount = 2;
        guide2.startWidth = lineWidth;
        guide2.endWidth = lineWidth;
        guide2.SetPosition(0, p2);
        guide2.SetPosition(1, p3);

        guide3.positionCount = 2;
        guide3.startWidth = lineWidth;
        guide3.endWidth = lineWidth;
        guide3.SetPosition(0, p3);
        guide3.SetPosition(1, p4);

        guidex1.positionCount = 2;
        guidex1.startWidth = lineWidth;
        guidex1.endWidth = lineWidth;
        guidex1.startColor = extraGuidecolor;
        guidex1.endColor = extraGuidecolor;

        guidex2.positionCount = 2;
        guidex2.startWidth = lineWidth;
        guidex2.endWidth = lineWidth;
        guidex2.startColor = extraGuidecolor;
        guidex2.endColor = extraGuidecolor;

        mainGuide.positionCount = 2;
        mainGuide.startWidth = lineWidth;
        mainGuide.endWidth = lineWidth;
        mainGuide.startColor = mainGuideColor;
        mainGuide.endColor = mainGuideColor;

        //new guide point
        guidePoint = Instantiate(pointPref, p1, Quaternion.identity, transform);
        guidePoint.SetActive(false);
    }

    private void Update()
    {
        if (!animated) { RedrawBezierCurve(); }
    }

    public IEnumerator DrawBezierCurveVisualGuide()
    {
        DestroyCurve();
        mainGuide.enabled = true;
        guidex1.enabled = true;
        guidex2.enabled = true;
        guidePoint.SetActive(true);
        animated = true;

        point1.GetComponent<BezierPointController>().LockDrag();
        point2.GetComponent<BezierPointController>().LockDrag();
        point3.GetComponent<BezierPointController>().LockDrag();
        point4.GetComponent<BezierPointController>().LockDrag();

        Vector2 p1 = GetPointPos(point1), p2 = GetPointPos(point2), p3 = GetPointPos(point3), p4 = GetPointPos(point4);
        int counter = 0;

        for (float t = 0f; t <= 1f; t += interpolationIncrements)
        {
            Vector2 midp1 = Vec2Lerp(p1, p2, t);
            Vector2 midp2 = Vec2Lerp(p2, p3, t);
            Vector2 midp3 = Vec2Lerp(p3, p4, t);

            Vector2 exp1 = Vec2Lerp(midp1, midp2, t);
            Vector2 exp2 = Vec2Lerp(midp2, midp3, t);

            Vector2 mainPoint = Vec2Lerp(exp1, exp2, t);

            myRend.positionCount = counter + 1;
            myRend.SetPosition(counter, mainPoint);
            counter++;

            guidex1.SetPosition(0, midp1);
            guidex1.SetPosition(1, midp2);

            guidex2.SetPosition(0, midp2);
            guidex2.SetPosition(1, midp3);

            mainGuide.SetPosition(0, exp1);
            mainGuide.SetPosition(1, exp2);
            guidePoint.transform.position = mainPoint;

            yield return new WaitForSeconds(delay);
        }

        DestroyCurve();
        mainGuide.enabled = false;
        guidePoint.SetActive(false);
        guidex1.enabled = false;
        guidex2.enabled = false;
        animated = false;

        point1.GetComponent<BezierPointController>().UnlockDrag();
        point2.GetComponent<BezierPointController>().UnlockDrag();
        point3.GetComponent<BezierPointController>().UnlockDrag();
        point4.GetComponent<BezierPointController>().UnlockDrag();
    }

    private void RedrawBezierCurve()
    {
        DestroyCurve();
        Vector2 p1 = GetPointPos(point1), p2 = GetPointPos(point2), p3 = GetPointPos(point3), p4 = GetPointPos(point4);
        int counter = 0;

        guide1.SetPosition(0, p1);
        guide1.SetPosition(1, p2);

        guide2.SetPosition(0, p2);
        guide2.SetPosition(1, p3);

        guide3.SetPosition(0, p3);
        guide3.SetPosition(1, p4);

        for (float t = 0f; t <= 1f; t += interpolationIncrements)
        {
            Vector2 midp1 = Vec2Lerp(p1, p2, t);
            Vector2 midp2 = Vec2Lerp(p2, p3, t);
            Vector2 midp3 = Vec2Lerp(p3, p4, t);

            Vector2 exp1 = Vec2Lerp(midp1, midp2, t);
            Vector2 exp2 = Vec2Lerp(midp2, midp3, t);

            Vector2 mainPoint = Vec2Lerp(exp1, exp2, t);

            myRend.positionCount = counter + 1;
            myRend.SetPosition(counter, mainPoint);
            counter++;
        }
    }

    private Vector2 Vec2Lerp(Vector2 pa, Vector2 pb, float t) //Lerp between 2 points!
    {
        float x = Mathf.Lerp(pa.x, pb.x, t);
        float y = Mathf.Lerp(pa.y, pb.y, t);
        return new Vector2(x, y);
    }

    private Vector2 GetPointPos(GameObject po)
    {
        return po.transform.position;
    }
}
