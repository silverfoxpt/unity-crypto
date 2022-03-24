using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ChooserController : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private List<string> scenes;
    public void EnterProj()
    {
        int cur = dropdown.value;
        SceneManager.LoadScene(dropdown.options[cur].text);
    }

    public void BackMain()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
