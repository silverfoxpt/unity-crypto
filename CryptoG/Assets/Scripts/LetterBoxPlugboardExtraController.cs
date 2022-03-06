using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LetterBoxPlugboardExtraController : MonoBehaviour
{
    [SerializeField] private GameObject fullRing;

    //add method to onClick()
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => {fullRing.GetComponent<PlugBoardExtraController>().BoxClicked(gameObject);} );
    }
}
