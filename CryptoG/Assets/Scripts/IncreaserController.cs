using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class IncreaserController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textInc;
    [SerializeField] private bool isRingIncreaser = false;
    
    public void IncreaseText() 
    { 
        if (!isRingIncreaser) { textInc.text = (textInc.text[0] == 'Z') ? textInc.text = "A" : textInc.text = ((char) (textInc.text[0] + 1)).ToString(); }
        else { textInc.text = (textInc.text == "25") ? textInc.text = "0" : textInc.text = (int.Parse(textInc.text) + 1).ToString(); }
    }
    public void DecreaseText() 
    { 
        if (!isRingIncreaser) { textInc.text = (textInc.text[0] == 'A') ? textInc.text = "Z" : textInc.text = ((char) (textInc.text[0] - 1)).ToString(); }
        else { textInc.text = (textInc.text == "0") ? textInc.text = "25" : textInc.text = (int.Parse(textInc.text) - 1).ToString(); }
    }
    public void ResetText() { textInc.text = "A";}
}
