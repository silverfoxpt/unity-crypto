using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VerletCollision : MonoBehaviour
{
    private List<VerletMove> bodies;
    private int cnt;
    [SerializeField] private int iter = 10;
    void Start()
    {
        bodies = FindObjectsOfType<VerletMove>().ToList();
        cnt = bodies.Count;
    }

    void Update()
    {
        for (int counter = 0; counter < iter; counter++)
        {
            float strength = (float) (counter+1)/iter;

            for (int i = 0; i < cnt; i++)
            {
                for (int j = 0; j < cnt; j++)
                {
                    if (i == j) {continue;}

                    float r1 = bodies[i].radius, r2 = bodies[j].radius;
                    Vector2 pos1 = bodies[i].pos, pos2 = bodies[j].pos;

                    float mag1 = (r1 + r2);
                    float mag2 = (pos2 - pos1).magnitude;

                    if (mag1 > mag2) //collision
                    {
                        bodies[j].pos += (pos2 - pos1).normalized * (mag1 - mag2) * strength;
                    }
                }
            }
        }
    }
}
