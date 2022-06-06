using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerletSpawn : MonoBehaviour
{
    [Header("Options")]
    [SerializeField] private int numObjects = 15;
    [SerializeField] private float scale = 0.1f;
    [SerializeField] private float bounds = 6f;

    [Space(10)]
    [SerializeField] private float maxAccel = 0.1f;
    [SerializeField] private float timeStep = 1f;
    [SerializeField] private bool useDeltaTime = false;
    [SerializeField] private float drag = 0.95f;

    [Space(10)]
    [SerializeField] private bool useCollision = false;

    [Header("References")]
    [SerializeField] private GameObject obj;
    [SerializeField] private VerletCollision collision;
    void Awake()
    {
        if (!useCollision) {collision.enabled = false;}

        for (int i = 0; i < numObjects; i++)
        {
            float x = UnityEngine.Random.Range(-bounds, bounds);
            float y = UnityEngine.Random.Range(-bounds, bounds);

            var n = Instantiate(obj, new Vector2(x, y), Quaternion.identity, transform);
            n.transform.localScale = new Vector3(scale, scale, 1f);

            var move = n.GetComponent<VerletMove>();
            move.SetMaxAccel(maxAccel);
            move.SetBoolTime(useDeltaTime);
            move.SetDrag(drag);
            move.SetTimeStep(timeStep);
        }
    }

    public float GetScale() {return scale;}
    public float GetBounds() {return bounds;}
}
