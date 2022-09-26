using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowFreeSolver : MonoBehaviour
{
    [System.Serializable]
    struct flowCell
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
    [SerializeField] private float waitTime = 0.1f;

    private Vector2Int boardSize;
    private Dictionary<int, Vector2Int> rootSearch;
    private Dictionary<int, bool> rootConnected;

    private bool solved = false;
    private BoardState solvedState = null;

    private int[] dx = new int[4] {-1, 0, +1, 0};
    private int[] dy = new int[4] {0, +1, 0, -1};

    private void Start()
    {
        solved = false;
        var ini = GenerateInitialBoardState();
        DrawFromBoardState(ini);

        StartCoroutine(SolveFromState(ini));
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
        rootConnected = new Dictionary<int, bool>();

        BoardState iniBoard = new BoardState();
        iniBoard.InitializeBoard();

        foreach (var root in rootCells)
        {
            rootConnected.Add(root.id, false);
        }

        for (int i = 0; i < boardSize.y; i++)
        {
            iniBoard.cellBoard.Add(new List<flowCell>());
            for (int j = 0; j < boardSize.x; j++)
            {
                var tempCell = new flowCell();
                tempCell.position = new Vector2Int(i, j);
                tempCell.color = uncheckedCells;
                tempCell.id = -1;
                
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

    IEnumerator SolveFromState(BoardState state)
    {
        string f = "";
        for (int i = 0; i < boardSize.y; i++)
        {
            for (int j = 0; j < boardSize.x; j++)
            {
                f += state.cellBoard[i][j].id.ToString() + " ";
            }
            f += "\n";
        }
        Debug.LogWarning(f);

        yield return new WaitForSeconds(waitTime);
        DrawFromBoardState(state);

        if (!CheckBoardStateFind(state)) { yield return null; }
        if (solved) { yield return null; }

        foreach(var cell in state.currentFlow)
        {
            Vector2Int pos = cell.position;
            for (int i = 0; i < 4; i++)
            {
                var newPos = new Vector2Int(pos.x + dx[i], pos.y + dy[i]);

                if (newPos.x >= boardSize.y || newPos.y >= boardSize.x || newPos.x < 0 || newPos.y < 0) {continue;}
                if (state.cellBoard[newPos.x][newPos.y].id != -1)  //not empty
                {
                    if (state.cellBoard[newPos.x][newPos.y].position == rootSearch[state.cellBoard[newPos.x][newPos.y].id]) //found end node
                    {
                        var tmp = new Dictionary<int, bool>(rootConnected); //tmp
                        rootConnected[cell.id] = true;

                        //create new shit
                        BoardState newState = new BoardState();

                        newState.cellBoard = new List<List<flowCell>>(state.cellBoard);
                        newState.currentFlow = new List<flowCell>();

                        foreach(var ce in state.currentFlow)
                        {
                            if (ce.Equals(cell)) {continue;} //terminate
                            else {newState.currentFlow.Add(ce); } 
                        }
                        
                        yield return StartCoroutine(SolveFromState(newState));
                        if (!solved) 
                        {
                            DrawFromBoardState(state);
                            rootConnected = new Dictionary<int, bool>(tmp);
                        }
                    }
                }
                else
                {
                    //create new shit
                    BoardState newState = new BoardState();

                    newState.cellBoard = new List<List<flowCell>>(state.cellBoard);
                    newState.currentFlow = new List<flowCell>();

                    var newCell = new flowCell();
                    newCell.position = newPos;
                    newCell.color = cell.color;
                    newCell.id = cell.id;

                    newState.cellBoard[newPos.x][newPos.y] = newCell;
                    foreach(var ce in state.currentFlow)
                    {
                        if (ce.Equals(cell)) {newState.currentFlow.Add(newCell);}
                        else {newState.currentFlow.Add(ce); }
                    }

                    yield return StartCoroutine(SolveFromState(newState));
                    if (!solved) {DrawFromBoardState(state); }
                }
            }
        }
    }

    private bool CheckBoardStateFind(BoardState state)
    {
        for (int i = 0; i < boardSize.y; i++)
        {
            for (int j = 0; j < boardSize.x; j++)
            {
                if (state.cellBoard[i][j].id == -1) {return true;}
            }
        }

        //check if all root connected
        foreach (var p in rootConnected)
        {
            if (!p.Value) {return false;}
        }

        solved = true; solvedState = state;
        return true;
    }

    private void Update()
    {
        if (solved)
        {
            Debug.Log("yarr");
            DrawFromBoardState(solvedState);
        }
    }
}
