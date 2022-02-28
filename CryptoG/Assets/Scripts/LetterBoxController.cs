using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class LetterBoxController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI letterText;
    [SerializeField] private bool isLeftLetterBox;
    [SerializeField] private float lineWidth = 0.025f;
    [SerializeField] private float lineLength = 0.1f;

    public char GetLetter()
    {
        return letterText.text[0];
    }

    public void SetLetter(char x)
    {
        letterText.text = x.ToString();
    }

    private void Start()
    {
        StartCoroutine(DrawPlug());
    }

    IEnumerator DrawPlug()
    {
        yield return new WaitForEndOfFrame();

        //first point
        Vector2 point1;
        Vector3[] corners = new Vector3[4]; GetComponent<RectTransform>().GetWorldCorners(corners);
        if (isLeftLetterBox) 
        {
            point1.x = corners[3].x;
            point1.y = (corners[2].y + corners[3].y)/2;
        }
        else 
        {
            point1.x = corners[0].x;
            point1.y = (corners[0].y + corners[1].y)/2;
        }

        //second point
        Vector2 point2; point2.y = point1.y;

        if (isLeftLetterBox)    { point2.x = point1.x + lineLength; }
        else                    { point2.x = point1.x - lineLength; }

        GetComponent<LineRenderer>().positionCount = 2;
        GetComponent<LineRenderer>().SetPosition(0, point1);
        GetComponent<LineRenderer>().SetPosition(1, point2);
        GetComponent<LineRenderer>().startWidth     = lineWidth;
        GetComponent<LineRenderer>().endWidth       = lineWidth;
    }

    public Vector2 GetPlugPoint()
    {
        return GetComponent<LineRenderer>().GetPosition(1);
    }
}
