using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarnsleyFernPoint : MonoBehaviour
{
    private SpriteRenderer sp;
    private void Awake()
    {
        sp = GetComponent<SpriteRenderer>();
    }

    public void SetSize(float s) {transform.localScale = new Vector3(s, s, 1f);}
    public void SetColor(Color col) {sp.color = col;}
    public void SetPos(Vector2 pos) {transform.position = pos;}

    public Vector2 GetPos() {return transform.position;}
}
