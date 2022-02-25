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
    }

    private void InitializeColumn()
    {
        for (int i = 0; i < EnigmaInfo.defaultLength; i++)
        {
            letterBoxes[i].GetComponent<LetterBoxController>().SetLetter(EnigmaInfo.defaultAlphabet[i]);
        }
    }
}
