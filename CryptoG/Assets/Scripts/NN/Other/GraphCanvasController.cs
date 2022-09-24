using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GraphCanvasController : MonoBehaviour
{
    [Header("Options")]
    [SerializeField] private float addLen;
    [SerializeField] private string title;
    [SerializeField] private float font;

    void Start()
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -addLen);    

        var ti = new GameObject();
        ti.AddComponent<RectTransform>();

        var rec = ti.GetComponent<RectTransform>();
        rec.SetParent(this.transform);
        rec.anchorMin = new Vector2(0f, 0f);
        rec.anchorMax = new Vector2(1f, 1f);
        

        var text = ti.AddComponent<TextMeshProUGUI>();
        text.text = title;
        text.color = Color.white;
        text.fontSize = font;     

        rec.anchoredPosition = new Vector2(0f, 0f);
        rec.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, 0);
        rec.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
        rec.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
        rec.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 0);
        rec.anchorMin = new Vector2(0f, 0f);
        rec.anchorMax = new Vector2(1f, 1f);
    }
}
