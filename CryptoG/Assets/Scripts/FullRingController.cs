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

    private List<GameObject> leftBoxes, rightBoxes;

    public Dictionary<int, int> connectDict = new Dictionary<int, int>();
    private Dictionary<int, int> reverseConnectDict = new Dictionary<int, int>();
    void Start()
    {
        GetNeededComponents();
        RefreshConnectDict();

        //test
        //StartCoroutine(test());
    }

    //coroutine to test everything cause it all too fragile
    IEnumerator test()
    {
        yield return new WaitForSeconds(1);
        LightUpConnection(0);
        yield return new WaitForSeconds(1);
        LightUpReverseConnection(0);
        yield return new WaitForSeconds(2);
        LightDownEverything();
    }

    private void GetNeededComponents()
    {
        leftBoxes   = leftRing.GetComponent<HalfRingController>().GetLetterBoxes();
        rightBoxes  = rightRing.GetComponent<HalfRingController>().GetLetterBoxes();
    }

    private void RefreshConnectDict()
    {
        //refresh everything - potential bug later.
        connectDict = new Dictionary<int, int>();
        reverseConnectDict = new Dictionary<int, int>();

        for (int i = 0; i < EnigmaInfo.defaultLength; i++)
        {
            connectDict.Add(i, (int) (connectorSetup[i] - 'A'));
            reverseConnectDict.Add((int) (connectorSetup[i] - 'A'), i);
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

    //light controls - probably bugs
    public void LightUpConnection(int idx)
    {
        //get everything - safety purposes
        GameObject leftBox      = leftBoxes[idx];
        GameObject rightBox     = rightBoxes[connectDict[idx]];
        
        //light shit up
        leftBox.GetComponent<LetterBoxController>().LightUpBox();
        rightBox.GetComponent<LetterBoxController>().LightUpBox();
        connectorParent.GetComponent<ConnectorController>().LightUpConnector(idx);
    }

    public void LightUpReverseConnection(int idx)
    {
        //get everything - safety purposes
        GameObject leftBox      = leftBoxes[reverseConnectDict[idx]];
        GameObject rightBox     = rightBoxes[idx];
        
        //light shit up
        leftBox.GetComponent<LetterBoxController>().LightUpBoxReverse();
        rightBox.GetComponent<LetterBoxController>().LightUpBoxReverse();
        connectorParent.GetComponent<ConnectorController>().LightUpReverseConnector(idx);
    }

    public void LightDownEverything()
    {
        leftRing.GetComponent<HalfRingController>().LightDownAllBox();
        rightRing.GetComponent<HalfRingController>().LightDownAllBox();
        connectorParent.GetComponent<ConnectorController>().AllLightDown();
    }
}
