using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullRingController : MonoBehaviour
{
    [SerializeField] private GameObject leftRing;
    [SerializeField] private GameObject rightRing;
    [SerializeField] private string connectorSetup;

    public Dictionary<int, int> connectDict = new Dictionary<int, int>();
    void Start()
    {
        InitializeConnectionDict();
    }

    private void InitializeConnectionDict()
    {
        for (int i = 0; i < EnigmaInfo.defaultLength; i++)
        {
            connectDict.Add(i, (int) (connectorSetup[i] - 'A'));
        }
    }

    public GameObject GetLeftRing()     {return leftRing;}
    public GameObject GetRightRing()    {return rightRing;}
}
