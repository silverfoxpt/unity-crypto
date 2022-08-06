using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WaveFunctionCollapse : MonoBehaviour
{
    [Header("Rules")]
    [SerializeField] private List<TileBox> naiveTiles;

    [Header("Params")]
    [SerializeField] private Vector2Int size;
    [SerializeField] private float delay;

    [Header("Ref")]
    [SerializeField] private ImageBoardSpread board;

    private List<TileBox> allTiles;

    private List<List<List<TileBox>>> entropy;

    void Start()
    {
        board.CreateAllImages(size);
        ConstructAllTileset();   

        StartCoroutine(GenerateImage());
    }

    private string ReverseStr(string x)
    {
        string res = "";
        for (int i = x.Length-1; i >= 0; i--)
        {
            res += x[i];
        } return res;
    }

    IEnumerator GenerateImage()
    {
        while (WFCNotFilled())
        {
            board.ClearAllImage(size);

            //populate entropy box
            entropy = new List<List<List<TileBox>>>();

            for (int i = 0; i < size.y; i++)
            {
                entropy.Add(new List<List<TileBox>>());
                for (int j = 0; j < size.x; j++)
                {
                    entropy[i].Add(new List<TileBox>());

                    foreach(var tile in allTiles)
                    {
                        entropy[i][j].Add(tile);
                    }
                }
            }

            //wave function collapse
            while (!CheckComplete())
            {
                //yield return new WaitForSeconds(delay); //slow mode
                
                int minTileCount = int.MaxValue; Vector2Int minTilePos = UtilityFunc.nullVecInt;
                for (int i = 0; i < size.y; i++)
                {
                    for (int j = 0; j < size.x; j++)
                    {
                        if (entropy[i][j].Count > 0 && entropy[i][j].Count < minTileCount)
                        {
                            minTileCount = entropy[i][j].Count;
                        }
                    }
                }

                //get all tiles with least entropy, and choose one randomly
                List<Vector2Int> leastTiles = new List<Vector2Int>();
                for (int i = 0; i < size.y; i++)
                {
                    for (int j = 0; j < size.x; j++)
                    {
                        if (entropy[i][j].Count == minTileCount)
                        {
                            leastTiles.Add(new Vector2Int(i, j));
                        }
                    }
                }
                minTilePos = leastTiles[UnityEngine.Random.Range(0, leastTiles.Count)];

                //get that tile to be opened
                int randIdx = UnityEngine.Random.Range(0, minTileCount);
                board.SetTile(minTilePos, entropy[minTilePos.x][minTilePos.y][randIdx]);

                //collapse for surrounding tiles
                TileBox curTile = entropy[minTilePos.x][minTilePos.y][randIdx];
                int x = minTilePos.x, y = minTilePos.y;

                //upper tile
                if (x > 0)
                {
                    List<TileBox> approved = new List<TileBox>();
                    foreach(var tile in entropy[x-1][y])
                    {
                        if (tile.GetDown() == curTile.GetUp()) { approved.Add(tile); }
                    }
                    entropy[x-1][y] = new List<TileBox>(approved);
                }

                //right tile
                if (y < size.x-1)
                {
                    List<TileBox> approved = new List<TileBox>();
                    foreach(var tile in entropy[x][y+1])
                    {
                        if (tile.GetLeft() == curTile.GetRight()) { approved.Add(tile); }
                    }
                    entropy[x][y+1] = new List<TileBox>(approved);
                }

                //down tile
                if (x < size.y-1)
                {
                    List<TileBox> approved = new List<TileBox>();
                    foreach(var tile in entropy[x+1][y])
                    {
                        if (tile.GetUp() == curTile.GetDown()) { approved.Add(tile); }
                    }
                    entropy[x+1][y] = new List<TileBox>(approved);
                }

                //left tile
                if (y > 0)
                {
                    List<TileBox> approved = new List<TileBox>();
                    foreach(var tile in entropy[x][y-1])
                    {
                        if (tile.GetRight() == curTile.GetLeft()) { approved.Add(tile); }
                    }
                    entropy[x][y-1] = new List<TileBox>(approved);
                }

                entropy[x][y] = new List<TileBox>(); //clearout
            }
            yield return new WaitForSeconds(delay); //fast mode
        }
    }

    private bool WFCNotFilled()
    {
        for (int i = 0; i < size.y; i++)
        {
            for (int j = 0; j < size.x; j++)
            {
                if (!board.GetImg(new Vector2Int(i, j))) {return true;}
            }
        }
        return false;
    }

    private bool CheckComplete()
    {
        for (int i = 0; i < size.y; i++)
        {
            for (int j = 0; j < size.x; j++)
            {
                if (entropy[i][j].Count > 0) {return false;}
            }
        }
        return true;
    }

    private void ConstructAllTileset()
    {
        allTiles = new List<TileBox>();
        foreach(var tile in naiveTiles)
        {
            allTiles.Add(tile);

            var curTile = tile;
            for (int i = 1; i <= 3; i++)
            {
                var newTile = ScriptableObject.CreateInstance<TileBox>();

                newTile.SetSprite(curTile.GetSprite());
                newTile.SetRotate(i);

                newTile.SetRight(curTile.GetUp());
                newTile.SetDown(ReverseStr(curTile.GetRight()));
                newTile.SetLeft(curTile.GetDown());
                newTile.SetUp(ReverseStr(curTile.GetLeft()));

                allTiles.Add(newTile);
                curTile = newTile;
            }
        }
    }
}
