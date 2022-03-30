using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareWallController : MonoBehaviour
{
    [Header("Borders")]
    [SerializeField] private GameObject leftBorder;
    [SerializeField] private GameObject rightBorder;
    [SerializeField] private GameObject topBorder;
    [SerializeField] private GameObject bottomBorder;

    [Header("Other")]
    [SerializeField] private Color lightUpColor;
    [SerializeField] private Color secondaryColor;
    [SerializeField] private Color tertiaryColor;
    private Color normColor;
    private SpriteRenderer rend;

    void Awake()
    {
        InitializeSquare();
        //EnableAllBorder();
    }

    private void InitializeSquare()
    {
        rend = GetComponent<SpriteRenderer>();
        normColor = rend.color;
    }

    public void EnableAllBorder() 
    {
        for (int idx = 0; idx < 4; idx++) {EnableSingleBorder(idx);}
    }
    public void EnableSingleBorder(int idx) 
    {
        switch(idx)
        {
            case 0: leftBorder.SetActive(true); break;
            case 1: rightBorder.SetActive(true); break;
            case 2: topBorder.SetActive(true); break;
            case 3: bottomBorder.SetActive(true); break;
        }
    }

    public void DisableAllBorder() //may not be needed
    {
        for (int idx = 0; idx < 4; idx++) {DisableSingleBorder(idx);}
    }
    public void DisableSingleBorder(int idx) 
    {
        switch(idx)
        {
            case 0: leftBorder.SetActive(false); break;
            case 1: rightBorder.SetActive(false); break;
            case 2: topBorder.SetActive(false); break;
            case 3: bottomBorder.SetActive(false); break;
        }
    }

    public void LightUpSquare() {rend.color = lightUpColor;}
    public void LightUpSquareSecondary() {rend.color = secondaryColor;}
    public void LightUpSquareTertiary() {rend.color = tertiaryColor;}

    public void LightDownSquare() {rend.color = normColor;}
    public bool IsLightUp() {return rend.color != normColor;}
}
