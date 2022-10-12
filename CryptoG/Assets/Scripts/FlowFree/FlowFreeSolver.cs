using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowFreeSolver : MonoBehaviour
{
    #region supporting structs and classes
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

    class cellMoveOptions
    {
        public flowCell cell;
        public List<int> options;
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

        public List<List<flowCell>> CopyCellBoard(Vector2Int boardSize) 
        {
            List<List<flowCell>> tmp = new List<List<flowCell>>();

            for (int i = 0; i < boardSize.y; i++)
            {
                tmp.Add(new List<flowCell>());
                for (int j = 0; j < boardSize.x; j++)
                {
                    tmp[i].Add(cellBoard[i][j]);
                }
            }
            return tmp;
        }
    }
    #endregion

    [Header("References")]
    [SerializeField] private GeneralDisplayBoardESS board;

    
    [Header("Param")]
    [SerializeField] List<flowCellPair> rootCells;
    [SerializeField] private Color uncheckedCells;
    [SerializeField] private float waitTime = 0.1f;
    [SerializeField] private bool atomicSolve = false;

    private Vector2Int boardSize;
    private Dictionary<int, Vector2Int> rootSearch;
    private Dictionary<int, bool> rootConnected;

    private bool solved = false;
    private BoardState solvedState = null;

    private int[] dx = new int[4] {-1, 0, +1, 0};
    private int[] dy = new int[4] {0, +1, 0, -1};

    private void Start()
    {
        InitializeSolver();
    }

    private void InitializeSolver()
    {
        solved = false; 
        var ini = GenerateInitialBoardState();
        DrawFromBoardState(ini);

        if (!atomicSolve) { StartCoroutine(SolveFromState(ini)); }
        else {SolveFromStateQuick(ini); }
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

    private void DrawFromBoardState(BoardState cur)
    {
        for (int i = 0; i < boardSize.y; i++)
        {
            for (int j = 0; j < boardSize.x; j++)
            {
                board.cells[i][j].ChangeToColorState(cur.cellBoard[i][j].color);
            }
        }

        foreach(var root in rootCells)
        {
            board.cells[root.positionFirst.x][root.positionFirst.y].ChangeToColorAndTextState(root.color, "S");
            board.cells[root.positionSecond.x][root.positionSecond.y].ChangeToColorAndTextState(root.color, "E");
        }

        foreach(var cell in cur.currentFlow)
        {
            board.cells[cell.position.x][cell.position.y].ChangeToColorAndTextState(cell.color, "c");
        }
    }

    IEnumerator SolveFromState(BoardState state)
    {
        yield return new WaitForSeconds(waitTime);

        //debug
        /*string f = "";
        foreach (var root in rootConnected)
        {
            f += root.Key.ToString() + " " + root.Value.ToString() + " ; ";
        }
        Debug.LogWarning(f);

        string g = "";
        foreach(var cell in state.currentFlow)
        {
            g += cell.position.ToString() + " ";
        }
        Debug.Log(g);*/

        //check if solved
        if (solved) {yield break;}

        //draw board
        DrawFromBoardState(state);

        List<cellMoveOptions> bestCellSort = GetBestMoveFromState(state);

        foreach(var headCell in bestCellSort)
        {
            //check the best possible current cell moves
            var cell = headCell.cell;
            Vector2Int pos = cell.position;
            foreach (var i in headCell.options) //check available options
            {
                var newPos = new Vector2Int(pos.x + dx[i], pos.y + dy[i]);
                if (state.cellBoard[newPos.x][newPos.y].id != -1)  //not empty
                {
                    if (state.cellBoard[newPos.x][newPos.y].position == rootSearch[state.cellBoard[pos.x][pos.y].id]) //found end node
                    {
                        //Debug.LogError("ROOT: " + state.cellBoard[newPos.x][newPos.y].id.ToString() + " " + rootSearch[state.cellBoard[newPos.x][newPos.y].id].ToString());
                        var tmp = new Dictionary<int, bool>(rootConnected); //tmp
                        rootConnected[cell.id] = true;

                        //create new shit
                        BoardState newState = new BoardState();

                        newState.cellBoard = state.CopyCellBoard(boardSize);
                        newState.currentFlow = new List<flowCell>();

                        foreach(var ce in state.currentFlow)
                        {
                            if (ce.Equals(cell)) {continue;} //terminate
                            else {newState.currentFlow.Add(ce); } 
                        }

                        yield return StartCoroutine(SolveFromState(newState));

                        if (solved) {yield break;}
                        rootConnected = new Dictionary<int, bool>(tmp);
                    }
                    else {continue;}
                }

                if (state.cellBoard[newPos.x][newPos.y].id == -1) //empty
                {
                    //create new shit
                    BoardState newState = new BoardState();

                    newState.cellBoard = state.CopyCellBoard(boardSize);
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

                    if (solved) {yield break; }
                    DrawFromBoardState(state);
                }
            }
        }

        //check if solved
        solved = true;
        foreach (var check in rootConnected)
        {
            if (!check.Value) { solved = false; break;}
        }
        if (solved && (!CheckEmptyCells(state))) { solvedState = state; } //no root not connected && no empty cells
        else {solved = false; }
    }

    private List<cellMoveOptions> GetBestMoveFromState(BoardState state)
    {
        List<cellMoveOptions> cellOptions = new List<cellMoveOptions>();
        foreach(var cell in state.currentFlow)
        {
            cellMoveOptions head = new cellMoveOptions();
            head.cell = cell;
            head.options = new List<int>() {0, 1, 2, 3};
        }

        //remove non-moveable options
        foreach(var head in cellOptions)
        {
            var pos = head.cell.position;
            List<int> newOptions = new List<int>();
            foreach(var dir in head.options)
            {
                var newPos = new Vector2Int(pos.x + dx[dir], pos.y + dy[dir]);

                if (newPos.x >= boardSize.y || newPos.y >= boardSize.x || newPos.x < 0 || newPos.y < 0) {continue;}
                if (state.cellBoard[newPos.x][newPos.y].id != -1) 
                {
                    if (!(state.cellBoard[newPos.x][newPos.y].position == rootSearch[state.cellBoard[pos.x][pos.y].id])) //NOT root node
                    {
                        continue;
                    }
                }

                //check for deadend
                //create a temporary board and make a move in the direction
                BoardState newState = new BoardState();
                flowCell thisCell = head.cell;

                newState.cellBoard = state.CopyCellBoard(boardSize);
                newState.currentFlow = new List<flowCell>();

                var newCell = new flowCell();
                newCell.position = newPos;
                newCell.color = thisCell.color;
                newCell.id = thisCell.id;

                newState.cellBoard[newPos.x][newPos.y]  = newCell;

                //check for deadends with the new board
                //conditions: empty cell with three COMPLETED neighbor (neighbor != current flow(head) of any color || incompleted goal of any color)
                if (CheckIsolatedCell(newState, cellOptions)) {continue;}                

                //all check completed
                newOptions.Add(dir); //if direction moveable, add to (new) options list
            }
            head.options = newOptions; // set new options after removing unmoveable options
        }
        return cellOptions;
    }

    private bool CheckIsolatedCell(BoardState state, List<cellMoveOptions> cellOptions)
    {
        for (int i = 0; i < boardSize.y; i++)
        {
            for (int j = 0; j < boardSize.x; j++)
            {
                var cur = state.cellBoard[i][j];

                             
            }
        }
        return false;
    }

    private bool CheckEmptyCells(BoardState state)
    {
        for (int i = 0; i < boardSize.y; i++)
        {
            for (int j = 0; j < boardSize.x; j++)
            {
                if (state.cellBoard[i][j].id == -1) {return true;}
            }
        }
        return false;
    }

    private void Update()
    {
        if (solved)
        {
            DrawFromBoardState(solvedState);
        }
    }

    private void SolveFromStateQuick(BoardState state) //quicker
    {
        //check if solved
        if (solved) {return;}

        //draw board
        DrawFromBoardState(state);

        //investigate paths
        foreach(var cell in state.currentFlow)
        {
            Vector2Int pos = cell.position;
            for (int i = 0; i < 4; i++)
            {
                var newPos = new Vector2Int(pos.x + dx[i], pos.y + dy[i]);

                if (newPos.x >= boardSize.y || newPos.y >= boardSize.x || newPos.x < 0 || newPos.y < 0) {continue;}

                if (state.cellBoard[newPos.x][newPos.y].id != -1)  //not empty
                {
                    if (state.cellBoard[newPos.x][newPos.y].position == rootSearch[state.cellBoard[pos.x][pos.y].id]) //found end node
                    {
                        var tmp = new Dictionary<int, bool>(rootConnected); //tmp
                        rootConnected[cell.id] = true;

                        //create new shit
                        BoardState newState = new BoardState();

                        newState.cellBoard = state.CopyCellBoard(boardSize);
                        newState.currentFlow = new List<flowCell>();

                        foreach(var ce in state.currentFlow)
                        {
                            if (ce.Equals(cell)) {continue;} //terminate
                            else {newState.currentFlow.Add(ce); } 
                        }

                        SolveFromStateQuick(newState);

                        if (solved) {return;}
                        else if (!solved) 
                        {
                            rootConnected = new Dictionary<int, bool>(tmp);
                        }
                    }
                    else {continue;}
                }

                if (state.cellBoard[newPos.x][newPos.y].id == -1) //empty
                {
                    //create new shit
                    BoardState newState = new BoardState();

                    newState.cellBoard = state.CopyCellBoard(boardSize);
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

                    SolveFromStateQuick(newState);
                    if (solved) {return; }
                    else if (!solved) {DrawFromBoardState(state); }
                }
            }
        }
        solved = true;
        foreach (var check in rootConnected)
        {
            if (!check.Value) { solved = false; break;}
        }
        if (solved && (!CheckEmptyCells(state))) { solvedState = state; } //no root not connected && no empty cells
        else {solved = false; }
    }
}
