using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntController : MonoBehaviour
{
    public float maxSpeed = 2;
    public float steerStrength = 2;
    public float wanderStrength = 1;
    
    private Vector2 position;
    public Vector2 velocity;
    public Vector2 desiredDirection; //needed to be set
    public int foodStat = 0; //0 not found, 1 found, 2 got food + return

    private void Start()
    {
        position = transform.position;
        desiredDirection = Random.insideUnitCircle;
    }
    
    void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        desiredDirection = (desiredDirection + Random.insideUnitCircle * wanderStrength).normalized;

        Vector2 desiredVelocity = desiredDirection * maxSpeed;
        Vector2 desiredSteer = (desiredVelocity - velocity) * steerStrength;
        Vector2 acceleration = Vector2.ClampMagnitude(desiredSteer, steerStrength) / 1; //mass = 1

        velocity = Vector2.ClampMagnitude(velocity + acceleration * Time.deltaTime, maxSpeed);
        position += velocity * Time.deltaTime;

        transform.up = velocity;
        transform.position = position;
    }
}
