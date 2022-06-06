using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerletSpawn : MonoBehaviour
{
    [SerializeField] private int numObjects = 15;
    [SerializeField] private float scale = 0.1f;
    [SerializeField] private GameObject obj;
    void Awake()
    {
        for (int i = 0; i < numObjects; i++)
        {
            float x = UnityEngine.Random.Range(-4f, 4f);
            float y = UnityEngine.Random.Range(-4f, 4f);

            var n = Instantiate(obj, new Vector2(x, y), Quaternion.identity, transform);
            n.transform.localScale = new Vector3(scale, scale, 1f);
        }
    }
}
