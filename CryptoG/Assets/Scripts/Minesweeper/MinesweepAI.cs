using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public struct mineSweepMove
{
    public Vector2Int pos; 
    public int type;

    public mineSweepMove(Vector2Int p, int t) {this.pos = p; this.type = t;}
}

public class MinesweepAI : MonoBehaviour
{

    [Header("Options")]
    [SerializeField] [TextArea(10, 20)] private string inputBoard;
    [SerializeField] private bool useCustomBoard = true;
    [SerializeField] private float delay = 0.1f;

    [Header("References")]
    [SerializeField] private MinesweepBoard board;
    [SerializeField] private MineManagerSweep manager;
    
    private List<List<MineConstraint>> cons = new List<List<MineConstraint>>();
    private List<List<char>> playerVal;
    private List<Vector2Int> borderCells;
    private Dictionary<Vector2Int, Vector2Int> par;
    private List<List<Vector2Int>> estimates;
    private HashSet<mineSweepMove> moves;

    private static string num = ".123456789"; //opened cell, not bomb/flags
    public bool firstTime;
    private int row, col;

    private Dictionary<char, int> valInt = new Dictionary<char, int>() {
        {'.', 0}, {'1', 1},
        {'2', 2}, {'3', 3},
        {'4', 4}, {'5', 5},
        {'6', 6}, {'7', 7},
        {'8', 8}, {'9', 9},
    };

    void Start()
    {
        //AssembleBoard();
        //FindBorder();
        //StartCoroutine(SolveMinesweep());
    }

    public IEnumerator SolveMinesweep()
    {
        GetBoardInfo();

        while (board.stat == "playing")
        {
            playerVal = new List<List<char>>(board.playerVal); //copy some value bruh
            yield return new WaitForSeconds(delay);

            GenerateConstraints();
            FindMoves();

            board.ApplyMove(moves);
        }
        yield return new WaitForSeconds(delay);
        manager.RefreshBoard(false);
    }

    private void FindMoves()
    {
        moves = new HashSet<mineSweepMove>();

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                HashSet<Vector2Int> first = cons[i][j].bombCells;
                int num1 = cons[i][j].bombCount;

                if (!num.Contains(playerVal[i][j])) {continue;}

                //single constraint
                if (first.Count == 0) {continue;}
                else if (first.Count == num1) //all marked neighbor baby!
                {
                    foreach(var pos in first)
                    {
                        moves.Add(new mineSweepMove(pos, 1)); //flagged
                    }
                    continue;
                }
                else if (num1 == 0) //all free neighbor baby!
                {
                    foreach(var pos in first)
                    {
                        moves.Add(new mineSweepMove(pos, 0));
                    }
                    continue;
                }

                //double constraints -> bug-proned
                //Debug.Log("Double baby!");
                for (int m = -2; m <= 2; m++)
                {
                    for (int n = -2; n <= 2; n++)
                    {
                        int newX = i + m, newY = j + n;

                        if (m == 0 && n == 0) {continue;}
                        if (newX < 0 || newY < 0 || newX >= row || newY >= col) {continue;}
                        if (!num.Contains(playerVal[newX][newY])) {continue;}

                        HashSet<Vector2Int> sec = cons[newX][newY].bombCells;
                        int num2 = cons[newX][newY].bombCount;

                        HashSet<Vector2Int> sub;
                        if (sec.IsSubsetOf(first)) //first contains sec
                        {
                            sub = first.Except(sec).ToHashSet();
                            int newVal = num1 - num2;

                            if (sub.Count == newVal) //all bombs then!
                            {
                                foreach(var pos in sub)
                                {
                                    moves.Add(new mineSweepMove(pos, 1));
                                }
                            }
                            else if (newVal == 0) //all free then!
                            {
                                foreach(var pos in sub)
                                {
                                    moves.Add(new mineSweepMove(pos, 0));
                                }
                            }
                        }

                        else if (first.IsSubsetOf(sec)) //sec contains first
                        {
                            sub = sec.Except(first).ToHashSet();

                            int newVal = num2 - num1;

                            if (sub.Count == newVal) //all bombs then!
                            {
                                foreach(var pos in sub)
                                {
                                    moves.Add(new mineSweepMove(pos, 1));
                                }
                            }
                            else if (newVal == 0) //all free then!
                            {
                                foreach(var pos in sub)
                                {
                                    moves.Add(new mineSweepMove(pos, 0));
                                }
                            }
                        }
                    }
                }
            }
        }

        if (moves.Count == 0) //guessing time...
        {
            if (firstTime) {firstTime = false;}
            //else {return;}

            List<mineSweepMove> empty = new List<mineSweepMove>();
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    if (playerVal[i][j] == ' ') //empty
                    {
                        empty.Add(new mineSweepMove(new Vector2Int(i, j), 0));
                    }
                }
            }
            System.Random rng = new System.Random();
            empty = empty.OrderBy(a => rng.Next()).ToList();

            moves.Add(empty[0]);
        }
    }

    private void GetBoardInfo()
    {
        row = board.height; col = board.width;

        playerVal = new List<List<char>>(board.playerVal); //copy some value bruh
        cons = new List<List<MineConstraint>>();

        for (int i = 0; i < row; i++)
        {
            cons.Add(new List<MineConstraint>());
            for (int j = 0; j < col; j++)
            {
                MineConstraint x = new MineConstraint(); x.Initialize();
                cons[i].Add(x);
            }
        }
    }

    private void GenerateConstraints()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                char play = playerVal[i][j];    
                cons[i][j].Initialize();            

                if (num.Contains(play)) //number -> clues!
                {
                    cons[i][j].bombCount = valInt[play];
                    for (int m = -1; m <= 1; m++)
                    {
                        for (int n = -1; n <= 1; n++)
                        {
                            int newX = i + m, newY = j + n;

                            if (m == 0 && n == 0) {continue;}
                            if (newX < 0 || newY < 0 || newX >= row || newY >= col) {continue;}

                            Vector2Int pos = new Vector2Int(newX, newY);
                            char con = playerVal[newX][newY];

                            if (con == ' ') //empty
                            {
                                cons[i][j].bombCells.Add(pos);
                            }
                            else if (con == 'F') //flagged -> decrease count
                            {
                                cons[i][j].bombCount--;
                            }
                        }
                    }
                }
            }
        }
    }

    public void PrintCell(Vector2Int pos)
    {
        //Debug.Log(cons[pos.x][pos.y].bombCells.Count + " " + );
        string outp = "";
        outp += "Bombs : " + cons[pos.x][pos.y].bombCount.ToString() + "\n";

        foreach(var bomb in cons[pos.x][pos.y].bombCells)
        {
            outp += bomb.ToString() + "\n";
        }
        outp += "---------------------------------------------------\n";

        HashSet<Vector2Int> first = new HashSet<Vector2Int>(cons[pos.x][pos.y].bombCells);
        int num1 = cons[pos.x][pos.y].bombCount;

        if (!num.Contains(playerVal[pos.x][pos.y])) { Debug.Log("haha! jokes on you"); return;}

        //single constraint
        if (first.Count == num1) //all marked neighbor baby!
        {
            outp += "Found!";
        }
        //Debug.Log(outp);
    }

    /*
    #region deprecated
    private void FindBorder()
    {
        borderCells = new List<Vector2Int>();
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (playerVal[i][j] != ' ') {continue;} //not empty -> next
                Vector2Int pos = new Vector2Int(i, j);

                bool nei = false;
                for (int m = -1; m <= 1; m++)
                {
                    for (int n = -1; n <= 1; n++)
                    {
                        int x = i+m, y = j+n;
                        if (x < 0 || y < 0 || x >= row || y >= col) {continue;}

                        char c = playerVal[x][y];
                        if (num.Contains(c)) {nei = true; break;}
                    }
                    if (nei) {break;}
                }

                if (nei) {borderCells.Add(pos);} //add bordering cells to opened cells
            }
        }

        //group connected cells
    }

    private void AssembleBoard()
    {
        playerVal = new List<List<char>>();

        if (useCustomBoard)
        {
            inputBoard.Replace("\r", String.Empty); //remove CR

            List<string> li = inputBoard.Split('\n').ToList();
            row = li.Count; col = li[0].Length;

            for (int i = 0; i < row; i++)
            {
                playerVal.Add(new List<char>());
                for (int j = 0; j < col; j++)
                {
                    playerVal[i].Add(li[i][j]);
                }
            }
        }
    }
    #endregion
    */
}
