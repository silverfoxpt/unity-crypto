using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class VerletCollision : MonoBehaviour
{
    private List<VerletMove> bodies;
    private Dictionary<Vector2Int, List<VerletMove>> divide;
    private Dictionary<VerletMove, Vector2Int> reverseDivide;
    private float size, bound;

    private int cnt;
    [SerializeField] private int iter = 10;
    [SerializeField] private VerletSpawn spawn;

    private void Awake() 
    {
        divide = new Dictionary<Vector2Int, List<VerletMove>>();    
        reverseDivide = new Dictionary<VerletMove, Vector2Int>();
    }

    private Vector2Int ConvertSquare(Vector2 pos)
    {
        return new Vector2Int((int) (pos.x / size), (int) (pos.y / size));
    }

    void Start()
    {
        bodies = FindObjectsOfType<VerletMove>().ToList();

        cnt = bodies.Count;
        size = spawn.GetScale();
        bound = spawn.GetBounds();

        //divide space
        for (float x = -bound; x <= bound; x += size/2)
        {
            for (float y = -bound; y <= bound; y += size/2)
            {
                Vector2Int posSpace = ConvertSquare(new Vector2(x, y));
                if (!divide.ContainsKey(posSpace))
                {
                    divide.Add(posSpace, new List<VerletMove>());
                }
            }
        }

        for (int i = 0; i < cnt; i++)
        {
            Vector2Int posSpace = ConvertSquare(bodies[i].transform.position);

            divide[posSpace].Add(bodies[i]);
            reverseDivide[bodies[i]] = posSpace;
        }
    }

    void Update()
    {
        SolveAllCollision();
        UpdateDivisions();
    }

    private void UpdateBody(VerletMove body)
    {
        divide[reverseDivide[body]].Remove(body); //remove from old division

        Vector2 pos = body.transform.position;
        Vector2Int posSpace = ConvertSquare(pos);

        //add to new division
        divide[posSpace].Add(body);
        reverseDivide[body] = posSpace;
    }

    private void UpdateDivisions()
    {
        for (int i = 0; i < cnt; i++)
        {
            UpdateBody(bodies[i]);
        }
    }

    private void SolveAllCollision()
    {
        for (int counter = 0; counter < iter; counter++)
        {
            float strength = (float)(counter + 1) / iter;

            for (int i = 0; i < cnt; i++)
            {
                Vector2 pos = bodies[i].pos;
                Vector2Int spacePos = ConvertSquare(pos);

                //solve with neighbour divisions
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        //if (x == 0 && y == 0) {continue;}

                        Vector2Int cur = spacePos + new Vector2Int(x, y);
                        if (!divide.ContainsKey(cur)) {continue;}

                        //var tmpList = new List<VerletMove>(divide[cur]);
                        foreach(var obj in divide[cur])
                        {
                            SolveCollision(bodies[i], obj, strength);
                        }
                    }
                }
            }
        }
    }

    private void SolveCollision(VerletMove b1, VerletMove b2, float strength)
    {
        if (b1 == b2) {return;}

        float r1 = size/2, r2 = size/2;
        Vector2 pos1 = b1.pos, pos2 = b2.pos;

        float mag1 = (r1 + r2) * (r1 + r2);
        float mag2 = (pos2 - pos1).sqrMagnitude;

        if (mag1 > mag2) //collision
        {
            b2.pos += (pos2 - pos1).normalized * (mag1 - mag2) * strength;
        }
    }
}
