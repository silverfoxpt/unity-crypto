using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MineCell : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI tex;

    public void SetSize(float sz) { transform.localScale = new Vector3(sz, sz, 1f);}
    public void SetText(string t) {tex.text = t;}
    public string GetText() {return tex.text;}
}
