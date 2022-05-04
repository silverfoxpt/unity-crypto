using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleController : MonoBehaviour
{
    private LineRenderer rend;

    void Start()
    {
        rend = GetComponent<LineRenderer>(); 

        rend.positionCount = 1;
        rend.SetPosition(0, transform.position);

        //test();
    }

    private void test()
    {
        //sqiare
        SetWidth(0.02f);
        for (int i = 0; i < 4; i++)
        {
            Forward(2);
            SpinRight(90);
        }
    }

    public void SetWidth(float w)
    {
        rend.startWidth = w;
        rend.endWidth = w;
    }

    public void Forward(float step)
    {
        Vector2 dir = transform.up.normalized * step;
        Vector2 dest = (Vector2) transform.position + dir;
        
        rend.positionCount++;
        rend.SetPosition(rend.positionCount-1, dest);

        transform.position = dest;
    }

    public void SpinRight(float angle)
    {
        transform.Rotate(new Vector3(0f, 0f, 360f - angle));
    }

    public void SpinLeft(float angle)
    {
        transform.Rotate(new Vector3(0f, 0f, angle));
    }
}
