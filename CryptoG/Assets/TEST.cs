using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST : MonoBehaviour
{
    [SerializeField] private Material material;
    void Start()
    {
        GameObject obj = new GameObject("Mesh", typeof(MeshFilter), typeof(MeshRenderer));

        Vector3[] vert = new Vector3[5];
        Vector2[] uv = new Vector2[5];
        int[] triangles = new int[6];

        vert[0] = new Vector3(0, 0);
        vert[1] = new Vector3(1, 0);
        vert[2] = new Vector3(1, 1);
        vert[3] = new Vector3(2, 0);
        vert[4] = new Vector3(1, -2);

        uv[0] = new Vector3(0, 0);
        uv[1] = new Vector3(1, 0);
        uv[2] = new Vector3(1, 1);
        uv[3] = new Vector3(2, 0);
        uv[4] = new Vector3(1, -2);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        triangles[3] = 0;
        triangles[4] = 3;
        triangles[5] = 4;

        Mesh mesh = new Mesh();
        mesh.vertices = vert;
        mesh.uv = uv;
        mesh.triangles = triangles;

        obj.GetComponent<MeshFilter>().mesh = mesh;
        obj.GetComponent<MeshRenderer>().material = material;
    }
}
