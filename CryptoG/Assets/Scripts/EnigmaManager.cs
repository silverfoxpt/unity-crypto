using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class EnigmaManager : MonoBehaviour
{
    [Header("Rings")]
    [SerializeField] private GameObject plugboard;
    [SerializeField] private GameObject ring1;
    [SerializeField] private GameObject ring2;
    [SerializeField] private GameObject ring3;
    [SerializeField] private GameObject ukw;
    [SerializeField] private GameObject ekw;

    [Header("Controls")]
    [SerializeField] private GameObject ukwChanger;
    [SerializeField] private List<GameObject> wheelChangers;

    [Header("Others")]
    [SerializeField] private GameObject keyInputManager;
    [SerializeField] private List<Button> controlButtons;

    public static EnigmaManager instance;

    private List<int> notch1, notch2, notch3;
    private bool fresh = true;
    private bool doubleStep = false;

    private void Awake() 
    {
        instance = this;    
    }
    
    public void RestartEnigmaMachine()
    {
        fresh = true; doubleStep = false;
        PowerUpControls();
        //reset 3 rotors
        int cnt = 0;
        foreach(GameObject ch in wheelChangers)
        {
            ch.GetComponent<WheelSwitchController>().ResetWheel(EnigmaInfo.convertTextReverse[cnt]);
            cnt++;
        }

        //reset ukw
        ukwChanger.GetComponent<ReflectorUpdateController>().ResetReflector();

        //reset plugboard
        plugboard.GetComponent<PlugBoardExtraController>().ResetPlugboard();

        //reset keyboard
        keyInputManager.GetComponent<KeyInputManager>().LightDownKey();
        keyInputManager.GetComponent<KeyInputManager>().LightDownPlugline();
    }

    public void KeyInputClicked(char charClicked)
    {
        PowerDownControls();
        Debug.Log("Clicked : " + charClicked);
        PushWheels();
        char curChar = charClicked;

        //connection: keyboard -> plugboard
        keyInputManager.GetComponent<KeyInputManager>().LightUpPlugline(charClicked);

        //connection: plugboard
        plugboard.GetComponent<FullRingController>().LightUpConnection((int) (curChar - 'A'));
        curChar = (char) (plugboard.GetComponent<FullRingController>().connectDict[(int) (curChar - 'A')] + 'A');

        //connection: ekw
        ekw.GetComponent<FullRingController>().LightUpConnection((int) (curChar - 'A'));
        curChar = (char) (ekw.GetComponent<FullRingController>().connectDict[(int) (curChar - 'A')] + 'A');

        //connection: ring 1
        ring1.GetComponent<FullRingController>().LightUpConnection((int) (curChar - 'A'));
        curChar = (char) (ring1.GetComponent<FullRingController>().connectDict[(int) (curChar - 'A')] + 'A');

        //connection: ring2
        ring2.GetComponent<FullRingController>().LightUpConnection((int) (curChar - 'A'));
        curChar = (char) (ring2.GetComponent<FullRingController>().connectDict[(int) (curChar - 'A')] + 'A');

        //connection: ring 3
        ring3.GetComponent<FullRingController>().LightUpConnection((int) (curChar - 'A'));
        curChar = (char) (ring3.GetComponent<FullRingController>().connectDict[(int) (curChar - 'A')] + 'A');

        //connection UKW
        ukw.GetComponent<UKWExtraController>().ConnectReflector((int) (curChar - 'A'));
        ukw.GetComponent<FullRingController>().LightUpConnection((int) (curChar - 'A'));
        curChar = (char) (ukw.GetComponent<UKWExtraController>().ukwConnectDict[(int) (curChar - 'A')] + 'A');
        ukw.GetComponent<FullRingController>().LightUpReverseConnection((int) (curChar - 'A'));

        //connection: ring 3 reverse
        ring3.GetComponent<FullRingController>().LightUpReverseConnection((int) (curChar - 'A'));
        curChar = (char) (ring3.GetComponent<FullRingController>().reverseConnectDict[(int) (curChar - 'A')] + 'A');

        //connection: ring 2 reverse
        ring2.GetComponent<FullRingController>().LightUpReverseConnection((int) (curChar - 'A'));
        curChar = (char) (ring2.GetComponent<FullRingController>().reverseConnectDict[(int) (curChar - 'A')] + 'A');

        //connection: ring 1 reverse
        ring1.GetComponent<FullRingController>().LightUpReverseConnection((int) (curChar - 'A'));
        curChar = (char) (ring1.GetComponent<FullRingController>().reverseConnectDict[(int) (curChar - 'A')] + 'A');

        //connection: ekw reverse
        ekw.GetComponent<FullRingController>().LightUpReverseConnection((int) (curChar - 'A'));
        curChar = (char) (ekw.GetComponent<FullRingController>().reverseConnectDict[(int) (curChar - 'A')] + 'A');

        //connection: plugboard reverse
        plugboard.GetComponent<FullRingController>().LightUpReverseConnection((int) (curChar - 'A'));
        curChar = (char) (plugboard.GetComponent<FullRingController>().reverseConnectDict[(int) (curChar - 'A')] + 'A');

        //output keyboard pending

    }

    private void PushWheels()
    {
        if (fresh)
        {
            UpdateNewNotch();
            fresh = false;
        }

        bool jump1 = false;;
        //update first ring
        ring1.GetComponent<FullRingController>().PushBackwardAll(false);
        for (int i = 0; i < notch1.Count; i++)
        {
            notch1[i]--; if (notch1[i] < 0) { Debug.Log("hi"); notch1[i] = EnigmaInfo.defaultLength-1; jump1 = true;}
        }

        //update second ring
        if (jump1)
        {
            int maxNotch2 = notch2.Max();
            //double step immidiately
            if (maxNotch2 == EnigmaInfo.defaultLength-1)
            {
                //jump2 = true; //not needed anyway
                ring2.GetComponent<FullRingController>().PushBackwardAll(false);
                for (int i = 0; i < notch2.Count; i++)
                {
                    notch2[i]--; if (notch2[i] < 0) {notch2[i] = EnigmaInfo.defaultLength-1; }
                }

                ring3.GetComponent<FullRingController>().PushBackwardAll(false);
                for (int i = 0; i < notch3.Count; i++)
                {
                    notch3[i]--; if (notch3[i] < 0) {notch3[i] = EnigmaInfo.defaultLength-1; }
                }
                return; //stop any shit
            }
            //double step in next step
            else if (maxNotch2 == EnigmaInfo.defaultLength-2)
            {
                doubleStep = true;
                ring2.GetComponent<FullRingController>().PushBackwardAll(false);
                for (int i = 0; i < notch2.Count; i++)
                {
                    notch2[i]--; if (notch2[i] < 0) {notch2[i] = EnigmaInfo.defaultLength-1; }
                }
            }
        }

        //double step ring 3
        if (doubleStep)
        {
            doubleStep = false;
            ring3.GetComponent<FullRingController>().PushBackwardAll(false);
            for (int i = 0; i < notch3.Count; i++)
            {
                notch3[i]++; if (notch3[i] < 0) {notch3[i] = EnigmaInfo.defaultLength-1; }
            }
        }
    }

    private void UpdateNewNotch()
    {
        List<char> tmp = EnigmaInfo.turnovers[wheelChangers[0].GetComponent<WheelSwitchController>().GetWheelIndex()];
        notch1 = new List<int>();
        foreach (char c in tmp) { notch1.Add((int)(c - 'A')); }

        tmp = EnigmaInfo.turnovers[wheelChangers[1].GetComponent<WheelSwitchController>().GetWheelIndex()];
        notch2 = new List<int>();
        foreach (char c in tmp) { notch2.Add((int)(c - 'A')); }

        tmp = EnigmaInfo.turnovers[wheelChangers[2].GetComponent<WheelSwitchController>().GetWheelIndex()];
        notch3 = new List<int>();
        foreach (char c in tmp) { notch3.Add((int)(c - 'A')); }
    }

    public void PowerDownControls()
    {
        foreach (var but in controlButtons)
        {
            but.enabled = false;
        }
        plugboard.GetComponent<PlugBoardExtraController>().DisablePlugboard();
    }

    public void PowerUpControls()
    {
        foreach (var but in controlButtons)
        {
            but.enabled = true;
        }
        plugboard.GetComponent<PlugBoardExtraController>().EnablePlugboard();
    }
}
