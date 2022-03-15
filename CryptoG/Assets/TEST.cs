using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST : MonoBehaviour
{
    //string x = "ÅnD";
    void Start()
    {
        /*LineRenderer rend = GetComponent<LineRenderer>();
        MeshCollider meshCol = GetComponent<MeshCollider>();

        Mesh mesh = new Mesh();
        rend.BakeMesh(mesh, true);
        meshCol.sharedMesh = mesh;*/
    }

    private void OnMouseDown() 
    {
        Debug.Log("Bruv");
    }
}
