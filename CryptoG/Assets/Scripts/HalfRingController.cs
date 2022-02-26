using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class HalfRingController : MonoBehaviour
{
    [SerializeField] private GameObject letterContainer;
    private List<GameObject> letterBoxes = new List<GameObject>();
    void Start()
    {
        foreach(Transform box in letterContainer.transform)
        {
            letterBoxes.Add(box.gameObject);
        }
        InitializeColumn();

        //test
        PushBackwardTimes(3);
    }

    private void InitializeColumn()
    {
        for (int i = 0; i < EnigmaInfo.defaultLength; i++)
        {
            letterBoxes[i].GetComponent<LetterBoxController>().SetLetter(EnigmaInfo.defaultAlphabet[i]);
        }
    }

    public void PushForwardOnce()
    {
        char endi = letterBoxes[EnigmaInfo.defaultLength-1].GetComponent<LetterBoxController>().GetLetter();
        for (int i = EnigmaInfo.defaultLength-1; i >= 1; i--)
        {
            letterBoxes[i].GetComponent<LetterBoxController>().SetLetter(
                letterBoxes[i-1].GetComponent<LetterBoxController>().GetLetter()
            );
        }
        letterBoxes[0].GetComponent<LetterBoxController>().SetLetter(endi);
    }

    public void PushBackwardOnce()
    {
        char firsti = letterBoxes[0].GetComponent<LetterBoxController>().GetLetter();
        for (int i = 0; i < EnigmaInfo.defaultLength-1; i++)
        {
            letterBoxes[i].GetComponent<LetterBoxController>().SetLetter(
                letterBoxes[i+1].GetComponent<LetterBoxController>().GetLetter()
            );
        }
        letterBoxes[EnigmaInfo.defaultLength-1].GetComponent<LetterBoxController>().SetLetter(firsti);
    }

    public void PushForwardTimes(int numTimes)
    {
        for (int i = 0; i < numTimes; i++) { PushForwardOnce(); }
    }

    public void PushBackwardTimes(int numTimes)
    {
        for (int i = 0; i < numTimes; i++) { PushBackwardOnce(); }
    }
}
