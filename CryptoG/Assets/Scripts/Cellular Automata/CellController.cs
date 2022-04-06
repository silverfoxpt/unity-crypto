using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellController : MonoBehaviour
{
    public void SetSize(float sz)
    {
        transform.localScale = new Vector3(sz, sz, 1f);
    }

    public void SetColor(Color col) {GetComponent<SpriteRenderer>().color = col;}

    public int GetColorCode() {return GetComponent<SpriteRenderer>().color == Color.black ? 1 : 0;}

    private void Start()
    {
        //SetSize(0.5f); SetColor(Color.green); //test
    }
}
