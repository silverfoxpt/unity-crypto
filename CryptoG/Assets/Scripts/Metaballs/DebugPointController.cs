using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugPointController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameObject canvas;
    private float value = float.MaxValue;

    public void SetPos(Vector2 pos)
    {
        transform.position = pos;
    }

    public void SetVal(float val)
    {
        value = val;
        text.text = System.Math.Round(val, 2).ToString();
    }

    public float GetVal() {return value;}

    public Vector2 GetPos() {return transform.position;}

    public void TurnOffgraphics() { GetComponent<SpriteRenderer>().enabled = false; canvas.SetActive(false); }
}
