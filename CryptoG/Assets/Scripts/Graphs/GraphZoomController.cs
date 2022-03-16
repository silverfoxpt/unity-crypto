using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphZoomController : MonoBehaviour
{
    [SerializeField] private GameObject zoomSliderPar;

    [Header("Cam Vars")]
    [SerializeField] private float startCamVal = 6f;
    [SerializeField] private float maxChange = 2f;

    private float camValueBeforeCalibration;
    private float originalCamValue = 6f;

    private GraphDrawer graphDrawer;
    private GraphInitializer graphInitializer;

    private void Awake()
    {
        graphDrawer = FindObjectOfType<GraphDrawer>();
        camValueBeforeCalibration = originalCamValue = startCamVal;
    }

    private void Start()
    {
        graphInitializer = FindObjectOfType<GraphInitializer>();

        zoomSliderPar.GetComponent<SliderController>().SetValue(startCamVal);

        graphInitializer.SetSideLength((startCamVal+maxChange)/startCamVal*graphInitializer.GetSideLength());
        graphInitializer.ResetBaseGraph();
    }

    public void GraphZoom(float zoomVal)
    {
        Camera.main.orthographicSize = zoomVal + maxChange * Mathf.Floor((originalCamValue - zoomVal)/maxChange);

        if (Mathf.Abs(zoomVal - camValueBeforeCalibration) > maxChange)
        {
            Camera.main.orthographicSize = originalCamValue; //set back to same

            //too far, calibrate
            if (zoomVal - camValueBeforeCalibration >= maxChange)
            {
                graphInitializer.ResetMarker(2);
                graphDrawer.portionServing *= 2;
            }
            else if (camValueBeforeCalibration - zoomVal >= maxChange) //too close, calibrate
            {
                graphInitializer.ResetMarker(0.5f);
                graphDrawer.portionServing /= 2;
            }

            camValueBeforeCalibration = zoomVal; //set to new
            graphDrawer.RefreshAllGraph();
        }
    }
}
