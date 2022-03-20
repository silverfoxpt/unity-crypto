using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointPicker : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }
}
