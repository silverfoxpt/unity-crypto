using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class DebugLine : MonoBehaviour
{
    struct virtualBody
    {
        public float mass, radius;
        public Vector2 initialVelocity;

        public Vector2 currentVelocity;
        public Vector2 currentPosition;
    }

    [Header("Line options")]
    [SerializeField] private float lineWidth = 0.025f;
    [SerializeField] private bool hideLineWhenPlaying = false;

    [Header("Draw options")]
    [SerializeField] private bool emergencyKillSwitch = false;
    public int numStep = 10000;
    public int maxStep = 100000;
    public float timeStep = 0.1f;
    public float minDist = 0.001f;

    public CelestialBody[] normBodies;
    
    private List<virtualBody> bodies;
    private List<List<Vector2> > drawPos;
    private List<LineRenderer> rends = new List<LineRenderer>();
    private int numBodies;

    private void Awake()
    {
        RefreshRenderers();
    }

    private void RefreshRenderers()
    {
        rends = new List<LineRenderer>();
        foreach (Transform child in transform)
        {
            var rend = child.gameObject.GetComponent<LineRenderer>();
            rends.Add(rend);
            rend.startWidth = lineWidth;
            rend.endWidth = lineWidth;
        }
    }

    private void Start()
    {
        if (Application.isPlaying && hideLineWhenPlaying)
        {
            HideOrbit();
        }
    }

    private void Update()
    {
        if (!Application.isPlaying && !emergencyKillSwitch)
        {
            DrawOrbits();
        }
    }

    private void HideOrbit()
    {
        foreach(var line in rends)
        {
            line.enabled = false;
        }
    }

    private void DrawOrbits()
    {
        numStep = (numStep > maxStep) ? maxStep : numStep;
        CreateVirtualBodies();
        UpdateLineRenderers();
        UpdateAllPhysics();
    }

    private void UpdateAllPhysics()
    {
        for (int counter = 0; counter < numStep; counter++)
        {
            UpdateAllVelocity();
            UpdateAllPosition();
            AddNewBodiesPosition();
        }
    }

    private void AddNewBodiesPosition()
    {
        for (int i = 0; i < numBodies; i++)
        {
            var rend = rends[i];
            int po = rend.positionCount;

            rend.positionCount = po+1;
            rend.SetPosition(po, bodies[i].currentPosition);
        }
    }

    private void UpdateAllPosition()
    {
        for (int i = 0; i < numBodies; i++)
        {
            virtualBody cur = bodies[i];
            cur.currentPosition += cur.currentVelocity * timeStep;

            bodies[i] = cur;
        }
    }

    private void UpdateAllVelocity()
    {
        for (int i = 0; i < numBodies; i++)
        {
            virtualBody cur = bodies[i];

            for (int j = 0; j < numBodies; j++)
            {
                virtualBody nei = bodies[j];
                if (i != j)
                {
                    float sqrDist = (nei.currentPosition - cur.currentPosition).sqrMagnitude;

                    Vector2 forceDir = (nei.currentPosition - cur.currentPosition).normalized;
                    Vector2 force = forceDir * Universe.gravitationalConstant * cur.mass * nei.mass / sqrDist;
                    Vector2 acceleration = force / cur.mass;

                    cur.currentVelocity += acceleration * timeStep;
                }
            }

            bodies[i] = cur;
        }
    }

    private void CreateVirtualBodies()
    {
        bodies = new List<virtualBody>();
        numBodies = normBodies.Length;

        for (int i = 0; i < numBodies; i++)
        {
            var bod = new virtualBody();

            bod.mass = normBodies[i].mass;
            bod.radius = normBodies[i].radius;
            bod.initialVelocity = normBodies[i].initialVelocity;
            bod.currentVelocity = bod.initialVelocity;
            bod.currentPosition = normBodies[i].gameObject.transform.position;

            bodies.Add(bod);
        }
    }

    private void UpdateLineRenderers()
    {
        if (numBodies != rends.Count)
        {
            RefreshRenderers();
        }
        
        for (int i = 0; i < numBodies; i++)
        {
            rends[i].enabled = true;
            rends[i].positionCount = 1;
            rends[i].SetPosition(0, bodies[i].currentPosition);
        }      
    }
}
