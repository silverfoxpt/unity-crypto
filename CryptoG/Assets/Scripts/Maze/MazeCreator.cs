using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCreator : MonoBehaviour
{
    [Header("Square related")]
    [SerializeField] private GameObject squarePref;
    [SerializeField] private float scaler;

    [Header("Board related")]
    [SerializeField] private int side = 5;
    private Vector2 offset = new Vector2();
    
    public List<List<GameObject> > squareBoard = new List<List<GameObject>>();

    void Awake()
    {
        RefreshMaze();
    }

    public void RefreshMaze()
    {
        DeleteEverything();
        CalculateOffset();
        InitializeBoard();
        LightDownAllSquare();
    }

    private void DeleteEverything()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void CalculateOffset()
    {
        int halfSide = (int) (side/2.0f);
        offset.x = -halfSide*scaler;
        offset.y = -offset.x;
    }

    private void InitializeBoard()
    {
        squareBoard = new List<List<GameObject>>();

        int i, j;
        for (i = 0; i < side; i++)
        {
            squareBoard.Add(new List<GameObject>());
        }

        for (i = 0; i < side; i++)
        {
            for (j = 0; j < side; j++)
            {
                GameObject newSquare = Instantiate(squarePref, transform);
                newSquare.transform.position = new Vector2(j*scaler, -i*scaler) + offset;

                squareBoard[i].Add(newSquare);
            }
        }
    }

    public void LightDownAllSquare()
    {
        for (int idx = 0; idx < side; idx++)
        {
            for (int j = 0; j < side; j++)
            {
                squareBoard[idx][j].GetComponent<SquareWallController>().LightDownSquare();
            }
        }
    }

    public GameObject GetSquare(int x, int y) {return squareBoard[x][y];}
    public int GetSize() {return side;}

    public int CheckLightUpCell()
    {
        int cnt = 0;
        foreach(var row in squareBoard)
        {
            foreach(var square in row)
            {
                if (square.GetComponent<SquareWallController>().IsLightUp()) {cnt++;}
            }
        }
        return cnt;
    }
}
