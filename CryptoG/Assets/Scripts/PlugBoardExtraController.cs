using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlugBoardExtraController : MonoBehaviour
{
    [SerializeField] private GameObject leftRing, rightRing;
    [SerializeField] private GameObject fullRing;
    [SerializeField] private int maxPair = 10;

    private GameObject leftObject = null, rightObject = null;
    private List<GameObject> leftBoxes, rightBoxes;
    private string setupString;
    private int currentPair = 0;
    void Start()
    {
        GetNeededComponents();
    }

    private void GetNeededComponents()
    {
        leftBoxes = leftRing.GetComponent<HalfRingController>().GetLetterBoxes();
        rightBoxes = rightRing.GetComponent<HalfRingController>().GetLetterBoxes();
        setupString = fullRing.GetComponent<FullRingController>().GetSetup();
        //Debug.Log("Original : " + setupString);
    }

    private string SwapChars(string str, int index1, int index2) //steal lol
    {
        char[] strChar = str.ToCharArray();
        strChar[index1] = str[index2];
        strChar[index2] = str[index1];
        
        return new string(strChar);
    }

    public void BoxClicked(GameObject box)
    {
        bool isLeft = leftBoxes.Contains(box);
        if (isLeft)     
        { 
            leftObject = box;

            //if plugged
            int idx = leftBoxes.IndexOf(box);
            if (setupString[idx] != (char) ('A' + idx))
            {
                //plugged -> unplug
                int otherIdx = setupString[idx] - 'A';
                setupString = SwapChars(setupString, idx, otherIdx);
                leftObject = null; rightObject = null;
                currentPair--;
            }
            else
            {
                if (rightObject && (leftObject != rightObject)) //if rightObject marked already
                {
                    if (currentPair == maxPair) {return;}
                    else {currentPair++; }

                    int otherIdx = rightBoxes.IndexOf(rightObject);
                    setupString = SwapChars(setupString, idx, otherIdx);
                    leftObject = null; rightObject = null;
                }
            }
        }
        else            
        { 
            rightObject = box;
            //if plugged
            int idx = rightBoxes.IndexOf(box);
            if (setupString[idx] != (char) ('A' + idx))
            {
                //plugged -> unplug
                int otherIdx = setupString[idx] - 'A';
                setupString = SwapChars(setupString, idx, otherIdx);
                leftObject = null; rightObject = null;
                currentPair--;
            }
            else
            {
                if (leftObject && (leftObject != rightObject)) //if rightObject marked already
                {
                    if (currentPair == maxPair) {return;}
                    else {currentPair++; }
                    
                    int otherIdx = leftBoxes.IndexOf(leftObject);
                    setupString = SwapChars(setupString, idx, otherIdx);
                    leftObject = null; rightObject = null;
                }
            }
        }
        fullRing.GetComponent<FullRingController>().RefreshWithNewSetup(setupString);
    }
}
