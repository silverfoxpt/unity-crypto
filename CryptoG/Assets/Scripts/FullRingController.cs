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
    [SerializeField] private string connectorSetup;
    [SerializeField] private List<char> notches;

    private List<GameObject> leftBoxes, rightBoxes;

    public Dictionary<int, int> connectDict = new Dictionary<int, int>();
    private Dictionary<int, int> reverseConnectDict = new Dictionary<int, int>();

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
    public string GetSetup() {return connectorSetup; }
    #endregion

    public void PushForwardAll()
    {
        leftRing.GetComponent<HalfRingController>().PushForwardOnce();
        rightRing.GetComponent<HalfRingController>().PushForwardOnce();
        turnoverParent.GetComponent<TurnoverController>().PushNotchesForwardOnce();

        char lastChar = connectorSetup[EnigmaInfo.defaultLength-1];
        connectorSetup = connectorSetup.Remove(EnigmaInfo.defaultLength-1);
        connectorSetup = connectorSetup.Insert(0, lastChar.ToString());
        RefreshConnectDict();

        StartCoroutine(connectorParent.GetComponent<ConnectorController>().RedrawConnectors());
    }

    public void PushBackwardAll()
    {
        leftRing.GetComponent<HalfRingController>().PushBackwardOnce();
        rightRing.GetComponent<HalfRingController>().PushBackwardOnce();
        turnoverParent.GetComponent<TurnoverController>().PushNotchesBackwardOnce();

        char lastChar = connectorSetup[0];
        connectorSetup = connectorSetup.Remove(0, 1);
        connectorSetup += lastChar;
        RefreshConnectDict();

        StartCoroutine(connectorParent.GetComponent<ConnectorController>().RedrawConnectors());
    }

    #region refreshControls
    public void RefreshWithNewSetup(string setup)
    {
        connectorSetup = setup;
        RefreshConnectDict();
        StartCoroutine(connectorParent.GetComponent<ConnectorController>().RedrawConnectors());
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
