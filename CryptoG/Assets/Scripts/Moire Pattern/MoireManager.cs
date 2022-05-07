using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MoireManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> screens;
    [SerializeField] private int screenIndex = 0;

    [Header("Options")]
    [SerializeField] private Vector2 offset;
    [SerializeField] [Range(0f, 1f)] private float transparency = 0.50f;
    [SerializeField] [Range(0f, 100f)] private float size = 100;
    [SerializeField] [Range(0f, 360f)] private float rotation = 15f;
    private List<GameObject> dups;

    private void Start()
    {
        InitializeDuplicates();
        DrawScreen();
    }

    private void DrawScreen()
    {
        foreach(var s2 in dups)
        {
            s2.SetActive(false);
        }
        
        var cur = dups[screenIndex];

        cur.SetActive(true);
        cur.transform.position += (Vector3) offset;
        cur.transform.Rotate(new Vector3(0f, 0f, 360f - rotation));
        cur.transform.localScale = new Vector3(size * 0.01f, size * 0.01f, 1f);

        cur.GetComponent<MoireTransparent>().MakeTransparent(transparency);        
    }

    private void InitializeDuplicates()
    {
        dups = new List<GameObject>();
        foreach (var s in screens)
        {
            var s2 = Instantiate(s, new Vector3(0f, 0f, 0f), Quaternion.identity, transform);
            dups.Add(s2);

            s2.SetActive(false);
        }
    }
}
