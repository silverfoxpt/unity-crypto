using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CellboardCellController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tex;
    [SerializeField] private GameObject can;

    public void SetSize(float sz) {transform.localScale = new Vector3(sz, sz, 1);}

    public void SetText(string t) {tex.text = t;}

    public void OffCanvas() {can.SetActive(false);}
}
