using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST : MonoBehaviour
{
    void Start()
    {
        Debug.Log(GetComponent<RectTransform>().TransformPoint(GetComponent<RectTransform>().rect.center));    
    }
}
