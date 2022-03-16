using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MarkerController : MonoBehaviour
{
    [SerializeField] private Canvas myCanvas;
    [SerializeField] private TextMeshProUGUI textField;

    void Start()
    {
        myCanvas.worldCamera = Camera.main;
    }

    public void SetMarkerText(string tex, bool isXAxis = true)
    {
        if (isXAxis) { textField.alignment = TextAlignmentOptions.BottomGeoAligned; }
        else { textField.alignment = TextAlignmentOptions.CaplineLeft; }

        textField.text = tex;
    }

    public string GetMarkerText() { return textField.text;}
}
