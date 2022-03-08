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
    private Color normalColor;

    private void Start()
    {
        normalColor = GetComponent<Image>().color;
        keyButton.onClick.AddListener(() => {
            GetComponent<Image>().color = lightUpColor;
            EnigmaManager.instance.KeyInputClicked(keyText.text[0]);
        });
    }

    //turn off
    public void LightDown()
    {
        GetComponent<Image>().color = normalColor;
    }
}
