using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    public float mass;
    public float radius;
    public Vector2 initialVelocity;

    private Vector2 currentVelocity;

    private void Awake() 
    {
        currentVelocity = initialVelocity;    
    }

    public void UpdateVelocity(CelestialBody[] allBodies, float timeStep)
    {
        foreach(var other in allBodies)
        {
            if (other != this)
            {
                float sqrDist = (other.transform.position - transform.position).sqrMagnitude;

                Vector2 forceDir = (other.transform.position - transform.position).normalized;
                Vector2 force = forceDir * Universe.gravitationalConstant * mass * other.mass / sqrDist;
                Vector2 acceleration = force / mass;

                currentVelocity += acceleration * timeStep;
            }
        }
    }

    public void UpdatePosition(float timeStep)
    {
        transform.position = (Vector2) transform.position + currentVelocity * timeStep;
    }
}
