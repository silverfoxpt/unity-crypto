using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MinesweepBoard : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject cellPref;
    
    [Header("Options")]
    [SerializeField] private Vector2Int size;
    [SerializeField] private float squareSize;
    [SerializeField] private float spacing = 0.02f;

    [Space(10)]
    [SerializeField] private int numBombs = 30;

    private List<List<MineCell>> cells;
    public List<List<char>> playerVal;
    private List<List<int>> realVal;
    private List<List<int>> indicatorVal;
    public int height, width;
    private int curNumBomb = 0;

    public string stat = "playing";
    private int[] dy = new int[4] {+1, 0, -1, 0};
    private int[] dx = new int[4] {0, -1, 0, +1};

    void Start()
    {
        
    }

    #region boardRelated
    public void InitializeBoard(bool createNew = false)
    {
        height = size.y; width = size.x;
        stat = "playing";
        CreateValueList();

        if (createNew) {CreateBoard();}
        else {RefreshBoard();}
    }

    private void CreateValueList()
    {
        if (size.x * size.y < numBombs) { throw new System.Exception("Too many bombs");}
        curNumBomb = numBombs;

        //initialize some list
        playerVal = new List<List<char>>();
        realVal = new List<List<int>>();
        indicatorVal = new List<List<int>>();

        for (int i = 0; i < height; i++)
        {
            playerVal.Add(new List<char>());
            realVal.Add(new List<int>());
            indicatorVal.Add(new List<int>());

            for (int j = 0; j < width; j++)
            {
                playerVal[i].Add(' '); realVal[i].Add(0); indicatorVal[i].Add(0);
            }
        }

        //randomize some bombs
        List<int> bombs = new List<int>();
        for (int i = 0; i < numBombs; i++) {bombs.Add(1);}
        for (int i = 0; i < size.x * size.y - numBombs; i++) {bombs.Add(0);}
        
        System.Random rng = new System.Random();
        bombs = bombs.OrderBy(a => rng.Next()).ToList();

        //assign back to true list
        for (int i = 0; i < size.x * size.y; i++)
        {
            int x = i / size.y, y = i % size.y;
            realVal[x][y] = bombs[i];
            indicatorVal[x][y] = (bombs[i] == 1) ? -1 : 0;
        }

        for (int i = 0; i < size.x * size.y; i++)
        {
            int x = i / size.y, y = i % size.y;

            if (bombs[i] == 1) {continue;} //don't care
            for (int m = -1; m <= 1; m++)
            {
                for (int n = -1; n <= 1; n++)
                {
                    int newX = x + m, newY = y + n;

                    if (m == 0 && n == 0) {continue;}
                    if (newX < 0 || newY < 0 || newX >= height || newY >= width) {continue;}

                    if (realVal[newX][newY] == 1) {indicatorVal[x][y] += 1;}
                }
            }
        }
    }

    private void RefreshBoard()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                cells[i][j].SetText("");
            }
        }
    }

    private void CreateBoard()
    {
        cells = new List<List<MineCell>>();

        float startX = -size.x * squareSize / 2f, startY = size.y * squareSize / 2f;
        for (int i = 0; i < height; i++)
        {
            cells.Add(new List<MineCell>());
            for (int j = 0; j < width; j++)
            {
                var cell = Instantiate(cellPref, new Vector2(startX, startY), Quaternion.identity, transform);

                var comp = cell.GetComponent<MineCell>();
                comp.SetSize(squareSize); 
                comp.SetText("");
                comp.pos = new Vector2Int(i, j);

                cells[i].Add(comp); startX += squareSize + spacing;
            }
            startX = -size.x * squareSize / 2f; startY -= squareSize + spacing;
        }
    }
    #endregion

    #region moveRelated
    public void ProcessClick(int type, Vector2Int pos)
    {
        if (stat == "end") {return;} //game ended

        char player = playerVal[pos.x][pos.y]; // space for empty, num for open, F for flag, B for bomb
        int real = realVal[pos.x][pos.y];
        int indi = indicatorVal[pos.x][pos.y];

        if (type == 0) //left click
        {
            if (player != ' ') {return; } //not empty -> already clicked / flagged
            else if (indi == -1) //bomb, not flagged
            {
                stat = "end"; Debug.Log("lose!");
                StopAllCoroutines();
                playerVal[pos.x][pos.y] = 'B'; 
                cells[pos.x][pos.y].SetText("B");
            }
            else //a value
            {
                string outp = (indi == 0) ? "." : ("" + (char) ('0' + indi)); 
                playerVal[pos.x][pos.y] = (char) ('0' + indi);
                cells[pos.x][pos.y].SetText(outp);

                if (indi == 0) //open the whole 0 area
                {
                    for (int i = 0; i < 4; i++)
                    {
                        int newX = pos.x + dx[i], newY = pos.y + dy[i];
                        if (newX < 0 || newY < 0 || newX >= height || newY >= width) {continue;}

                        ProcessClick(0, new Vector2Int(newX, newY));
                    }
                }
            }
        }
        else if (type == 1) //right click -> only to set/unset flag
        {
            if (player == ' ' && numBombs > 0) //flagging
            {
                playerVal[pos.x][pos.y] = 'F';
                cells[pos.x][pos.y].SetText("F"); 
                curNumBomb--;
            }
            else if (player == 'F') //unflagging
            {
                playerVal[pos.x][pos.y] = ' ';
                cells[pos.x][pos.y].SetText(" ");
                curNumBomb++;
            }
        }

        //check win condition
        if (curNumBomb > 0) {return;}
        Debug.Log("win!"); stat = "end";
    }

    public void ApplyMove(List<mineSweepMove> moves)
    {
        foreach(var move in moves)
        {
            ProcessClick(move.type, move.pos);
            
        }
    }
    #endregion
}
