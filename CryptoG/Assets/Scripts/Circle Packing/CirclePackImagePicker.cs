//https://github.com/gkngkc/UnityStandaloneFileBrowser
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CirclePackImagePicker : MonoBehaviour
{
    [SerializeField] private Image image;

    public void OpenImage()
    {
        string actualPath           = UtilityFunc.OpenImageFile(); if(actualPath == "") {return;}
        
        Texture2D tex               = UtilityFunc.LoadImage(actualPath);
        Sprite newImgSprite         = UtilityFunc.Texture2DToSprite(tex);
        image.sprite                = newImgSprite;
    }
}
