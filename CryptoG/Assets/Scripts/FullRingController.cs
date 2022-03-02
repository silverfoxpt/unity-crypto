using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullRingController : MonoBehaviour
{
    [SerializeField] private GameObject leftRing;
    [SerializeField] private GameObject rightRing;
    [SerializeField] private GameObject connectorParent;

    [Header("Setup")]
    [SerializeField] private string connectorSetup;
    [SerializeField] private List<char> notches;

    public Dictionary<int, int> connectDict = new Dictionary<int, int>();
    void Start()
    {
        RefreshConnectDict();

        //test
        //PushForwardAll();
    }

    private void RefreshConnectDict()
    {
        for (int i = 0; i < EnigmaInfo.defaultLength; i++)
        {
            connectDict.Add(i, (int) (connectorSetup[i] - 'A'));
        }
    }

    public GameObject GetLeftRing()     {return leftRing;}
    public GameObject GetRightRing()    {return rightRing;}
    public List<char> GetTurnover() {return notches;}

    public void PushForwardAll()
    {
        leftRing.GetComponent<HalfRingController>().PushForwardOnce();
        rightRing.GetComponent<HalfRingController>().PushForwardOnce();

        //reform dict
        Dictionary<int, int> tmpDict = new Dictionary<int, int>();        
        for (int i = 0; i < EnigmaInfo.defaultLength; i++)
        {
            int key = i;
            int val = connectDict[i];
            if (key == EnigmaInfo.defaultLength - 1) {key = 0;} else {key++;}
            if (val == EnigmaInfo.defaultLength - 1) {val = 0;} else {val++;}

            tmpDict[key] = val;
        }
        connectDict = tmpDict;
        StartCoroutine(connectorParent.GetComponent<ConnectorController>().RedrawConnectors());
    }

    public void RefreshWithNewSetup(string setup)
    {
        RefreshConnectDict();
        StartCoroutine(connectorParent.GetComponent<ConnectorController>().RedrawConnectors());
    }
}
