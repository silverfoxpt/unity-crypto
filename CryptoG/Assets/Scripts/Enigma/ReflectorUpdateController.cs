using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReflectorUpdateController : MonoBehaviour
{
    [SerializeField] private GameObject reflectorFullRing;
    [SerializeField] private TextMeshProUGUI curText;
    
    public void SwitchReflector()
    {
        char cur = curText.text[0];
        curText.text = (cur == 'B') ? "C" : "B";

        reflectorFullRing.GetComponent<UKWExtraController>().RefreshWithnewSetup((cur == 'B') ? EnigmaInfo.reflectorC : EnigmaInfo.reflectorB);
    }

    public void ResetReflector()
    {
        char cur = 'B';
        curText.text = cur.ToString();

        reflectorFullRing.GetComponent<UKWExtraController>().RefreshWithnewSetup(EnigmaInfo.reflectorB);
    }

}
