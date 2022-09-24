using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;


public class ToggleDisplayESS : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Toggle toggle;

    [Header("Options")]
    [SerializeField] private bool status;

    [Header("Events")]
    [SerializeField] private UnityEvent<bool> onValueChanged;

    private void InputFieldValueChanged(bool val)
    {
        onValueChanged.Invoke(val);
    }

    private void Start()
    {
        toggle.isOn = status;

        toggle.onValueChanged.AddListener((bool value) => {InputFieldValueChanged(value); });
    }

    public void SetToggle(bool t) {toggle.isOn = t;}
    public bool GetToggle() {return toggle.isOn; }
}
