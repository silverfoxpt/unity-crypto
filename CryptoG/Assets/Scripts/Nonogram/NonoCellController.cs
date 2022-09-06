using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NonoCellController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tex;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SetScale(float f) { GetComponent<RectTransform>().localScale = new Vector3(f, f, 1f);}
    public void SetPosition(Vector2 pos) {GetComponent<RectTransform>().anchoredPosition = pos;}

    public void SetText(string t) {tex.text = t;}
}
