using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsMove : MonoBehaviour
{
    public float timeStep;
    public Vector2 acceleration;
    public Vector2 pos, prevPos;
    public bool stati = false;

    void Start()
    {
        pos = prevPos = transform.position;    
    }

    private void Update()
    {
        if (!stati)
        {
            //verlet intergration
            Vector2 newPos = 2*pos - prevPos + acceleration * timeStep * timeStep; 
            prevPos = pos; pos = newPos;

            transform.position = pos;
        }
        else {pos = transform.position;}
    }

    public void ForceUpdatePos()
    {
        if (stati) {return;}
        transform.position = pos;
    }

    public void TurnOffRend() {GetComponent<SpriteRenderer>().enabled = false;}
}
