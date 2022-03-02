using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnoverController : MonoBehaviour
{
    [SerializeField] private GameObject fullRing;
    [SerializeField] private GameObject leftRing, rightRing;
    [SerializeField] private Color notchColor;
    [SerializeField] private float notchWidth = 0.05f;
    private List<int> notchIndexes = new List<int>();
    private List<GameObject> notchObjs = new List<GameObject>();
    private List<GameObject> leftBox, rightBox;

    void Start()
    {
        GetNeccesaryComponents();
        StartCoroutine(RefreshNotches());
    }

    private void GetNeccesaryComponents()
    {
        foreach(Transform child in transform)
        {
            notchObjs.Add(child.gameObject);
        }
        leftBox = leftRing.GetComponent<HalfRingController>().GetLetterBoxes();
        rightBox = rightRing.GetComponent<HalfRingController>().GetLetterBoxes();
    }

    //NOTE: refresh EVERYTHING
    IEnumerator RefreshNotches()
    {
        yield return new WaitForEndOfFrame();

        notchIndexes = new List<int>(); //refresh
        List<char> notches = fullRing.GetComponent<FullRingController>().GetTurnover();
        foreach (char notch in notches)
        {
            notchIndexes.Add((int) (notch - 'A'));
        }
        DrawNotches();
        PushNotchesForwardOnce();
    }

    private void DrawNotches()
    {
        int cnt = 0;
        foreach(int notchIdx in notchIndexes)        
        {
            //Debug.Log("ranned");
            Vector2 pos1 = leftBox[notchIdx].GetComponent<LetterBoxController>().GetPlugPoint();
            Vector2 pos2 = rightBox[notchIdx].GetComponent<LetterBoxController>().GetPlugPoint();

            //draw notch
            LineRenderer line = notchObjs[cnt].GetComponent<LineRenderer>();
            line.positionCount = 2;
            line.SetPosition(0, pos1); line.SetPosition(1, pos2);
            line.endWidth = notchWidth; line.startWidth = notchWidth;
            line.endColor = notchColor; line.startColor = notchColor;
            //up
            cnt++;
        }
    }

    public void PushNotchesForwardOnce()
    {
        for (int i = 0; i < notchIndexes.Count; i++) { notchIndexes[i]++; if (notchIndexes[i] >= 26) {notchIndexes[i] = 0;}}

        int cnt = 0;
        foreach(int notchIdx in notchIndexes)        
        {
            //Debug.Log("ranned");
            Vector2 pos1 = leftBox[notchIdx].GetComponent<LetterBoxController>().GetPlugPoint();
            Vector2 pos2 = rightBox[notchIdx].GetComponent<LetterBoxController>().GetPlugPoint();

            //draw notch
            LineRenderer line = notchObjs[cnt].GetComponent<LineRenderer>();
            line.SetPosition(0, pos1); line.SetPosition(1, pos2);
            //up
            cnt++;
        }
    }
}
