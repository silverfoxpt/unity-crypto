using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateBody : MonoBehaviour
{
    public CelestialBody[] bodies;

    private void Awake()
    {
        Time.fixedDeltaTime = Universe.physicsTimeStep;
    }

    private void FixedUpdate() 
    {
        for (int i = 0; i < bodies.Length; i++)
        {
            bodies[i].UpdateVelocity(bodies, Universe.physicsTimeStep);
        }    

        for (int i = 0; i < bodies.Length; i++)
        {
            bodies[i].UpdatePosition(Universe.physicsTimeStep);
        }    
    }
}
