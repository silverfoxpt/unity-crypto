using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;

public class GeneralDisplayCell : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tex;
    
    private Image img;
    private RectTransform rect;

    private void Awake()
    {
        img = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
    }

    public void ChangeToTextState(string text)
    {
        tex.enabled = true;
        img.enabled = true;

        tex.text = text;

        img.color = Color.white;
        img.sprite = null;
    }

    public void ChangeToImageState(Sprite im)
    {
        tex.enabled = false;
        img.enabled = true;

        img.color = Color.white;
        img.sprite = im;
    }

    public void ChangeToColorState(Color col)
    {
        tex.enabled = false;
        img.enabled = true;

        img.sprite = null;
        img.color = col;
    }

    public void ChangeToColorAndTextState(Color col, string te)
    {
        tex.enabled = true;
        img.enabled = true;

        img.sprite = null; img.color = col;
        tex.text = te;
    }

    public void SetPosition(Vector2 pos)
    {
        rect.anchoredPosition = pos;
    }

    public void SetScale(float f)
    {
        rect.localScale = new Vector3(f, f, 1f);
    }

    public void Disable() {this.gameObject.SetActive(false);}
    public void Enable() {this.gameObject.SetActive(true);}
}
