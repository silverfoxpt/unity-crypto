using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class SliderInputDisplayESS : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_InputField textInput;

    [Header("Options")]
    [SerializeField] private Vector2 range;
    [SerializeField] private float originalValue;
    [SerializeField] private int truncateDecimals;
    [SerializeField] private bool wholeNumbers = false;

    [Header("Events")]
    [SerializeField] private UnityEvent<float> onValueChanged;

    private void SliderValueChanged(float val)
    {
        onValueChanged.Invoke(val);
        textInput.text = System.Math.Round(slider.value, truncateDecimals).ToString();
    }

    private void InputFieldValueChanged(string val)
    {
        float value = float.Parse(val);

        onValueChanged.Invoke(value);
        slider.value = value;
    }

    void Start()
    {
        slider.maxValue = range.y;
        slider.minValue = range.x;

        slider.value = originalValue;
        slider.wholeNumbers = wholeNumbers;
        
        slider.onValueChanged.AddListener((float value) => {SliderValueChanged(value); });
    
        textInput.contentType = (wholeNumbers) ? TMP_InputField.ContentType.IntegerNumber : TMP_InputField.ContentType.DecimalNumber;
        textInput.text = (originalValue).ToString();

        textInput.onValueChanged.AddListener((string value) => {InputFieldValueChanged(value); });
    }

    public void SetValue(float val) {slider.value = val; textInput.text = slider.value.ToString();}

    public float GetValue() {return slider.value; }
}
