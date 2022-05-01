using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Jobs;
using Unity.Collections;

public class TEST : MonoBehaviour
{

    [SerializeField] Tilemap tilemap;
    [SerializeField] Tile tile;

    [Space(15)]
    [SerializeField] private float hexSize = 0.1f;

    [Space(15)]
    [SerializeField] private int hexLevel = 20;
    [SerializeField] private int perLevel = 20;

    private float hexWidth, hexHeight;
    [SerializeField] private float delay = 0.2f;

    Vector3Int x;

    private void Start()
    {
        StartCoroutine(CreatBoard());
    }

    IEnumerator CreatBoard()
    {
        tilemap.transform.localScale = new Vector3(hexSize, hexSize, 1f);
        hexHeight = hexSize; hexWidth = hexHeight/2f * Mathf.Sqrt(3f);

        bool goLeft = true; Vector2 original = new Vector2(0f, 0f);
        original += new Vector2(-hexWidth*perLevel/2f, hexLevel*hexHeight*0.75f/2f);

        //create board
        for (int i = 0; i < hexLevel; i++)
        {
            yield return new WaitForSeconds(delay);
            original = (goLeft) ? GetBottomLeftPos(original) : GetBottomRightPos(original); goLeft = !goLeft; Vector2 cur = original;
            for (int j = 0; j < perLevel; j++)
            {
                x = tilemap.WorldToCell(cur);

                tilemap.SetTile(x, tile);
                tilemap.SetTileFlags(x, TileFlags.None);
                tilemap.SetColor(x, UtilityFunc.GetRandColor());

                cur = GetRightPos(cur);
            }
        }
    }

    private Vector2 GetTopRightPos(Vector2 pos) { return new Vector2(pos.x + hexWidth/2, pos.y + hexHeight*0.75f); }
    private Vector2 GetTopLeftPos(Vector2 pos) { return new Vector2(pos.x - hexWidth/2, pos.y + hexHeight*0.75f); }

    private Vector2 GetLeftPos(Vector2 pos) { return new Vector2(pos.x - hexWidth, pos.y); }
    private Vector2 GetRightPos(Vector2 pos) { return new Vector2(pos.x + hexWidth, pos.y); }

    private Vector2 GetBottomRightPos(Vector2 pos) { return new Vector2(pos.x + hexWidth/2, pos.y - hexHeight*0.75f); }  
    private Vector2 GetBottomLeftPos(Vector2 pos) { return new Vector2(pos.x - hexWidth/2, pos.y - hexHeight*0.75f); }
}
