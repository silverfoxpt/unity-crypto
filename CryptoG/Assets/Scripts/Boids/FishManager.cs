using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour
{
    [SerializeField] private GameObject fishPref;
    [SerializeField] private int fishNum = 100;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float detectionRange = 1f;
    [SerializeField] private float maxForce = 1f;
    [SerializeField] [Range(0f, 100f)] private float cohesionPercent = 100f;
    [SerializeField] [Range(0f, 100f)] private float seperationPercent = 1f;
    [SerializeField] [Range(0f, 100f)] private float alignmentPercent = 12.5f;
    [SerializeField] private bool spreadOut = false;

    [Header("Obstacle")]
    [SerializeField] private List<GameObject> obstacles;
    [SerializeField] private float obstacleMaxForce = 10f;
    [SerializeField] [Range(0f, 100f)] private float avoidPercent = 50f;

    private float leftBound, rightBound, topBound, bottomBound;
    private List<GameObject> fishes;
    private List<Rigidbody2D> rigids;
    private List<Vector2> position, velo;

    void Start()
    {
        GetBounds();
        GenFishes();   
    }

    private void GenFishes()
    {
        fishes = new List<GameObject>();
        rigids = new List<Rigidbody2D>();

        for (int i = 0; i < fishNum; i++)
        {
            var fish = CreateNewFish();
            fishes.Add(fish);
            rigids.Add(fish.GetComponent<Rigidbody2D>());
        }    
    }

    private GameObject CreateNewFish()
    {
        var pos = UtilityFunc.GetRandPos(leftBound, rightBound, bottomBound, topBound);
        var vel = UtilityFunc.RotatePoint(Vector2.up * speed, UnityEngine.Random.Range(0f, 360f));

        var fish = Instantiate(fishPref, pos, Quaternion.identity, transform);
        fish.GetComponent<Rigidbody2D>().velocity = vel;
        fish.transform.up = vel;

        return fish;
    }

    private void GetBounds()
    {
        Vector2 bPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
        rightBound = bPos.x; leftBound = -rightBound;
        topBound = bPos.y; bottomBound = -topBound;
    }

    private void Update()
    {
        UpdateVelocity();
        TeleportBoids();
    }

    private void UpdateVelocity()
    {
        position = new List<Vector2>();
        velo = new List<Vector2>();
        
        foreach(var fish in fishes)
        {
            position.Add(fish.transform.position);
        }

        foreach(var rig in rigids)
        {
            velo.Add(rig.velocity);
        }

        var newVelocity = new List<Vector2>();

        //cohesion
        for (int i = 0; i < fishNum; i++)
        {
            newVelocity.Add(velo[i]);
            
            Vector2 center = new Vector2(0f, 0f); int counter = 0;
            for (int j = 0; j < fishNum; j++)
            {
                if (i == j || UtilityFunc.Dist(position[i], position[j]) > detectionRange) {continue;}
                center += position[j]; counter++;
            }
            if (counter > 0) {center /= counter;} else {center = position[i];}

            Vector2 cohesion = Vector2.ClampMagnitude((center - position[i]), maxForce);
            cohesion *= (cohesionPercent / 100f);

            if (!spreadOut) { newVelocity[i] += cohesion; }//heads toward center of mass - cohesion
            else {newVelocity[i] -= cohesion; }
        }

        //seperation
        for (int i = 0; i < fishNum; i++)
        {
            Vector2 sep = new Vector2(0f, 0f);
            for (int j = 0; j < fishNum; j++)
            {
                if (i != j && UtilityFunc.Dist(position[i], position[j]) <= detectionRange)
                {
                    sep -= (position[j] - position[i]);
                }
            }

            Vector2 seperation = Vector2.ClampMagnitude(sep, maxForce);
            seperation *= (seperationPercent / 100f);

            newVelocity[i] += seperation;
        }

        //alignment
        for (int i = 0; i < fishNum; i++)
        {
            Vector2 align = new Vector2(0f, 0f);
            for (int j = 0; j < fishNum; j++)
            {
                if (i != j && UtilityFunc.Dist(position[i], position[j]) <= detectionRange)
                {
                    align += velo[j];
                }
            }

            Vector2 alignment = Vector2.ClampMagnitude(align, maxForce);
            alignment *= (seperationPercent / 100f);

            newVelocity[i] += alignment;
        }

        //avoid obstacle
        for (int i = 0; i < fishNum; i++)
        {
            Vector2 obs = new Vector2(0f, 0f);
            for (int j = 0; j < obstacles.Count; j++)
            {
                if (UtilityFunc.Dist(position[i], obstacles[j].transform.position) <= obstacles[j].GetComponent<ColumnsObstacle>().ObstacleRange())
                {
                    obs -= ((Vector2) obstacles[j].transform.position - position[i]);
                }
            }

            Vector2 obstacle = Vector2.ClampMagnitude(obs, obstacleMaxForce);
            obstacle *= (avoidPercent / 100f);

            newVelocity[i] += obstacle;
        }

        //add back velocity
        for (int i = 0; i < fishNum; i++)
        {
            rigids[i].velocity = newVelocity[i].normalized * speed;
            fishes[i].transform.up = newVelocity[i]; //spin in direction
        }        
    }

    private void TeleportBoids()
    {
        foreach(var fish in fishes)
        {
            if (OutOfBounds(fish.transform.position))
            {
                //magical teleport!
                var pos = fish.transform.position;
                if (pos.x > rightBound) {pos.x = leftBound;}
                if (pos.x < leftBound) {pos.x = rightBound;}

                if (pos.y > topBound) {pos.y = bottomBound;}
                if (pos.y < bottomBound) {pos.y = topBound;}

                fish.transform.position = pos;
            }
        }
    }

    private bool OutOfBounds(Vector2 pos)
    {
        if (pos.x < leftBound || pos.x > rightBound || pos.y < bottomBound || pos.y > topBound) {return true;}
        return false;
    }
}
