using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayDotSceneTwo : MonoBehaviour
{
    [SerializeField] private GameObject dotPref;
    [SerializeField] private GameObject linePref;
    [SerializeField] private Color lineColor;
    [SerializeField] private GameObject bounder;
    [SerializeField] private GameObject sceneControl;
    [SerializeField] private Vector2 ownOffset = new Vector2(1f, 0f);
    private GameObject dot = null;
    private GameObject connectionLine = null;
    private LineRenderer rendLine = null;

    private void Awake()
    {
        dot = Instantiate(dotPref, 
            bounder.GetComponent<RectangleDrawer>().GetCenterRect() + sceneControl.GetComponent<MainSceneController>().offset + ownOffset, 
            Quaternion.identity, transform);

        connectionLine = Instantiate(linePref, new Vector3(0f, 0f, 0f), Quaternion.identity, transform);
        var rend = connectionLine.GetComponent<LineRenderer>(); rendLine = rend;
        rend.startColor = lineColor;
        rend.endColor = lineColor;
        rend.positionCount = 2;

        gameObject.SetActive(false);
    }

    private void Update()
    {
        //dot.transform.position = bounder.GetComponent<RectangleDrawer>().GetCenterRect();
        Vector2 p1 = dot.transform.position;
        Vector2 p2 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        rendLine.SetPosition(0, p1);
        rendLine.SetPosition(1, p2);
        
    }
}
