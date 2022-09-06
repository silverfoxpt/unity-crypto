using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonoBoardController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject cellPref;

    [Header("Params")]
    [SerializeField] private float originalCellSize;
    [SerializeField] private float cellScale;
    [SerializeField] private Vector2Int boardSize;

    private List<List<NonoCellController>> cells;
    private float cellSize;

    private void Start()
    {
        cellSize = originalCellSize * cellScale;
        GenerateCellBoard();
    }

    private void GenerateCellBoard()
    {
        cells = new List<List<NonoCellController>>();
        float startX = (boardSize.x % 2 == 0) ? (-boardSize.x / 2.0f + 0.5f) * cellSize : (-boardSize.x / 2.0f) * cellSize;
        float startY = (boardSize.y % 2 == 0) ? (boardSize.y / 2.0f + 0.5f) * cellSize : (boardSize.y / 2.0f) * cellSize;

        for (int i = 0; i < boardSize.y; i++)
        {
            cells.Add(new List<NonoCellController>());
            for (int j = 0; j < boardSize.x; j++)
            {
                CreateNewCell(new Vector2(startX, startY), new Vector2Int(i, j));
                startX += cellSize;
            }
            startX = (boardSize.x % 2 == 0) ? (-boardSize.x / 2.0f + 0.5f) * cellSize : (-boardSize.x / 2.0f) * cellSize;
            startY -= cellSize;
        }
    }

    public void CreateNewCell(Vector2 pos, Vector2Int idx)
    {
        var newCell = Instantiate(cellPref, Vector3.zero, Quaternion.identity, transform);
        var con = newCell.GetComponent<NonoCellController>();

        con.SetPosition(pos);
        con.SetScale(cellScale);

        cells[idx.x].Add(con);
    }

    public Vector2Int GetBoardSize()
    {
        return boardSize;
    }
}
