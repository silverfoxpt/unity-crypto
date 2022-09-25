using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowFreeSolver : MonoBehaviour
{
    [System.Serializable]
    class flowCell
    {
        public Vector2Int position;
        public Color color;

        public int id;
    }

    [System.Serializable]
    struct flowCellPair
    {
        public Vector2Int positionFirst, positionSecond;
        public Color color;

        public int id;
    }

    class BoardState
    {
        public List<List<flowCell>> cellBoard;
        public List<flowCell> currentFlow;

        public void InitializeBoard()
        {
            cellBoard = new List<List<flowCell>>();
            currentFlow = new List<flowCell>();
        }
    }

    [Header("References")]
    [SerializeField] private GeneralDisplayBoardESS board;
    
    [Header("Param")]
    [SerializeField] List<flowCellPair> rootCells;
    [SerializeField] private Color uncheckedCells;

    private Vector2Int boardSize;
    private Dictionary<int, Vector2Int> rootSearch;

    private void Start()
    {
        var ini = GenerateInitialBoardState();
        DrawFromBoardState(ini);
    }

    private void DrawFromBoardState(BoardState cur)
    {
        for (int i = 0; i < boardSize.y; i++)
        {
            for (int j = 0; j < boardSize.x; j++)
            {
                board.cells[i][j].ChangeToColorState(cur.cellBoard[i][j].color);
            }
        }
    }

    private BoardState GenerateInitialBoardState()
    {
        boardSize = board.GetBoardSize();
        rootSearch = new Dictionary<int, Vector2Int>();

        BoardState iniBoard = new BoardState();
        iniBoard.InitializeBoard();

        for (int i = 0; i < boardSize.y; i++)
        {
            iniBoard.cellBoard.Add(new List<flowCell>());
            for (int j = 0; j < boardSize.x; j++)
            {
                var tempCell = new flowCell();
                tempCell.position = new Vector2Int(i, j);
                tempCell.color = uncheckedCells;
                
                iniBoard.cellBoard[i].Add(tempCell);
            }
        }

        foreach(flowCellPair roots in rootCells)
        {
            flowCell ce1 = new flowCell(); ce1.position = roots.positionFirst; ce1.color = roots.color; ce1.id = roots.id;
            flowCell ce2 = new flowCell(); ce2.position = roots.positionSecond; ce2.color = roots.color; ce2.id = roots.id;

            rootSearch.Add(roots.id, roots.positionSecond); //search for roots of each id

            iniBoard.currentFlow.Add(ce1); //ce1 is start, ce2 is end

            iniBoard.cellBoard[ce1.position.x][ce1.position.y] = ce1;
            iniBoard.cellBoard[ce2.position.x][ce2.position.y] = ce2;
        }
        return iniBoard;
    }
}
