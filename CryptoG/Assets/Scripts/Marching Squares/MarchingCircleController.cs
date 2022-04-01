using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MarchingCircleController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    public void SetTextVal(float val)
    {
        text.text = System.Math.Round(val,3).ToString();
    }
}
