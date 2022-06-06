using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerletMove : MonoBehaviour
{
    [SerializeField] private Vector2 acceleration;
    [SerializeField] private float maxAccel = 0.1f;
    [SerializeField] private float timeStep = 1f;
    [SerializeField] private bool useDeltaTime = false;
    [SerializeField] private float drag = 0.95f;
    [SerializeField] private float minSpeed = 0.01f;

    public float radius;
    public Vector2 pos;

    Vector2 prevPos;

    private void Start()
    {
        pos = prevPos = transform.position;
        radius = transform.localScale.x/2;
    }

    void Update()
    {
        Verlet();
    }

    private void Verlet()
    {
        //calculate acceleration
        acceleration = new Vector2(0f, 0f) - (Vector2)transform.position;
        acceleration = Vector2.ClampMagnitude(acceleration, maxAccel);

        //calculate time step
        float dt = (useDeltaTime) ? Time.deltaTime : timeStep;

        //verlet intergration
        Vector2 newPos = pos + drag * (pos - prevPos) + acceleration * dt * dt;
        prevPos = pos; pos = newPos;

        //apply position
        transform.position = pos;

        //if ((pos - prevPos).magnitude <= minSpeed) {ForceUpdatePrevpos(); } //resting state
    }

    public void ForceUpdatePrevpos() {prevPos = pos;}

    public void SetMaxAccel(float mag) {maxAccel = mag;}
    public void SetTimeStep(float dt) {timeStep = dt;}
    public void SetBoolTime(bool y) {useDeltaTime = y;}
    public void SetDrag(float d) {drag = d;}
}
