using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MarchingUIController : MonoBehaviour
{
    [SerializeField] private MarchingSquare marchingSquare;
    [Header("UI")]
    [SerializeField] private SliderController sideSize;
    [SerializeField] private SliderController lineWidth;
    [SerializeField] private Toggle perlinToggle;
    [SerializeField] private SliderController inc;
    [SerializeField] private Toggle debugToggle;

    public void Rebuild()
    {
        marchingSquare.SetSideSize(sideSize.GetValue());
        marchingSquare.SetLineWidth(lineWidth.GetValue());
        marchingSquare.SetPerlinToggle(perlinToggle.isOn);
        marchingSquare.SetInc(inc.GetValue());
        marchingSquare.SetDebugToggle(debugToggle.isOn);

        marchingSquare.NewMarchingScreen();
    }
}
