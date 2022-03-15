using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionLineColliderController : MonoBehaviour
{
    private LineRenderer myRend;
    private MeshCollider myMeshCol;
    [SerializeField] private Color glowColor;

    public void CreateMeshCollider()
    {
        //create mesh collider
        myRend = gameObject.GetComponent<LineRenderer>();
        myMeshCol = gameObject.AddComponent<MeshCollider>();

        Mesh mesh = new Mesh();
        myRend.BakeMesh(mesh, true);
        myMeshCol.sharedMesh = mesh;
    }

    public void OnMouseDown()
    {
        myRend.startColor   = glowColor;
        myRend.endColor     = glowColor;
    }
}
