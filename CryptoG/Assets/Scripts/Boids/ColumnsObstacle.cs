using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColumnsObstacle : MonoBehaviour
{
    [SerializeField] private float padding = 0.25f;

    public float ObstacleRange()
    {
        return transform.localScale.x + padding;
    }
}
