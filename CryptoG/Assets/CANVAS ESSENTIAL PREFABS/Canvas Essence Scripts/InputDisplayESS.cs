using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class InputDisplayESS : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_InputField inputField;

    [Header("Options")]
    [SerializeField] private string originalValue;
    [SerializeField] private TMP_InputField.ContentType type;

    [Header("Events")]
    [SerializeField] private UnityEvent<string> onValueChanged;

    private void InputFieldValueChanged(string val)
    {
        onValueChanged.Invoke(val);
    }

    private void Start()
    {
        inputField.contentType = type;
        inputField.text = (originalValue).ToString();

        inputField.onValueChanged.AddListener((string value) => {InputFieldValueChanged(value); });
    }

    public string GetValue() {return inputField.text;}
}
