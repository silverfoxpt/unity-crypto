using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AdditionalTextFieldsController : MonoBehaviour
{
    [SerializeField] private GameObject outputTextField;
    [SerializeField] private GameObject inputTextField;

    [Header("Debug ONLY")]
    [SerializeField] private string previousField = "";

    public void AddToOutputTextField(char newChar)
    {
        outputTextField.GetComponent<TMP_InputField>().text += newChar;
    }

    public void AddToInputTextField(char newChar)
    {
        string newText = inputTextField.GetComponent<TMP_InputField>().text + newChar;
        inputTextField.GetComponent<TMP_InputField>().SetTextWithoutNotify(newText);
        previousField += newChar;
    }

    public void InputTextFieldChanged(string text)
    {
        int numChanged = text.Length - previousField.Length;
        string changed = "";
        for (int i = text.Length - numChanged; i < text.Length; i++)
        {
            changed += text[i];
        }

        foreach(char c in changed)
        {
            char cur = (c.ToString()).ToUpper()[0];
            if (EnigmaInfo.defaultAlphabet.Contains(cur))
            {
                EnigmaManager.instance.KeyInputClicked(cur, true);
            }
            else
            {
                AddToOutputTextField(cur);
            }
        }
        previousField = text;
    }
}
