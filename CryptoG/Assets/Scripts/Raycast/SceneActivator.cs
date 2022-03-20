using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneActivator : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private List<RectangleDrawer> fields;
    [SerializeField] private List<GameObject> sceneBehave;
    [SerializeField] private float offset = 0.1f;

    void Start()
    {
        DrawRects();
    }

    private void DrawRects()
    {
        int idx = 0;
        foreach (RectangleDrawer rect in fields)
        {
            DrawSingleRect(rect, idx); idx++;
        }
    }

    private void DrawSingleRect(RectangleDrawer curRect, int idx)
    {
        GameObject contain = Instantiate(container, transform);
        contain.GetComponent<ContainerController>().SetSceneBehaviourScripts(sceneBehave[idx]);

        BoxCollider2D col = contain.GetComponent<BoxCollider2D>();
        col.offset = curRect.GetCenterRect();        
        col.size = new Vector2(curRect.GetWidth() - offset, curRect.GetHeight() - offset);
    }
}
