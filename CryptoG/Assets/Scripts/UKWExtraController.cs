using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UKWExtraController : MonoBehaviour
{
    [SerializeField] private GameObject rightHalf;
    [SerializeField] private GameObject UKWConnector;
    [SerializeField] private string UKWSetup;
    [SerializeField] private float connectWidth = 0.025f;
    [SerializeField] private Color reflectColor;
    private List<GameObject> rightLetterBoxes;
    private Dictionary<int, int> ukwConnectDict = new Dictionary<int, int>();

    private void Start()
    {
        RefreshDict();
        InitializeReflector();
        DisconnectReflector();
        rightLetterBoxes = rightHalf.GetComponent<HalfRingController>().GetLetterBoxes();

        //test
        //StartCoroutine(TestPlug());
    }

    //debug only
    IEnumerator TestPlug()
    {
        yield return new WaitForSeconds(1);
        ConnectReflector(1);
    }

    private void RefreshDict()
    {
        for (int i = 0; i < EnigmaInfo.defaultLength; i++)
        {
            ukwConnectDict[i] = (int) (UKWSetup[i] - 'A');
        }
    }

    private void InitializeReflector()
    {
        UKWConnector.GetComponent<LineRenderer>().startWidth    = connectWidth;
        UKWConnector.GetComponent<LineRenderer>().endWidth      = connectWidth;
        UKWConnector.GetComponent<LineRenderer>().positionCount = 2;
        UKWConnector.GetComponent<LineRenderer>().endColor      = reflectColor;
        UKWConnector.GetComponent<LineRenderer>().startColor    = reflectColor;
    }

    public void ConnectReflector(int idx)
    {
        UKWConnector.SetActive(true);
        Vector2 point1 = rightLetterBoxes[idx].GetComponent<LetterBoxController>().GetPlugPoint();
        Vector2 point2 = rightLetterBoxes[ukwConnectDict[idx]].GetComponent<LetterBoxController>().GetPlugPoint();

        UKWConnector.GetComponent<LineRenderer>().SetPosition(0, point1);
        UKWConnector.GetComponent<LineRenderer>().SetPosition(1, point2);
    }

    public void DisconnectReflector()
    {
        UKWConnector.SetActive(false);
    }

    public void RefreshWithnewSetup(string setup)
    {
        UKWSetup = setup;
        RefreshDict();
    }
}
