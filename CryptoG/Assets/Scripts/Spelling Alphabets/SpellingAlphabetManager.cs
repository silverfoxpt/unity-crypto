using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpellingAlphabetManager : MonoBehaviour
{
    [SerializeField] private GameObject inputField;
    [SerializeField] private GameObject outputField;
    [SerializeField] private List<SpellingAlphabetScriptableObject> alphabets;

    [Header("Others")]
    [SerializeField] private int startingIndex = 0;

    private SpellingAlphabetScriptableObject currentAlphabet;
    private List<string> mainAlphabet;
    private List<weirdPair> weirdAlphabet;
    private List<multiPair> multiAlphabet;

    void Start()
    {
        AlphabetChanged(startingIndex);
    }

    public void AlphabetChanged(int idx) 
    { 
        currentAlphabet = alphabets[idx]; 
        mainAlphabet = currentAlphabet.GetMainAlphabet();
        weirdAlphabet = currentAlphabet.GetWeirdAlphabet();
        multiAlphabet = currentAlphabet.GetMultiAlphabet();
    }

    public void TextInputChanged(string text)
    {
        int i = 0;
        string plainText = text.ToLower();
        string finishedText = "";

        while (i < text.Length)
        {
            bool goOn = false;
            //check for multiAlpha FIRSTHAND
            foreach(var mul in multiAlphabet)
            {
                string norm = mul.encodeString.ToLower();
                if (plainText.Length - norm.Length < i) {break;} //if length larger

                string cutout = plainText.Substring(i, norm.Length);
                if (cutout == norm)//if correct
                {
                    goOn = true;
                    finishedText += mul.decodeString; finishedText += ' ';
                    i += norm.Length; break;
                }
            }
            if (i >= plainText.Length) {break;}

            //check for weird char
            foreach (var weird in weirdAlphabet)
            {
                if (plainText[i] == weird.encodeChar)
                {
                    goOn = true;
                    finishedText += weird.decodeString; finishedText += ' ';
                    i++; break;
                }
            }
            if (i >= plainText.Length) {break;}

            //finally, check for normal shit
            for (int j = 0; j < mainAlphabet.Count; j++)
            {
                if (plainText[i] == (char) ('a' + j))
                {
                    goOn = true;
                    finishedText += mainAlphabet[j]; finishedText += ' ';
                    i++; break;
                }
            }

            //if nothing match then...
            if (!goOn)
            {
                finishedText += plainText[i]; 
                if (plainText[i] != '\n' && plainText[i] != '\r') { finishedText += ' ';}
                i++;
            }
        }
        outputField.GetComponent<TMP_InputField>().text = finishedText;
    }
}
