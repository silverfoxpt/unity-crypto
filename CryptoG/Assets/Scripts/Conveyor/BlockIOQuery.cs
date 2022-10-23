using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Conveyor;

public class BlockIOQuery : MonoBehaviour
{
    public Dictionary<Vector2Int, bulkItem> ResourceQueryDict;

    private void Awake()
    {
        ResourceQueryDict = new Dictionary<Vector2Int, bulkItem>();
    }
}
