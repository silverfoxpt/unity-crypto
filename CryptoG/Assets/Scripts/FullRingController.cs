using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullRingController : MonoBehaviour
{
    [SerializeField] private GameObject leftRing;
    [SerializeField] private GameObject rightRing;
    [SerializeField] private GameObject connectorParent;
    [SerializeField] private GameObject turnoverParent;

    [Header("Setup")]
    [SerializeField] private string connectSetup;
    [SerializeField] private List<char> notches;

    private List<GameObject> leftBoxes, rightBoxes;

    public Dictionary<int, int> connectDict = new Dictionary<int, int>();
    public Dictionary<int, int> reverseConnectDict = new Dictionary<int, int>();

    #region setupControls
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
    #endregion

    #region getters
    public GameObject GetLeftRing()     {return leftRing;}
    public GameObject GetRightRing()    {return rightRing;}
    public List<char> GetTurnover() {return notches;}
    public string GetSetup() {return connectSetup; }
    #endregion

    #region pusher
    public void PushForwardAll(bool ring)
    {
        if (!ring) 
        { 
            leftRing.GetComponent<HalfRingController>().PushForwardOnce();
            rightRing.GetComponent<HalfRingController>().PushForwardOnce();
            turnoverParent.GetComponent<TurnoverController>().PushNotchesForwardOnce(); 
        }

        Dictionary<int, int> tmpDict = new Dictionary<int, int>();
        for (int i = 0; i < EnigmaInfo.defaultLength; i++)
        {
            int otherIdx = connectDict[i]+1;
            int curIdx = i+1;
            if (otherIdx >= 26) {otherIdx = 0;}
            if (curIdx >= 26) {curIdx = 0;}
            tmpDict[curIdx] = otherIdx;
        }        
        connectDict = tmpDict;
        ReverseConstructConnectSetup();
        RefreshConnectDict();

        StartCoroutine(connectorParent.GetComponent<ConnectorController>().RedrawConnectors());
    }

    public void PushBackwardAll(bool ring)
    {
        if (!ring) 
        { 
            leftRing.GetComponent<HalfRingController>().PushBackwardOnce();
            rightRing.GetComponent<HalfRingController>().PushBackwardOnce();
            turnoverParent.GetComponent<TurnoverController>().PushNotchesBackwardOnce(); 
        }

        Dictionary<int, int> tmpDict = new Dictionary<int, int>();
        for (int i = 0; i < EnigmaInfo.defaultLength; i++)
        {
            int otherIdx = connectDict[i]-1;
            int curIdx = i-1;
            if (otherIdx < 0) {otherIdx = 25;}
            if (curIdx < 0) {curIdx = 25;}
            tmpDict[curIdx] = otherIdx;
        }        
        connectDict = tmpDict;
        ReverseConstructConnectSetup();
        RefreshConnectDict();

        StartCoroutine(connectorParent.GetComponent<ConnectorController>().RedrawConnectors());
    }
    #endregion

    #region refreshControls
    public void RefreshWithNewSetup(string setup, List<char> newNotches = null)
    {
        connectSetup = setup;
        RefreshConnectDict();
        LightDownEverything();

        if (newNotches != null && turnoverParent != null) //in case of EKW
        {
            notches = newNotches;
            StartCoroutine(turnoverParent.GetComponent<TurnoverController>().RefreshNotches());
        }
        StartCoroutine(connectorParent.GetComponent<ConnectorController>().RedrawConnectors());
    }
    private void RefreshConnectDict()
    {
        connectDict = new Dictionary<int, int>();
        reverseConnectDict = new Dictionary<int, int>();

        for (int i = 0; i < EnigmaInfo.defaultLength; i++)
        {
            connectDict.Add(i, (int) (connectSetup[i] - 'A'));
            reverseConnectDict.Add((int) (connectSetup[i] - 'A'), i);
        }
    }

    private void ReverseConstructConnectSetup()
    {
        string newSetup = "";
        for (int i = 0; i < EnigmaInfo.defaultLength; i++)
        {
            newSetup += (char) (connectDict[i] + 'A');
        }
        connectSetup = newSetup;
    }
    #endregion    

    //light controls - probably bugs
    #region lightControls
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
    #endregion
}
