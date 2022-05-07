using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EulerCurve : MonoBehaviour
{
    [SerializeField] private int step = 1000;
    [SerializeField] private float theta = 1f;
    [SerializeField] private float width = 0.015f;
    [SerializeField] private float delay = 0.1f;
    [SerializeField] private float stepLength = 1f;
    [SerializeField] private TurtleController turtle;

    void Start()
    {
        StartCoroutine(DrawCurve());
    }

    IEnumerator DrawCurve()
    {
        turtle.SetWidth(width);
        turtle.SpinRight(90);

        for (int i = 0; i < step; i++)
        {
            yield return new WaitForSeconds(delay);
            turtle.Forward(stepLength);
            turtle.SpinLeft((i+1) * theta);
        }
    }
}
