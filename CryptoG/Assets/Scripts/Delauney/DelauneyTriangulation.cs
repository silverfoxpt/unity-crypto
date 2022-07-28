using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelauneyTriangulation : MonoBehaviour
{
    struct triangle
    {
        public GameObject p1, p2, p3;

        public List<edge> GetEdges()
        {
            List<edge> e = new List<edge>();
            var e1 = new edge(); e1.p1 = p1; e1.p2 = p2; e.Add(e1);
            var e2 = new edge(); e2.p1 = p2; e2.p2 = p3; e.Add(e2);
            var e3 = new edge(); e3.p1 = p3; e3.p2 = p1; e.Add(e3);

            return e;
        }
    }

    struct edge
    {
        public GameObject p1, p2;

        public bool eq(edge b) 
        {
            return (p1 == b.p1 && p2 == b.p2) || (p2 == b.p1 && p1 == b.p2);
        }
    }

    struct circle
    {
        public Vector2 center; public float rad;
    }

    [Header("References")]
    [SerializeField] private GameObject pointPref;

    [Header("Params")]
    [SerializeField] private int numPoint = 50;
    [SerializeField] private int maxRange = 4;

    private List<GameObject> points;
    private List<triangle> triangulation;
    private triangle superTri;

    void Start()
    {
        GeneratePoints();    
        CreateSuperTriangle();
        GenerateDelauneyTriangulation();
    }

    private void CreateSuperTriangle()
    {
        superTri = new triangle();
        var p1 = Instantiate(pointPref, new Vector2(10, 200), Quaternion.identity, transform);
        var p2 = Instantiate(pointPref, new Vector2(-200, -200), Quaternion.identity, transform);
        var p3 = Instantiate(pointPref, new Vector2(200, -250), Quaternion.identity, transform);

        superTri.p1 = p1; superTri.p2 = p2; superTri.p3 = p3;
    }

    private void GenerateDelauneyTriangulation()
    {
        triangulation = new List<triangle>();
        triangulation.Add(superTri);
        
        int c = 0;
        foreach(var po in points)
        {
            c++;

            //find bad triangle
            HashSet<triangle> badTri = new HashSet<triangle>();
            foreach(var tri in triangulation)
            {
                var cir = GetCircumcircle(tri); if (float.IsNaN(cir.center.x)) {Debug.Log(c);}
                if (Vector2.Distance(cir.center, po.transform.position) <= cir.rad) //in circumcircle
                {
                    badTri.Add(tri);
                }
            }

            //save edges
            HashSet<edge> poly = new HashSet<edge>(); List<edge> dups = new List<edge>();
            foreach (var tri in badTri)
            {   
                foreach(var e in tri.GetEdges()) { dups.Add(e); }
            }

            foreach(var e in dups)
            {
                if (dups.FindAll(ed => ed.eq(e)).Count == 1) { poly.Add(e); } //if edge isn't duplicated, save it save it!
            }

            //removal bad triangles
            foreach(var tri in badTri)
            {
                triangulation.Remove(tri);
            }

            //re triangulate
            foreach(var e in poly)
            {
                var newTri = new triangle(); 
                newTri.p1 = e.p1; newTri.p2 = e.p2; newTri.p3 = po;

                triangulation.Add(newTri);
            }
        }

        //remove the super triangle
        List<triangle> final = new List<triangle>();
        foreach (var tri in triangulation)
        {
            if (tri.p1 == superTri.p1 || tri.p2 == superTri.p1 || tri.p3 == superTri.p1) {continue;}
            if (tri.p1 == superTri.p2 || tri.p2 == superTri.p2 || tri.p3 == superTri.p2) {continue;}
            if (tri.p1 == superTri.p3 || tri.p2 == superTri.p3 || tri.p3 == superTri.p3) {continue;}

            final.Add(tri);
        }

        //draw
        foreach(var tri in final)
        {
            foreach(var e in tri.GetEdges())
            {
                Debug.DrawLine(e.p1.transform.position, e.p2.transform.position, Color.green, Time.deltaTime);
            }
        }
    }

    private void Update()
    {
        GenerateDelauneyTriangulation();
    }

    private void GeneratePoints()
    {
        points = new List<GameObject>();
        for (int i = 0; i < numPoint; i++)
        {
            float px = UnityEngine.Random.Range(-maxRange - 0.01f, maxRange + 0.01f);
            float py = UnityEngine.Random.Range(-maxRange - 0.01f, maxRange + 0.01f);

            var newPoint = Instantiate(pointPref, new Vector2(px, py), Quaternion.identity, transform);
            points.Add(newPoint);
        }
    }

    #region helperFunc
    private circle GetCircumcircle(triangle tri)
    {
        Vector2 M = tri.p1.transform.position, N = tri.p2.transform.position, P = tri.p3.transform.position;
        Vector2 mn = LineFromPoints(M, N);
        Vector2 mp = LineFromPoints(M, P);

        Vector2 A = (M+N) / 2, B = (M+P) / 2;

        float a3 = -1 / mn.x, b3 = A.y - a3 * A.x; Vector2 ao = new Vector2(a3, b3);
        float a4 = -1 / mp.x, b4 = B.y - a4 * B.x; Vector2 bo = new Vector2(a4, b4);
        
        Vector2 center = LineIntersect(ao, bo); 
        float radius = Vector2.Distance(center, M);

        var cir = new circle(); cir.center = center; cir.rad = radius;
        return cir;
    }

    private Vector2 LineIntersect(Vector2 a, Vector2 b)
    {
        float x = (b.y - a.y) / (a.x - b.x);
        float y = a.x * x + a.y;

        return new Vector2(x, y);
    }

    private Vector2 LineFromPoints(Vector2 p1, Vector2 p2)
    {
        float a = (p1.y - p2.y) / (p1.x - p2.x);
        float b = (p1.y - p1.x * a);

        return new Vector2(a, b);
    }
    #endregion
}
