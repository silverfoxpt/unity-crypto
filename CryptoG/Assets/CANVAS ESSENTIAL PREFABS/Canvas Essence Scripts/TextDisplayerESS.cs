using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextDisplayerESS : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI textBox;

    [Header("Options")]
    [SerializeField] private string text;
    [SerializeField] private float fontSize;
    [SerializeField] private bool autoDisplayTextOnStart = true;

    private void Start()
    {
        if (autoDisplayTextOnStart) { SetText(text); }
        SetFontSize(fontSize);
    }

    public void SetText(string tex)
    {
        textBox.text = tex;
    }

    public void SetFontSize(float f)
    {
        textBox.fontSize = f;
    }

    public string GetText() {return textBox.text; }
}
