using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleControllerPack : MonoBehaviour
{
    private SpriteRenderer rend;
    private bool isGrowing = true;
    private float increments = -1f;
    private float refreshRate = -1f;

    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        StartCoroutine(ContinueGrow());
    }

    IEnumerator ContinueGrow()
    {
        while(isGrowing)
        {
            Grow();
            yield return new WaitForSeconds(refreshRate);
        }
    }

    private void TEST()
    {
        SetSize(0.5f);
        SetColor(new Color(1f, 1f, 0f, 1f));
    }

    public void SetSize(float size)
    {
        transform.localScale = new Vector3(size, size, 1f);
    }
    public void SetColor(Color col)
    {
        rend.color = col;
    }
    public void SetRefresh(float refer) 
    {
        refreshRate = refer;
    }
    public float GetSize() {return transform.localScale.x;}
    public Vector2 GetCenter() {return transform.position;}

    public void Grow()
    {
        transform.localScale = new Vector2(GetSize() + increments, GetSize() + increments);
    }
    public void TurnOnGrowing() {isGrowing = true;}
    public void TurnOffGrowing() {isGrowing = false;}
    public void SetGrowthRate(float gr) {increments = gr;}
    public bool Growing() {return isGrowing;}
}
