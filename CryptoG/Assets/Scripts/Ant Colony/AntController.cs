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
    public Vector2Int foodPos; //only accessible for compute shader FindFood
    private Vector4 mapSize;

    private void Start()
    {
        position = transform.position;
        desiredDirection = Random.insideUnitCircle;
    }
    
    void Update()
    {
        HandleMovement();
        CheckOutOfBounds();

        transform.up = velocity;
        transform.position = position;
    }

    private void CheckOutOfBounds()
    {
        var newPos = (Vector2) transform.position + velocity * Time.deltaTime * 20; //avoid strength, just hardcoded 
        if (newPos.x <= mapSize.x || newPos.x >= mapSize.y ||
            newPos.y <= mapSize.z || newPos.y >= mapSize.w)
        {
            desiredDirection = -transform.position;
        }
    }

    private void HandleMovement()
    {
        desiredDirection = (desiredDirection.normalized + Random.insideUnitCircle.normalized * wanderStrength).normalized;

        Vector2 desiredVelocity = desiredDirection * maxSpeed;
        Vector2 desiredSteer = (desiredVelocity - velocity) * steerStrength;
        Vector2 acceleration = Vector2.ClampMagnitude(desiredSteer, steerStrength) / 1; //mass = 1

        velocity = Vector2.ClampMagnitude(velocity + acceleration * Time.deltaTime, maxSpeed);
        position += velocity * Time.deltaTime;
    }

    public void SetAntMapSize(Vector4 bounds) {mapSize = bounds; }
}
