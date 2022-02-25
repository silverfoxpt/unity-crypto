using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LetterBoxController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI letterText;

    public string GetLetter()
    {
        return letterText.text;
    }

    public void SetLetter(char x)
    {
        letterText.text = x.ToString();
    }
}
