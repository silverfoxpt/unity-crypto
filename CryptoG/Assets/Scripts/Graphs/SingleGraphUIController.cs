using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SingleGraphUIController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private GameObject graphInput;
    [SerializeField] private Button submitButton;
    [SerializeField] private Button deleteButton;

    private GraphController myGraph;
    private GraphDrawer graphDrawer;

    private void Start()
    {
        graphDrawer = FindObjectOfType<GraphDrawer>();
        submitButton.onClick.AddListener(() => {ChangeGraphInput();});
    }

    public void SetGraph(GraphController graph)
    {
        myGraph = graph;
        deleteButton.onClick.AddListener(() => {graphDrawer.DeleteSingleGraph(graph.graphIdx);});
    }
    public void ChangeTitle(string newTitle) {title.text = newTitle;}
    public void ChangeTextField(string tex)
    {
        graphInput.GetComponent<TMP_InputField>().text = tex;
    }

    public void ChangeGraphInput()
    {
        string change = graphInput.GetComponent<TMP_InputField>().text;

        FindObjectOfType<GraphDrawer>().ModifyGraph(myGraph.graphIdx, change);
    }   
}
