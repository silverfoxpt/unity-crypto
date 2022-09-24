using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointControlNN : MonoBehaviour
{
    public void SetColor(Color col)
    {
        GetComponent<SpriteRenderer>().color = col;
    }

    public void SetScale(float i)
    {
        transform.localScale = new Vector3(i, i, 1f);
    }

    public void SetGlobalPos(Vector2 pos)
    {
        transform.position = pos;
    }

    public void SetLocalPos(Vector2 pos)
    {
        transform.localPosition = pos;
    }


    public Vector2 GetLocalPos() {return transform.localPosition;}
}
