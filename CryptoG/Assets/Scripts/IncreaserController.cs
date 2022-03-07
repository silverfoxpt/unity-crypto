using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class IncreaserController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textInc;
    
    public void IncreaseText() { textInc.text = (textInc.text[0] == 'Z') ? textInc.text = "A" : textInc.text = ((char) (textInc.text[0] + 1)).ToString(); }
    public void DecreaseText() { textInc.text = (textInc.text[0] == 'A') ? textInc.text = "Z" : textInc.text = ((char) (textInc.text[0] - 1)).ToString(); }
    public void ResetText() { textInc.text = "A";}
}
