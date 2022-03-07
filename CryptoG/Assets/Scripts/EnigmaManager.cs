using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnigmaManager : MonoBehaviour
{
    [Header("Rings")]
    [SerializeField] private GameObject plugboard;
    [SerializeField] private GameObject ring1;
    [SerializeField] private GameObject ring2;
    [SerializeField] private GameObject ring3;
    [SerializeField] private GameObject ukw;

    [Header("Controls")]
    [SerializeField] private GameObject ukwChanger;
    [SerializeField] private List<GameObject> wheelChangers;
    
    public void RestartEnigmaMachine()
    {
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
    }
}
