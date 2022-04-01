using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class SliderController : MonoBehaviour
{
    [Header("Slider value settings")]
    [SerializeField] private float maxValue;
    [SerializeField] private float minValue;
    [SerializeField] private float originalValue;
    [SerializeField] private int truncateDecimals = 3;
    [SerializeField] private float multiplier = 0.1f;

    [Header("References")]
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI valueText;

    [Header("Events")]
    [SerializeField] private UnityEvent<float> onSliderValueChanged;

    void Start()
    {
        slider.maxValue = maxValue * multiplier;
        slider.minValue = minValue * multiplier;
        slider.value = originalValue * multiplier;
        slider.onValueChanged.AddListener((float value) => {SliderValueChanged(value); });

        valueText.text = (originalValue*multiplier).ToString();
    }

    private void SliderValueChanged(float val)
    {
        onSliderValueChanged.Invoke(val * (1/multiplier));
        valueText.text = System.Math.Round(slider.value, truncateDecimals).ToString();
    }

    public void SetValue(float val) {slider.value = val * multiplier;}

    public float GetValue() {return slider.value; }
}
