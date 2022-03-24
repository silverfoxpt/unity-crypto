    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AreaFillSceneFive : MonoBehaviour
{
    [SerializeField] private GameObject dotPref;
    [SerializeField] private GameObject linePref;
    [SerializeField] private Color lineColor;
    [SerializeField] private GameObject bounder;
    [SerializeField] private GameObject sceneControl;
    [SerializeField] private Vector2 ownOffset = new Vector2(1f, 0f);
    [SerializeField] private Material mat;

    private GameObject dot = null, connectionLine = null;
    private List<LineRenderer> lines = new List<LineRenderer>();
    private Vector2 mainOffset;
    private int numLine;
    private float e = 0.000001f;
    private List<Vector3> vertices;
    private List<Vector2> uvs;
    private List<int> triangles;
    private GameObject meshObj; 
    private Mesh curMesh;

    private void Awake() { InstantiateScene(); }

    private void InstantiateScene()
    {
        //do mesh
        curMesh = new Mesh();
        meshObj = new GameObject("Mesh", typeof(MeshFilter), typeof(MeshRenderer));
        meshObj.GetComponent<MeshFilter>().mesh = curMesh;
        meshObj.GetComponent<MeshRenderer>().material = mat;
        meshObj.transform.parent = transform;

        mainOffset = sceneControl.GetComponent<MainSceneController>().offset;
        dot = Instantiate(dotPref,
            bounder.GetComponent<RectangleDrawer>().GetCenterRect() + mainOffset + ownOffset,
            Quaternion.identity, transform);

        FindNumLines();

        //spawn lines
        for (int idx = 0; idx < numLine; idx++)
        {
            connectionLine = Instantiate(linePref, new Vector3(0f, 0f, 0f), Quaternion.identity, transform);

            var rend            = connectionLine.GetComponent<LineRenderer>();
            rend.startColor     = lineColor;
            rend.endColor       = lineColor;
            rend.positionCount  = 2;
            
            lines.Add(rend);
        }

        gameObject.SetActive(false);
    }

    private void FindNumLines()
    {
        List<singleLine> lines = sceneControl.GetComponent<MainSceneController>().GetAllSingleLines();
        numLine = lines.Count * 6;
    }

    private void TEST()
    {
        float x1 = -1, y1 = 2;
        float x2 = 2, y2 = -4;

        float ax = ((float) (y1-y2))/(x1-x2);
        float bx = (float) y2 - ax*x2;
    }

    private void Update()
    {
        dot.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        triangles = new List<int>();
        uvs = new List<Vector2>();
        vertices = new List<Vector3>();

        DrawMouseLine();
    }

    private void DrawMouseLine()
    {
        Vector2 p1 = dot.transform.position;
        List<singleLine> allLine = sceneControl.GetComponent<MainSceneController>().GetAllSingleLines();

        int idx = 0;
        foreach (singleLine line in allLine)
        {
            float x1 = line.firstPoint.x, y1 = line.firstPoint.y;
            float x2 = line.firstPoint.x, y2 = line.firstPoint.y;

            DrawLine(x1, y1, idx, p1); idx++;
            DrawLine(x1 - e, y1 - e, idx, p1); idx++;
            DrawLine(x1 + e, y1 + e, idx, p1); idx++;

            DrawLine(x2, y2, idx, p1); idx++;
            DrawLine(x2 - e, y2 - e, idx, p1); idx++;
            DrawLine(x2 + e, y2 + e, idx, p1); idx++;
        }

        RebuildMesh(p1);

    }

    private void RebuildMesh(Vector2 p1)
    {
        curMesh.Clear();
        vertices = vertices.OrderBy(x => Mathf.Atan2(x.x - p1.x, x.y - p1.y)).ToList();
        vertices.Insert(0, p1);//origin

        foreach(var tt in vertices) {uvs.Add(tt);}
        for (int idx = 1; idx < vertices.Count - 1; idx++)
        {
            triangles.Add(0);
            triangles.Add(idx);
            triangles.Add(idx + 1);
        }
        triangles.Add(0);
        triangles.Add(vertices.Count - 1);
        triangles.Add(1);

        curMesh.vertices = vertices.ToArray();
        curMesh.uv = uvs.ToArray();
        curMesh.triangles = triangles.ToArray();
    }

    private void DrawLine(float curX, float curY, int idx, Vector2 p1)
    {
        Vector2 p2 = GetClosestPointOfRay(new Vector2(curX, curY));
        lines[idx].SetPosition(0, p1);
        lines[idx].SetPosition(1, p2);
        vertices.Add(p2);
    }

    private Vector2 GetClosestPointOfRay(Vector2 newPoint)
    {
        float x1m = dot.transform.position.x;
        float y1m = dot.transform.position.y;

        float x2m = newPoint.x;
        float y2m = newPoint.y;

        float ax2 = ((float) (y1m-y2m))/( (float) x1m-x2m);
        float bx2 = (float) y2m - ax2*x2m;

        actualLine sec = new actualLine(ax2, bx2, x1m, y1m);
        List<singleLine> lines = sceneControl.GetComponent<MainSceneController>().GetAllSingleLines();

        float minDist = float.MaxValue; Vector2 res = new Vector2(-1f, -1f);

        foreach(var sing in lines)
        {
            float x1 = sing.firstPoint.x, y1 = sing.firstPoint.y;
            float x2 = sing.secPoint.x, y2 = sing.secPoint.y;

            float ax = ((float) (y1-y2))/(x1-x2);
            float bx = (float) y2 - ax*x2;
           
            actualLine first = new actualLine(ax, bx, x1, y1); 
            Vector2 inter = sec.intersec(first);

            if (!PointCalculations.onSameSide(dot.transform.position, new Vector2(x2m, y2m), inter)) { continue; } //not same side, skip
            if (!PointCalculations.onLine(new Vector2(x1, y1), new Vector2(x2, y2), inter)) { continue; } //not on line, skip
            
            float di = PointCalculations.dist(inter, dot.transform.position);         
            if (di < minDist) { minDist = di; res = inter; }
        }
        return res;
    }
}

