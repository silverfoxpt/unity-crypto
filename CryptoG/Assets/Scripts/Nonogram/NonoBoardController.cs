using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NonoBoardController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject cellPref;

    [Header("Params")]
    [SerializeField] private float originalCellSize;
    [SerializeField] private float cellScale;
    [SerializeField] private Vector2Int boardSize;

    [Header("Descriptions")]
    [SerializeField] private NonogramSolver mainSolver;
    [SerializeField] private float spacing = 0.5f;
    [SerializeField] private float descSize = 50;
    [SerializeField] private GameObject hintObj;

    private List<List<NonoCellController>> cells;
    private float cellSize;

    private void Start()
    {
        cellSize = originalCellSize * cellScale;
        GenerateCellBoard();
        //GenerateHintRows();
    }

    public void GenerateHintRows()
    {
        //columns
        float startXUpper = cells[0][0].GetComponent<RectTransform>().anchoredPosition.x, 
            startYUpper = cells[0][0].GetComponent<RectTransform>().anchoredPosition.y + spacing;
        var constSize = cells[0][0].GetComponent<RectTransform>().sizeDelta.x;
        var constScale = cells[0][0].GetComponent<RectTransform>().localScale;

        for (int i = 0; i < boardSize.x; i++)
        {
            GameObject hint = new GameObject(); 
        
            var rect = hint.AddComponent<RectTransform>();
            var word = hint.AddComponent<TextMeshProUGUI>();

            rect.SetParent(this.transform); 
            rect.localScale = constScale;
            rect.anchoredPosition = new Vector2(startXUpper, startYUpper);           
            rect.sizeDelta = new Vector2(constSize, descSize);

            word.fontSize = 50;
            word.alignment = TextAlignmentOptions.BottomGeoAligned;
            word.color = Color.black;

            foreach (int x in mainSolver.colList[i])
            {
                word.text += x.ToString(); word.text += '\n';
            }

            startXUpper += (cells[0][1].GetComponent<RectTransform>().anchoredPosition.x 
                - cells[0][0].GetComponent<RectTransform>().anchoredPosition.x);
        }

        //rows
        float startXLower = cells[0][0].GetComponent<RectTransform>().anchoredPosition.x - spacing, 
            startYLower = cells[0][0].GetComponent<RectTransform>().anchoredPosition.y;
        //constSize = cells[0][0].GetComponent<RectTransform>().sizeDelta.x;
        //constScale = cells[0][0].GetComponent<RectTransform>().localScale;

        for (int i = 0; i < boardSize.x; i++)
        {
            GameObject hint = new GameObject(); 
        
            var rect = hint.AddComponent<RectTransform>();
            var word = hint.AddComponent<TextMeshProUGUI>();

            rect.SetParent(this.transform); 
            rect.localScale = constScale;
            rect.anchoredPosition = new Vector2(startXLower, startYLower);           
            rect.sizeDelta = new Vector2(descSize, constSize);

            word.fontSize = 50;
            word.alignment = TextAlignmentOptions.MidlineRight;
            word.color = Color.black;

            foreach (int x in mainSolver.rowList[i])
            {
                word.text += x.ToString(); word.text += ' ';
            }

            startYLower += (cells[1][0].GetComponent<RectTransform>().anchoredPosition.y 
                - cells[0][0].GetComponent<RectTransform>().anchoredPosition.y);
        }
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
        con.SetScale(cellScale * .93f); //have some border between cells  -HARDCODE ALERT!!!!

        cells[idx.x].Add(con);
    }

    public Vector2Int GetBoardSize()
    {
        return boardSize;
    }

    //HARDCODING ALEART!!!!!!!!!!!!!!!!!
    public void SetCellsFromBoard(List<List<int>> currentBoard)
    {
        for (int i = 0; i < boardSize.y; i++)
        {
            for (int j = 0; j < boardSize.x; j++)
            {
                switch(currentBoard[i][j])
                {
                    case -1: cells[i][j].SetColor(Color.gray); break;
                    case 0: cells[i][j].SetColor(Color.black); break;
                    case 1: cells[i][j].SetColor(Color.white); break;
                }
            }
        }
    }
}
