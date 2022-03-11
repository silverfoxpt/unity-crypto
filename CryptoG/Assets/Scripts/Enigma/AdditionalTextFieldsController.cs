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

    public void DeleteOnceFromOutputTextField()
    {
        string curText = outputTextField.GetComponent<TMP_InputField>().text;
        if (curText.Length < 1) {return;}
        curText = curText.Remove(curText.Length-1);
        outputTextField.GetComponent<TMP_InputField>().text = curText;
    }

    public void DeleteEverythingFromOutputTextField() { outputTextField.GetComponent<TMP_InputField>().text = "";}
    public void DeleteEverythingFromInputTextField() { inputTextField.GetComponent<TMP_InputField>().SetTextWithoutNotify("");} //for safety

    public void InputTextFieldChanged(string text)
    {
        //case of increase
        int numChanged = text.Length - previousField.Length;
        if (numChanged > 0)
        {
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
        }
        else
        {
            //Debug.Log("Del");
            numChanged = -numChanged;
            for (int i = 0; i < numChanged; i++) {DeleteOnceFromOutputTextField(); }
        }
        previousField = text;
    }
}
