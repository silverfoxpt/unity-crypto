using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathMapGenerator : MonoBehaviour
{   
    public Vector2Int size;
    [SerializeField] private int numBlock = 1000;
    [SerializeField] private float multiplier = 8;
    [SerializeField] private MainBoardController board;

    public Dictionary<Vector2Int, List<Vector2Int>> ne;
    private int[] dx = {1, 0, -1, 0, -1, -1, 1, 1};
    private int[] dy = {0, -1, 0, 1, 1, -1, 1, -1};

    private void Start()
    {
        InitializeBoard();
        RandomizeMap();
        GenerateNodeList();
    }

    private void RandomizeMap()
    {
        for (int i = 0; i < numBlock; i++)
        {
            int x = UnityEngine.Random.Range(0, size.x);
            int y = UnityEngine.Random.Range(0, size.y);

            board.board.SetPixelDirect(new Vector2Int(x, y), Color.black, false);
        }
        board.board.imageTex.Apply();
    }

    private void GenerateNodeList()
    {
        ne = new Dictionary<Vector2Int, List<Vector2Int>>();
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                ne.Add(new Vector2Int(i, j), new List<Vector2Int>() {});
                for (int k = 0; k < 8; k++)
                {
                    int newX = i + dx[k], newY = j + dy[k];
                    if (newX < 0 || newY < 0 || newX >= size.x || newY >= size.y) {continue;}
                    if (board.board.GetPixelDirect(new Vector2Int(newX, newY)) == Color.black) {continue;}

                    ne[new Vector2Int(i, j)].Add(new Vector2Int(newX, newY));
                }
            }
        }
    }

    private void InitializeBoard()
    {
        board.SetBoardSize(size);
        board.SetMultiplier(multiplier);

        board.board.CreateNewBoard();
        board.pen.RefreshPen();
        board.pen.allowedDraw = false;
    }
}
