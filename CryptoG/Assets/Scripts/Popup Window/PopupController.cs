using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupController : MonoBehaviour
{
    public void DisablePopup()
    {
        this.gameObject.SetActive(false);
    }

    public void EnablePopup()
    {
        this.gameObject.SetActive(true);
    }
}
