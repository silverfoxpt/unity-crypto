using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyInputManager : MonoBehaviour
{
    [Header("Connection & Bridge")]
    [SerializeField] private Image plugInput;
    [SerializeField] private Color plugLineColor;
    [SerializeField] private float lineWidth = 0.025f;
    [SerializeField] private float bridgeLength = 0.1f;
    [SerializeField] private GameObject plugboardHalfRing;
    [SerializeField] private GameObject connection;
    [SerializeField] private GameObject bridge;

    [Header("Others")]
    [SerializeField] private List<GameObject> keys;
    private List<GameObject> plugBoxes;


    void Start()
    {
        plugBoxes = plugboardHalfRing.GetComponent<HalfRingController>().GetLetterBoxes();

        //test
        //StartCoroutine(test());
    }

    IEnumerator test()
    {
        yield return new WaitForSeconds(1);
        LightUpPlugline('C');
    }

    public void LightUpPlugline(char keyClicked) 
    {
        connection.GetComponent<LineRenderer>().enabled = true;
        bridge.GetComponent<LineRenderer>().enabled = true;
        
        //bridge
        LineRenderer tmp = bridge.GetComponent<LineRenderer>();
        GameObject neededBox = plugBoxes[(int) (keyClicked - 'A')];
        Vector3[] boxCorners = new Vector3[4]; neededBox.GetComponent<RectTransform>().GetWorldCorners(boxCorners);

        Vector2 neededPoint1;
        neededPoint1.x = boxCorners[0].x;
        neededPoint1.y = (boxCorners[0].y + boxCorners[1].y)/2;

        Vector2 neededPoint2 = new Vector2(neededPoint1.x - bridgeLength, neededPoint1.y);
        tmp.positionCount = 2;
        tmp.endWidth    = lineWidth;
        tmp.startWidth  = lineWidth;
        tmp.SetPosition(0, neededPoint1);
        tmp.SetPosition(1, neededPoint2);

        //connection
        tmp = connection.GetComponent<LineRenderer>();
        tmp.positionCount = 2;

        tmp.endWidth    = lineWidth;
        tmp.startWidth  = lineWidth;
        tmp.endColor    = plugLineColor;
        tmp.startColor  = plugLineColor;
        
        tmp.SetPosition(0, plugInput.GetComponent<RectTransform>().TransformPoint(plugInput.GetComponent<RectTransform>().rect.center));
        tmp.SetPosition(1, neededPoint2);
    }

    public void LightDownPlugline()
    {
        connection.GetComponent<LineRenderer>().enabled = false;
        bridge.GetComponent<LineRenderer>().enabled = false;
    }

    public void LightDownKey()
    {
        foreach(GameObject key in keys)
        {
            key.GetComponent<KeyboardInputController>().LightDown();
        }
    }

    public void LightUpSingleKey(char curChar)
    {
        foreach (GameObject key in keys)
        {
            if (key.GetComponent<KeyboardInputController>().GetKeyChar() == curChar)
            {
                key.GetComponent<KeyboardInputController>().LightUp();
                return;
            }
        }
    }
}
