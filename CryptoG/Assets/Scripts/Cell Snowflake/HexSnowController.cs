using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexSnowController : MonoBehaviour
{
    public HexSnowController topLeftHex     = null;
    public HexSnowController topRightHex    = null;
    public HexSnowController leftHex        = null;
    public HexSnowController rightHex       = null;
    public HexSnowController bottomLeftHex  = null;
    public HexSnowController bottomRightHex = null;

    private SpriteRenderer rend;

    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    public void SetPos(Vector2 pos)
    {
        transform.position = pos;
    }

    public void SetColor(Color col, bool turnOn = false)
    {
        if (turnOn) {RendOn();}
        rend.color = col;
    }

    public void SetSize(float sz)
    {
        transform.localScale = new Vector3(sz, sz, 1);
    }

    public void RendOff() {rend.enabled = false;}
    public void RendOn() {rend.enabled = true;}

    public bool GetState() {return rend.enabled;}

    public Vector2 GetPos() { return transform.position; }
}
