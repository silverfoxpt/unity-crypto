using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WheelSwitchController : MonoBehaviour
{
    [SerializeField] private GameObject fullRing;
    [SerializeField] private GameObject increaser;
    [SerializeField] private GameObject ringIncreaser;
    [SerializeField] private TextMeshProUGUI curText;

    public void SwitchToNewWheel(bool isUp = true)
    {
        int idx = EnigmaInfo.convertText[curText.text];
        int newIdx;
        if (isUp)
        {
            newIdx = (idx == EnigmaInfo.numWheel-1) ? 0 : idx+1;
        }
        else
        {
            newIdx = (idx == 0) ? EnigmaInfo.numWheel-1 : idx-1;
        }

        string newSetup = EnigmaInfo.wheels[newIdx];
        List<char> newTurnover = EnigmaInfo.turnovers[newIdx];

        //reset stuff
        curText.text = EnigmaInfo.convertTextReverse[newIdx];
        fullRing.GetComponent<FullRingController>().RefreshWithNewSetup(newSetup, newTurnover);
        increaser.GetComponent<IncreaserController>().ResetText();
        ringIncreaser.GetComponent<IncreaserController>().ResetText();
    }

    public void ResetWheel(string wheel)
    {
        int newIdx = EnigmaInfo.convertText[wheel];

        string newSetup = EnigmaInfo.wheels[newIdx];
        List<char> newTurnover = EnigmaInfo.turnovers[newIdx];

        //reset stuff
        curText.text = EnigmaInfo.convertTextReverse[newIdx];
        fullRing.GetComponent<FullRingController>().RefreshWithNewSetup(newSetup, newTurnover);
        increaser.GetComponent<IncreaserController>().ResetText();
        ringIncreaser.GetComponent<IncreaserController>().ResetText();
    }
}
