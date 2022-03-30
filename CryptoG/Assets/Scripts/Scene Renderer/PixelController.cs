using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelController : MonoBehaviour
{
    private void Start()
    {
        //testPixel(); //-> ok
    }

    private void testPixel()
    {
        ChangeColor(new Color(255, 0, 0, 1));
        ChangeSize(2f);
        SetTopLeftPosition(new Vector3(0f, 0f, 0f));
    }

    public void ChangeColor(Color newColor)
    {
        GetComponent<SpriteRenderer>().color = newColor;
    }

    public void ChangeSize(float newSize)
    {
        transform.localScale = new Vector3(newSize, newSize, 0f);
    }

    public void SetCentralPosition(Vector3 pos)
    {
        transform.position = new Vector3(pos.x, pos.y, pos.z);
    }

    public float GetScale() {return transform.localScale.x;}

    public void SetTopLeftPosition(Vector3 pos)
    {
        float halfScale = GetScale()/2f;

        transform.position = new Vector3(pos.x + halfScale, pos.y + halfScale, pos.z);
    }
}
