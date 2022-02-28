using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectorController : MonoBehaviour
{
    [SerializeField] private GameObject fullRing;
    [SerializeField] private float lineWidth = 0.0025f;
    [SerializeField] private Color lightUpColor;
    [SerializeField] private Color lightUpReverseColor;
    private Color normalColor;
    private FullRingController fullRingController;
    private HalfRingController halfControlLeft, halfControlRight;
    private List<GameObject> leftLetterBoxes, rightLetterBoxes;
    private List<GameObject> connectors = new List<GameObject>();
    private Dictionary<int, int> connectDict, reverseConnectDict;

    private void Start()
    {
        GetNeededComponents();
        InitializeVars();
        StartCoroutine(RedrawConnectors());

        //test
        //LightUpConnector(1);
        //LightUpReverseConnector(4);
    }

    private void InitializeVars()
    {
        foreach (Transform connector in transform)
        {
            connectors.Add(connector.gameObject);
        }
        normalColor = leftLetterBoxes[0].GetComponent<LineRenderer>().startColor;
    }

    public IEnumerator RedrawConnectors()
    {
        yield return new WaitForEndOfFrame();
        RefreshConnectDict();
        
        for (int i = 0; i < EnigmaInfo.defaultLength; i++)
        {
            //left            
            Vector2 needed1 = leftLetterBoxes[i].GetComponent<LetterBoxController>().GetPlugPoint();

            //right
            Vector2 needed2 = rightLetterBoxes[connectDict[i]].GetComponent<LetterBoxController>().GetPlugPoint();

            //draw connector
            LineRenderer tmpLine = connectors[i].GetComponent<LineRenderer>();
            tmpLine.positionCount = 2;
            tmpLine.SetPosition(0, needed1);
            tmpLine.SetPosition(1, needed2);
            tmpLine.startWidth   = lineWidth;
            tmpLine.endWidth     = lineWidth;
        }
    }

    private void GetNeededComponents()
    {
        fullRingController = fullRing.GetComponent<FullRingController>();

        halfControlLeft = fullRingController.GetLeftRing().GetComponent<HalfRingController>();
        halfControlRight = fullRingController.GetRightRing().GetComponent<HalfRingController>();

        leftLetterBoxes = halfControlLeft.GetLetterBoxes();
        rightLetterBoxes = halfControlRight.GetLetterBoxes();
    }

    private void RefreshConnectDict() 
    { 
        connectDict = fullRingController.connectDict; 
        reverseConnectDict = new Dictionary<int, int>();

        for (int i = 0; i < EnigmaInfo.defaultLength; i++)
        {
            reverseConnectDict[connectDict[i]] = i;
        }
    }

    public void LightUpConnector(int idx)
    {
        RefreshConnectDict();
        connectors[idx].GetComponent<LineRenderer>().startColor     = lightUpColor;
        connectors[idx].GetComponent<LineRenderer>().endColor       = lightUpColor;
    }

    public void LightUpReverseConnector(int idx)
    {
        RefreshConnectDict();
        connectors[reverseConnectDict[idx]].GetComponent<LineRenderer>().startColor     = lightUpReverseColor;
        connectors[reverseConnectDict[idx]].GetComponent<LineRenderer>().endColor       = lightUpReverseColor;
    }
}
