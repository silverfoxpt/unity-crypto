using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BlockPlaceDict : MonoBehaviour
{
    [SerializeField] private Dictionary<TileBase, int> tileDictionary;

    public int GetTileID(Tilemap map, Vector2Int pos)
    {
        TileBase tile = map.GetTile((Vector3Int) pos);
        return tileDictionary[tile];
    }
}
