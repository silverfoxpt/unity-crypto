using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChooserButtonController : MonoBehaviour
{
    [SerializeField] private int initialActiveTab = 0;
    [SerializeField] private List<Button> tabActivationButtons;
    [SerializeField] private List<GameObject> tabPanels;
    private int curActive;

    private void Start()
    {
        curActive = initialActiveTab;
        if (tabActivationButtons.Count != tabPanels.Count) {Debug.LogError("Not enough tabs!");}    

        for (int idx = 0; idx < tabPanels.Count; idx++)
        {
            tabPanels[idx].SetActive(false);
        }
        SetActiveTab(curActive);
    }

    public void SetActiveTab(int needToActivate)
    {
        //disable current tab
        tabPanels[curActive].SetActive(false);

        //enable new tab
        tabPanels[needToActivate].SetActive(true);
        
        curActive = needToActivate;
    }
}
