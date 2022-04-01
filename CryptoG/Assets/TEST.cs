using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST : MonoBehaviour
{
    public Texture2D tex;
    void Start()
    {
        GetComponent<SpriteRenderer>().color = new Color(10f, 0f, 0f, 1f);
    }
}
