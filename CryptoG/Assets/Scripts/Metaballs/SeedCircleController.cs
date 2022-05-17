using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedCircleController : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    private float leftBound, rightBound, topBound, bottomBound;
    private float size;
    private Rigidbody2D rigid;

    private void Awake() 
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        GetBounds();
        SetNewVelocity();
    }

    private void SetNewVelocity()
    {
        Vector2 spin = UtilityFunc.RotatePoint(Vector2.up, UnityEngine.Random.Range(0f, 360f));
        rigid.velocity = spin * speed;
    }

    private void GetBounds()
    {
        size = transform.localScale.x;

        Vector2 bPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
        rightBound = bPos.x; leftBound = -rightBound;
        topBound = bPos.y; bottomBound = -topBound;

        leftBound += size/2f; rightBound -= size/2f;
        topBound -= size/2f; bottomBound += size/2f;
    }

    private void Update()
    {
        size = transform.localScale.x;
        Vector2 pos = transform.position;
        Vector2 spin = rigid.velocity;

        if (pos.x < leftBound || pos.x > rightBound)
        {
            spin.x = -spin.x;
        }
        if (pos.y > topBound || pos.y < bottomBound)
        {
            spin.y = -spin.y;
        }
        rigid.velocity = spin;

        pos.x = Mathf.Clamp(pos.x, leftBound, rightBound);
        pos.y = Mathf.Clamp(pos.y, bottomBound, topBound);
        transform.position = pos;
    }

    public void SetSpeed(float sp) {speed = sp;}
    public void SetSize(float sz) {transform.localScale = new Vector3(sz, sz, 1f); size = sz;}

    public float GetSize() {return transform.localScale.x;}
    public Vector2 GetPos() {return transform.position;}

    public void TurnOffRend() {GetComponent<SpriteRenderer>().enabled = false;}
}
