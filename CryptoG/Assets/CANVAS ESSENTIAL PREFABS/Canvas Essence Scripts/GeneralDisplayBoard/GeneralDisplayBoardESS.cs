using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;

public class GeneralDisplayBoardESS : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject cellPref;

    [Header("Params")]
    [SerializeField] private float originalCellSize;
    [SerializeField] private float cellScale;
    [SerializeField] private float cellSpacingMultiplier = 0.95f;

    [Space(10)]
    [SerializeField] private Vector2Int boardSize;

    public List<List<GeneralDisplayCell>> cells;
    private float cellSize;

    private void Start()
    {
        InitializeGeneralBoardDisplay();
    }

    private void InitializeGeneralBoardDisplay()
    {
        cellSize = originalCellSize * cellScale;
        GenerateCellBoard();
        //ChangeToTextState("Hi");
    }

    private void GenerateCellBoard()
    {
        cells = new List<List<GeneralDisplayCell>>();
        float startX = (boardSize.x % 2 == 0) ? (-boardSize.x / 2.0f + 0.5f) * cellSize : (-boardSize.x / 2.0f) * cellSize;
        float startY = (boardSize.y % 2 == 0) ? (boardSize.y / 2.0f + 0.5f) * cellSize : (boardSize.y / 2.0f) * cellSize;

        for (int i = 0; i < boardSize.y; i++)
        {
            cells.Add(new List<GeneralDisplayCell>());
            for (int j = 0; j < boardSize.x; j++)
            {
                CreateNewCell(new Vector2(startX, startY), new Vector2Int(i, j));
                startX += cellSize;
            }
            startX = (boardSize.x % 2 == 0) ? (-boardSize.x / 2.0f + 0.5f) * cellSize : (-boardSize.x / 2.0f) * cellSize;
            startY -= cellSize;
        }
    }

    private void CreateNewCell(Vector2 pos, Vector2Int idx)
    {
        var newCell = Instantiate(cellPref, Vector3.zero, Quaternion.identity, transform);
        var con = newCell.GetComponent<GeneralDisplayCell>();

        con.SetPosition(pos);
        con.SetScale(cellScale * cellSpacingMultiplier); //have some border between cells with cellSpacingMultiplier!

        cells[idx.x].Add(con);
    }

    public Vector2Int GetBoardSize()
    {
        return boardSize;
    }

    public void ChangeToColorState(Color col)
    {
        for (int i = 0; i < boardSize.x; i++)
        {
            for (int j = 0; j < boardSize.y; j++)
            {
                cells[i][j].ChangeToColorState(col);
            }
        }
    }

    public void ChangeToImageState(Sprite sp)
    {
        for (int i = 0; i < boardSize.x; i++)
        {
            for (int j = 0; j < boardSize.y; j++)
            {
                cells[i][j].ChangeToImageState(sp);
            }
        }
    }

    public void ChangeToTextState(string te)
    {
        for (int i = 0; i < boardSize.x; i++)
        {
            for (int j = 0; j < boardSize.y; j++)
            {
                cells[i][j].ChangeToTextState(te);
            }
        }
    }
}
