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

    [Header("Generation")]
    [SerializeField] private Vector2 size = new Vector2(1.5f, 1.5f);
    [SerializeField] private int row = 5;
    [SerializeField] private int col = 5;

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
    private List<List<Masspoint>> doubleMass;
    private List<spring> springs;

    private float rowSpace, colSpace;

    void Start()
    {
        CreateAllPoints();
        AddSprings();
    }

    private void AddSprings()
    {
        springs = new List<spring>();
        float diagonal = Mathf.Sqrt(2 * space * space);

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (j != 0)
                {
                    var new1 = new spring(doubleMass[i][j], doubleMass[i][j-1], space); //left
                    springs.Add(new1);
                    if (i != row-1)
                    {
                        var new2 = new spring(doubleMass[i][j], doubleMass[i+1][j-1], diagonal); //bottom left
                        springs.Add(new2);
                    }
                }

                if (j != col-1)
                {
                    var new1 = new spring(doubleMass[i][j], doubleMass[i][j+1], space); //right
                    springs.Add(new1);
                    if (i != row-1)
                    {
                        var new2 = new spring(doubleMass[i][j], doubleMass[i+1][j+1], diagonal); //bottom right
                        springs.Add(new2);
                    }
                }

                if (i != row-1)
                {
                    var new1 = new spring(doubleMass[i][j], doubleMass[i+1][j], space); //down
                    springs.Add(new1);
                }
            }
        }
    }

    private void CreateAllPoints()
    {
        masses = new List<Masspoint>();
        doubleMass = new List<List<Masspoint>>();

        rowSpace = size.y / row;
        colSpace = size.x / col;

        float startX = -size.x/2f, startY = size.y/2f;

        for (int i = 0; i < row; i++)
        {
            doubleMass.Add(new List<Masspoint>());
            for (int j = 0; j < col; j++)
            {
                Masspoint newMass = Instantiate(massPoint, new Vector2(startX, startY), Quaternion.identity, transform).GetComponent<Masspoint>();
                masses.Add(newMass); doubleMass[i].Add(newMass);
                newMass.mass = mass;

                startX += colSpace;
            }
            startY += rowSpace;
            startX = -size.x / 2f;
        }
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

        //spring line
        foreach(var sp in springs)
        {
            Debug.DrawLine(sp.pa.pos, sp.pb.pos, Color.green, Time.deltaTime);
        }
    }

    private void UpdateGravity()
    {
        for (int i = 0; i < masses.Count; i++)
        {
            masses[i].force.y += -(gravitationalConstant * masses[i].mass);
        }
    }

    private void ApplyForce()
    {
        foreach(var p in masses)
        {
            if (p.mass == 0) {continue;}

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
