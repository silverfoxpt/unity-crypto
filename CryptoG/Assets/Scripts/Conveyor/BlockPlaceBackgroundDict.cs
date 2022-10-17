using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
struct DisplayDictElement
{
    public TileBase tile;
    public int id;
}

public class BlockPlaceBackgroundDict : MonoBehaviour
{
    [SerializeField] private List<DisplayDictElement> tileIDDictionary;

    public int GetTileID(Tilemap map, Vector2Int pos)
    {
        TileBase tile = map.GetTile((Vector3Int) pos);
        if (!tile) {return -1;}
        
        for (int i = 0; i < tileIDDictionary.Count; i++)
        {
            var cur = tileIDDictionary[i];
            if (cur.tile == tile) {return cur.id; }
        }
        return -1;
    }
}
