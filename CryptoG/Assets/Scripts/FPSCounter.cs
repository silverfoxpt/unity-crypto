using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI te;

    void Update()
    {
        te.text = "FPS: " + System.Math.Round(1f/Time.unscaledDeltaTime, 2).ToString();    
    }
}
