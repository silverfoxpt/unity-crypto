using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftBodySim : MonoBehaviour
{
    struct spring
    {
        public Masspoint pa, pb;
        public float restLength;

        public spring(Masspoint a, Masspoint b, float rest) { pa = a; pb = b; restLength = rest;}
    }

    [Header("Points")]
    [SerializeField] private GameObject massPoint;
    [SerializeField] private float mass = 1f;

    [Header("Springs")]
    [SerializeField] private float space = 1f;
    [SerializeField] private float dampingFactor = 0.5f;
    [SerializeField] private float stiffness = 0.7f;

    [Header("Other parameters")]
    [SerializeField] private float gravitationalConstant = 0.1f;
    [SerializeField] private float deltaTime = 0.1f;
    [SerializeField] private float lowerBound = -4.5f;

    private List<Masspoint> masses;
    private List<spring> springs;

    void Start()
    {
        springs = new List<spring>();
        masses = new List<Masspoint>();

        /*//test
        Masspoint a = Instantiate(massPoint, new Vector2(0, 0), Quaternion.identity, transform).GetComponent<Masspoint>();
        Masspoint b = Instantiate(massPoint, new Vector2(0, -1), Quaternion.identity, transform).GetComponent<Masspoint>();
        a.mass = 1; b.mass = 1;

        springs.Add(new spring(a, b, space));
        masses.Add(a); masses.Add(b);

        b.velocity.x += 0.1f;
        a.velocity.x += -0.1f;*/
    }

    void Update()
    {
        //reset
        ResetAllForces();
        
        //accumulate forces
        UpdateAllSprings();  
        UpdateGravity();

        //apply
        ApplyForce();
    }

    private void UpdateGravity()
    {
        for (int i = 0; i < masses.Count; i++)
        {
            //if (i == 0) {continue;} //test
            masses[i].force.y += -(gravitationalConstant * masses[i].mass);
        }
    }

    private void ApplyForce()
    {
        foreach(var p in masses)
        {
            p.acceleration = p.force / p.mass; //reset acceleration completely
            p.velocity += p.acceleration; 

            p.pos += p.velocity * deltaTime;
            p.pos.y = Mathf.Clamp(p.pos.y, lowerBound, float.MaxValue);

            p.UpdatePosition();
        }
    }

    private void ResetAllForces()
    {
        foreach(var p in masses)
        {
            p.force = Vector2.zero;
        }
    }

    private void UpdateAllSprings()
    {
        foreach(var sp in springs)
        {
            Masspoint a = sp.pa, b = sp.pb;

            float springForce = ((a.pos - b.pos).magnitude - sp.restLength) * stiffness; 

            var dampForceB = (b.velocity * -dampingFactor);
            var dampForceA = (a.velocity * -dampingFactor);

            b.force += (a.pos - b.pos).normalized * springForce + dampForceB;
            a.force += (b.pos - a.pos).normalized * springForce + dampForceA;
        }
    }
}
