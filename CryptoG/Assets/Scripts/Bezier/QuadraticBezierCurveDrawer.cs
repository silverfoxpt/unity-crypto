using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadraticBezierCurveDrawer : MonoBehaviour
{
    [SerializeField] private GameObject linePref;
    [SerializeField] private GameObject pointPref;

    [Header("Points")]
    [SerializeField] private GameObject point1;
    [SerializeField] private GameObject point2;
    [SerializeField] private GameObject point3;

    [Header("Animation options")]
    [SerializeField] private float delay = 0.05f;
    [SerializeField] private Color mainGuideColor;

    [Header("Other options")]
    [SerializeField] private float interpolationIncrements = 0.01f;
    [SerializeField] private float lineWidth;
    [SerializeField] private Vector2 topRightBound;
    
    private LineRenderer myRend;
    private LineRenderer guide1, guide2, mainGuide;
    private GameObject guidePoint;
    private bool animated = false;

    private void Start()
    {
        InitializeDrawing();

        //StartCoroutine(DrawBezierCurveVisualGuide());
    }

    private void DestroyCurve()
    {
        myRend.positionCount = 0;
    }

    private void InitializeDrawing()
    {
        Vector2 p1 = GetPointPos(point1), p2 = GetPointPos(point2), p3 = GetPointPos(point3);
        point1.GetComponent<BezierPointController>().SetBounds(topRightBound);
        point2.GetComponent<BezierPointController>().SetBounds(topRightBound);
        point3.GetComponent<BezierPointController>().SetBounds(topRightBound);

        myRend = gameObject.GetComponent<LineRenderer>();
        myRend.positionCount = 0;
        myRend.startWidth = lineWidth;
        myRend.endWidth = lineWidth;

        GameObject newLine = Instantiate(linePref, new Vector3(0f, 0f, 0f), Quaternion.identity, transform);
        guide1 = newLine.GetComponent<LineRenderer>();

        newLine = Instantiate(linePref, new Vector3(0f, 0f, 0f), Quaternion.identity, transform);
        guide2 = newLine.GetComponent<LineRenderer>();

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

    IEnumerator DrawBezierCurveVisualGuide()
    {
        DestroyCurve();
        mainGuide.enabled = true;
        guidePoint.SetActive(true);
        animated = true;

        point1.GetComponent<BezierPointController>().LockDrag();
        point2.GetComponent<BezierPointController>().LockDrag();
        point3.GetComponent<BezierPointController>().LockDrag();
        
        Vector2 p1 = GetPointPos(point1), p2 = GetPointPos(point2), p3 = GetPointPos(point3);
        int counter = 0;

        for (float t = 0f; t <= 1f; t += interpolationIncrements)
        {
            Vector2 midp1 = Vec2Lerp(p1, p2, t);
            Vector2 midp2 = Vec2Lerp(p2, p3, t);
            Vector2 mainPoint = Vec2Lerp(midp1, midp2, t);

            myRend.positionCount = counter + 1;
            myRend.SetPosition(counter, mainPoint);
            counter++;

            mainGuide.SetPosition(0, midp1);
            mainGuide.SetPosition(1, midp2);
            guidePoint.transform.position = mainPoint;

            yield return new WaitForSeconds(delay);
        }

        DestroyCurve();
        mainGuide.enabled = false;
        guidePoint.SetActive(false);
        animated = false;

        point1.GetComponent<BezierPointController>().UnlockDrag();
        point2.GetComponent<BezierPointController>().UnlockDrag();
        point3.GetComponent<BezierPointController>().UnlockDrag();
    }

    private void RedrawBezierCurve()
    {
        DestroyCurve();
        Vector2 p1 = GetPointPos(point1), p2 = GetPointPos(point2), p3 = GetPointPos(point3);
        int counter = 0;

        guide1.SetPosition(0, p1);
        guide1.SetPosition(1, p2);

        guide2.SetPosition(0, p2);
        guide2.SetPosition(1, p3);

        for (float t = 0f; t <= 1f; t += interpolationIncrements)
        {
            Vector2 midp1 = Vec2Lerp(p1, p2, t);
            Vector2 midp2 = Vec2Lerp(p2, p3, t);
            Vector2 mainPoint = Vec2Lerp(midp1, midp2, t);

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

