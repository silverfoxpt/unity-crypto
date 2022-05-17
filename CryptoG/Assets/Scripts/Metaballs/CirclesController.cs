using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CirclesController : MonoBehaviour
{
    [SerializeField] private GameObject circlePref;
    [SerializeField] private int circleCount = 30;
    [SerializeField] private Vector2 sizeRange = new Vector2(1, 2.5f);
    [SerializeField] private Vector2 speedRange = new Vector2(0.75f, 2f);

    public static List<SeedCircleController> circles;
    private float leftBound, rightBound, topBound, bottomBound;

    void Start()
    {
        circles = new List<SeedCircleController>();
        for (int i = 0; i < circleCount; i++)  
        {
            var cir = CreateNewCircle();
            circles.Add(cir);
        } 
    }

    private SeedCircleController CreateNewCircle()
    {
        var cir = Instantiate(circlePref, UtilityFunc.GetRandPos(leftBound, rightBound, bottomBound, topBound), Quaternion.identity, transform);

        var con = cir.GetComponent<SeedCircleController>();
        con.SetSpeed(UnityEngine.Random.Range(speedRange.x, speedRange.y));
        con.SetSize(UnityEngine.Random.Range(sizeRange.x, sizeRange.y));

        return con;
    }

    private void GetBounds()
    {
        Vector2 bPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
        rightBound = bPos.x; leftBound = -rightBound;
        topBound = bPos.y; bottomBound = -topBound;

        leftBound += sizeRange.y/2f; rightBound -= sizeRange.y/2f;
        topBound -= sizeRange.y/2f; bottomBound += sizeRange.y/2f;
    }
}
