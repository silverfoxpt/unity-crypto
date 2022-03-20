using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerController : MonoBehaviour
{
    [SerializeField] private GameObject sceneBehaves; //debug purpose

    public void SetSceneBehaviourScripts(GameObject sc)
    {
        sceneBehaves = sc;
    }

    private void OnMouseEnter() 
    {
        Debug.Log("yea");
        sceneBehaves.SetActive(true);
    }

    private void OnMouseExit()
    {
        Debug.Log("nah");
        sceneBehaves.SetActive(false);
    }
}
