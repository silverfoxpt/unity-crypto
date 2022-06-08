using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothSimulation : MonoBehaviour
{
    struct link
    {
        public PointsMove  p1, p2;
        public LineRenderer rend;

        public link(PointsMove a, PointsMove b, LineRenderer r) {p1 = a; p2 = b; rend = r;}
    }

    [Header("Reference")]
    [SerializeField] private GameObject pointPref;
    [SerializeField] private GameObject linePref;

    [Header("Options")]
    [SerializeField] private float restingDist = 0.2f;

    [Space(10)]
    [SerializeField] private Vector2Int size;
    [SerializeField] private float startHeight = 3.5f;
    [SerializeField] private float timeStep = 0.1f;

    [Space(10)]
    [SerializeField] private float gravity = 9.8f;
    [SerializeField] private float wind = 0.1f;
    [SerializeField] private int iter = 10;

    [Space(10)]
    [SerializeField] private float lineWidth = 0.025f;
    [SerializeField] private Color lineColor = Color.black;
    [SerializeField] private bool showPoints = false;
    
    [Space(10)]
    [SerializeField] private float tearRadius = 1f;

    private List<List<PointsMove>> points;
    private List<link> links;

    private void Start()
    {
        CreateAllPoints();
        CreateAllLinks();
    }

    private void CreateAllLinks()
    {
        links = new List<link>();
        for (int i = 0; i < size.y; i++)
        {
            for (int j = 0; j < size.x; j++)
            {
                if (j != 0) 
                { 
                    var l = Instantiate(linePref, Vector3.zero, Quaternion.identity, transform);

                    var rend = l.GetComponent<LineRenderer>();
                    rend.positionCount = 2;
                    rend.startColor = rend.endColor = lineColor;
                    rend.startWidth = rend.endWidth = lineWidth;

                    links.Add(new link(points[i][j], points[i][j-1], rend)); 
                } //left
                if (i != 0) 
                {   
                    var l = Instantiate(linePref, Vector3.zero, Quaternion.identity, transform);

                    var rend = l.GetComponent<LineRenderer>();
                    rend.positionCount = 2;
                    rend.startColor = rend.endColor = lineColor;
                    rend.startWidth = rend.endWidth = lineWidth;
                    
                    links.Add(new link(points[i][j], points[i-1][j], rend)); 
                } //top
            }
        }
    }

    private void CreateAllPoints()
    {
        float startX = -restingDist * size.x/2, startY = startHeight;
        points = new List<List<PointsMove>>();

        for (int i = 0; i < size.y; i++)
        {
            points.Add(new List<PointsMove>());
            for (int j = 0; j < size.x; j++)
            {
                var p = Instantiate(pointPref, new Vector2(startX, startY), Quaternion.identity, transform);

                var move = p.GetComponent<PointsMove>();
                if (i == 0) {move.stati = true;} else {move.stati = false;} // first  row static
                move.timeStep = timeStep; move.acceleration = new Vector2(0f, 0f);
                if (!showPoints) {move.TurnOffRend();}

                startX += restingDist;
                points[i].Add(move);
            }
            startY -= restingDist; startX = -restingDist * size.x/2;
        }
    }

    private void Update()
    {
        ConstraintCloth();
        ApplyClothForce();
        CheckTearCloth();
    }

    private void CheckTearCloth()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            for (int i = 1; i < size.y; i++)
            {
                for (int j = 0; j < size.x; j++)
                {
                    if (!points[i][j]) {continue;}
                    
                    float mag = (mousePos - points[i][j].pos).magnitude;
                    if (mag <= tearRadius) {Destroy(points[i][j].gameObject);}
                }
            }
        }
    }

    private void ConstraintCloth()
    {
        for (int i = 0; i < iter; i++)
        {
            foreach(var li in links)
            {
                if (!li.p1 || !li.p2) //broken then skip
                {
                    li.rend.enabled = false; 
                    continue;
                }

                Vector2 p1 = li.p1.pos, p2 = li.p2.pos;
                float dist = (p1-p2).magnitude; float diff = restingDist - dist;

                Vector2 moveP1 = (p2-p1).normalized * (-diff) * 0.5f;
                Vector2 moveP2 = (p1-p2).normalized * (-diff) * 0.5f;

                li.p2.pos += moveP2; 
                li.p1.pos += moveP1; 
            }
        }

        for (int i = 1; i < size.y; i++)
        {
            for (int j = 0; j < size.x; j++)
            {
                if (!points[i][j]) {continue;}
                points[i][j].ForceUpdatePos();
            }
        }

        foreach(var li in links)
        {
            if (!li.p1 || !li.p2) {continue;}

            Vector2 p1 = li.p1.pos, p2 = li.p2.pos;
            LineRenderer rend = li.rend;
            rend.SetPosition(0, p1);
            rend.SetPosition(1, p2);
        }
    }

    private void ApplyClothForce()
    {
        for (int i = 0; i < size.y; i++)
        {
            for (int j = 0; j < size.x; j++)
            {
                if (!points[i][j]) {continue;}
                points[i][j].acceleration = new Vector2(wind, -gravity);
            }
        }
    }
}
