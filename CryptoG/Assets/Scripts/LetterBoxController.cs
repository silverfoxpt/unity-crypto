using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LetterBoxController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI letterText;

    public char GetLetter()
    {
        return letterText.text[0];
    }

    public void SetLetter(char x)
    {
        letterText.text = x.ToString();
    }
}
