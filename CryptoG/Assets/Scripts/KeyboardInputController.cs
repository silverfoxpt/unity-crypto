using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyboardInputController : MonoBehaviour
{
    [SerializeField] private Button keyButton;
    [SerializeField] private TextMeshProUGUI keyText;
    [SerializeField] private Color lightUpColor;
    [SerializeField] private bool isOutputKey;
    private Color normalColor;

    private void Start()
    {
        normalColor = GetComponent<Image>().color;
        if (!isOutputKey)
        {
            keyButton.onClick.AddListener(() => {
                //LightUp(); //not going to work
                EnigmaManager.instance.KeyInputClicked(keyText.text[0]);
            });
        }
    }

    //turn off
    public void LightDown()
    {
        GetComponent<Image>().color = normalColor;
    }

    public char GetKeyChar()
    {
        return keyText.text[0];
    }

    public void LightUp()
    {
        GetComponent<Image>().color = lightUpColor;
    }
}
