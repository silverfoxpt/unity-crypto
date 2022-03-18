using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphDisplayerUIController : MonoBehaviour
{
    [SerializeField] private GameObject graphDisplayPrefab;
    [SerializeField] private GameObject contentLayout;
    private List<GameObject> graphDisplays = new List<GameObject>();
    private float graphDisplayHeight = 0f;
    private bool ranned = false;

    public void AddNewGraphDisplay(GameObject graph)
    {
        GameObject newDisplay = Instantiate(graphDisplayPrefab, contentLayout.transform);
        SingleGraphUIController singleControl = newDisplay.GetComponent<SingleGraphUIController>();

        //set stuffs
        singleControl.SetGraph(graph.GetComponent<GraphController>());
        singleControl.ChangeTextField(graph.GetComponent<GraphController>().graphEquation);
        singleControl.ChangeTitle("Graph " + (graph.GetComponent<GraphController>().graphIdx + 1).ToString());
        //singleControl.ChangeGraphInput(); //no need since created anyway

        graphDisplays.Add(newDisplay);

        //resize
        if (!ranned) { graphDisplayHeight = graphDisplayPrefab.GetComponent<RectTransform>().sizeDelta.y; ranned = true;}

        contentLayout.GetComponent<RectTransform>().sizeDelta = new Vector2(contentLayout.GetComponent<RectTransform>().sizeDelta.x, 
            graphDisplayHeight * graphDisplays.Count);
    }   

    public void DeleteExistingGraph(int idx)
    {
        GameObject graphDisplayToBeKilled = graphDisplays[idx];
        graphDisplays.RemoveAt(idx);
        Destroy(graphDisplayToBeKilled);

        //refresh layout
        contentLayout.GetComponent<RectTransform>().sizeDelta = new Vector2(contentLayout.GetComponent<RectTransform>().sizeDelta.x, 
            graphDisplayHeight * graphDisplays.Count);
    }
}
