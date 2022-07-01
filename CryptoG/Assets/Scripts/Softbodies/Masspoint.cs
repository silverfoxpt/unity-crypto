using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Masspoint : MonoBehaviour
{
    public Vector2 pos;
    public Vector2 velocity;
    public Vector2 acceleration;
    public Vector2 force;
    public float mass;

    private void Awake()
    {
        pos = transform.position;
    }

    public void UpdatePosition() {transform.position = pos;}    
    public void TurnOffRender() {GetComponent<SpriteRenderer>().enabled = false;}
}
