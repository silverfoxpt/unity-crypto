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
    struct constraint
    {
        public HashSet<Vector2Int> tiles;
        public int num;

        public void Star() {this.num = 0; this.tiles = new HashSet<Vector2Int>(); }
        public void ModNum(int n) {this.num = n; }
        public void AddTile(Vector2Int t) {tiles.Add(t);}

        public constraint(int n) {this.num  = n; this.tiles = new HashSet<Vector2Int>();}
    }

    [Header("Options")]
    [SerializeField] [TextArea(10, 20)] private string inputBoard;
    [SerializeField] private bool useCustomBoard = true;
    [SerializeField] private float delay = 0.1f;

    [Header("References")]
    [SerializeField] private MinesweepBoard board;
    [SerializeField] private MineManagerSweep manager;
    
    private List<List<constraint>> cons = new List<List<constraint>>();
    private List<List<char>> playerVal;
    private List<Vector2Int> borderCells;
    private Dictionary<Vector2Int, Vector2Int> par;
    private List<List<Vector2Int>> estimates;
    private List<mineSweepMove> moves;

    private static string num = ".123456789"; //opened cell, not bomb/flags
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
        moves = new List<mineSweepMove>();

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                HashSet<Vector2Int> first = cons[i][j].tiles;
                int num1 = cons[i][j].num;

                if (!num.Contains(playerVal[i][j])) {continue;}

                //single constraint
                if (first.Count == 0) {continue;}
                if (first.Count == num1) //all marked neighbor baby!
                {
                    foreach(var pos in first)
                    {
                        moves.Add(new mineSweepMove(pos, 1)); //flagged
                    }
                    continue;
                }
                if (num1 == 0) //all free neighbor baby!
                {
                    Debug.Log("All free : " + i.ToString() + " " + j.ToString() + " " + num1.ToString());
                    foreach(var pos in first)
                    {
                        moves.Add(new mineSweepMove(pos, 0));
                    }
                    continue;
                }

                //double constraints -> bug-proned
                /*for (int m = -2; m <= 2; m++)
                {
                    for (int n = -2; n <= 2; n++)
                    {
                        int newX = i + m, newY = j + n;

                        if (m == 0 && n == 0) {continue;}
                        if (newX < 0 || newY < 0 || newX >= row || newY >= col) {continue;}

                        HashSet<Vector2Int> sec = cons[newX][newY].tiles;
                        int num2 = cons[newX][newY].num;

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
                }*/
            }
        }

        if (moves.Count == 0) //guessing time...
        {
            Debug.Log("rand1");
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
        cons = new List<List<constraint>>();

        for (int i = 0; i < row; i++)
        {
            cons.Add(new List<constraint>());
            for (int j = 0; j < col; j++)
            {
                constraint x = new constraint(); x.Star();
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

                if (num.Contains(play)) //number -> clues!
                {
                    //Debug.LogWarning(new Vector2Int(i, j) + " " + cons[i][j].num.ToString() + " " + valInt[play].ToString());

                    cons[i][j] = new constraint(valInt[play]);
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
                                cons[i][j].AddTile(pos);
                            }
                        }
                    }
                }
            }
        }
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
