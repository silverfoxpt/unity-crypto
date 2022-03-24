using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtonManager : MonoBehaviour
{
    [SerializeField] private PopupController popupController;
    public void StartProj()
    {
        SceneManager.LoadScene("Chooser");
    }

    public void OpenCredit()
    {
        popupController.EnablePopup();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
