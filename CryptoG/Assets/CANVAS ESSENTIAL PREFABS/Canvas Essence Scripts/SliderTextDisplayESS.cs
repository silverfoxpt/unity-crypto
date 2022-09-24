using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class SliderTextDisplayESS : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider slider;
    [SerializeField] private TextDisplayerESS textDisplay;

    [Header("Options")]
    [SerializeField] private Vector2 range;
    [SerializeField] private float originalValue;
    [SerializeField] private int truncateDecimals;
    [SerializeField] private bool wholeNumbers = false;

    [Header("Events")]
    [SerializeField] private UnityEvent<float> onSliderValueChanged;

    private void SliderValueChanged(float val)
    {
        onSliderValueChanged.Invoke(val);
        textDisplay.SetText(System.Math.Round(slider.value, truncateDecimals).ToString());
    }

    void Start()
    {
        slider.maxValue = range.y;
        slider.minValue = range.x;

        slider.value = originalValue;
        slider.wholeNumbers = wholeNumbers;
        
        slider.onValueChanged.AddListener((float value) => {SliderValueChanged(value); });

        textDisplay.SetText((originalValue).ToString());
    }

    public void SetValue(float val) {slider.value = val;}

    public float GetValue() {return slider.value; }
}
