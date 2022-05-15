using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumpsController : MonoBehaviour
{
    [Header("Bumps options")]
    [SerializeField] private GameObject bump;
    [SerializeField] public int level = 12;
    [SerializeField] public int startLevel = 9;
    [SerializeField] public float space = 0.2f;
    [SerializeField] private float size = 0.1f;
    
    public List<List<GameObject>> bumps = new List<List<GameObject>>();

    private void Start()
    {
        bumps = new List<List<GameObject>>();
        Vector2 startPos = new Vector2(-space * (startLevel/2), 0f); 

        for (int i = 0; i < level; i++)
        {
            bumps.Add(new List<GameObject>());
            Vector2 start = startPos + new Vector2(-space * i / 2f, -space * i);
            for (int j = 0; j < startLevel + i; j++)
            {
                bumps[i].Add(CreateBumps(start));
                start += new Vector2(space, 0f);
            }
        }
    }

    private GameObject CreateBumps(Vector2 pos)
    {
        
        var newBump = Instantiate(bump, pos, Quaternion.identity, transform);

        newBump.transform.localScale = new Vector3(size, size, 1f);
        return newBump;
    }
}
