using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectorController : MonoBehaviour
{
    [SerializeField] private GameObject fullRing;
    private FullRingController fullRingController;
    private HalfRingController halfControlLeft, halfControlRight;
    private List<GameObject> leftLetterBoxes, rightLetterBoxes;
    private List<GameObject> connectors = new List<GameObject>();
    void Start()
    {
        GetNeededComponents();
        foreach (Transform connector in transform)
        {
            connectors.Add(connector.gameObject);
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
}
