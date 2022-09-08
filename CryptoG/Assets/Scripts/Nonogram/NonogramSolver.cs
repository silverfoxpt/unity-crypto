using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NonogramSolver : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] [TextArea(10, 20)] private string upper;
    [SerializeField] [TextArea(10, 20)] private string lefter;



    private List<List<int>> colList, rowList;
    private List<List<int>> confirmedLines;

    private Vector2Int boardSize;

    private NonoBoardController boardController;
    
    private void Awake()
    {
        boardController = FindObjectOfType<NonoBoardController>();
    }

    private void Start()
    {
        InitializeInputModules();
        InitializeConfirmedLines();

        TEST_LINESOLVE();
    }

    private void TEST_LINESOLVE()
    {
        InitializeLineSolver(0, 1);

        DebugList(confirmedLines[0]);
    }

    private void InitializeConfirmedLines()
    {
        boardSize = boardController.GetBoardSize();
        confirmedLines = new List<List<int>>();

        for (int i = 0; i < boardSize.x; i++)
        {
            confirmedLines.Add(new List<int>());
            for (int j = 0; j < boardSize.y; j++)
            {
                confirmedLines[i].Add(-1);
            }
        }
    }

    private void InitializeInputModules()
    {
        colList = new List<List<int>>(); rowList = new List<List<int>>();

        //upper first
        List<string> lines = upper.Split('\n').ToList();
        foreach(var line in lines)
        {
            List<int> sp = new List<int>();
            foreach(var x in line.Split(' ')) { sp.Add(int.Parse(x)); }
            colList.Add(sp);
        }

        //lefter latter
        lines = lefter.Split('\n').ToList();
        foreach(var line in lines)
        {
            List<int> sp = new List<int>();
            foreach(var x in line.Split(' ')) { sp.Add(int.Parse(x)); }
            rowList.Add(sp);
        }
    }

    #region lineSolver
    private int[,] solved; //dynamic programming baby!
    private List<int> desc; //description of line (blocks' sizes)
    private List<int> partial; //old coloring
    private List<int> newPartial; //new coloring (to be found, if any)
    private int currentK;

    private void InitializeLineSolver(int idx, int dir)
    {
        //confirmedLines[0][1] = 0; //test

        int n = (dir == 1) ? boardSize.y : boardSize.x;

        newPartial = new List<int>(); newPartial.Add(-1000000000); //keepsake
        for (int i = 1; i <= n; i++) {newPartial.Add(-1);}

        if (dir == 1) //horizontal
        {
            desc = rowList[idx]; 
            partial = new List<int>(confirmedLines[idx]); partial.Insert(0, -1000000000);
        } 
        else //vertical
        {
            desc = colList[idx]; 

            partial = new List<int>(); partial.Add(-1000000000);
            for (int i = 0; i < boardSize.x; i++)
            {
                partial.Add(confirmedLines[i][idx]);
            }
        }
        int k = desc.Count; desc.Insert(0, -1000000000); currentK = k;

        solved = new int[n+1,k+1];
        for (int i = 0; i <= n; i++)
        {
            for (int j = 0; j <= k; j++)
            {
                solved[i,j] = -1;
            }
        }
        solved[0, 0] = 1;

        //test
        //DebugList(desc);
        //DebugList(partial);
        //DebugList(newPartial);

        //solve
        /*for (int i = 0; i <= n; i++)
        {
            for (int j = 0; j <= k; j++)
            {
                SolveLine(i, j);
            }
        }*/
        SolveLine(n, k);

        newPartial.RemoveAt(0); //remove the keepsake
        partial.RemoveAt(0);
        desc.RemoveAt(0);

        //Debug.LogWarning(solved[8, 1]);

        //reupdate
        for (int i = 0; i < n; i++)
        {
            if (partial[i] != -1 || newPartial[i] == 2) {continue;} //skip
            if (dir == 1)
            {
                confirmedLines[idx][i] = newPartial[i];
            }
            else if (dir == 2)
            {
                confirmedLines[i][idx] = newPartial[i];
            }
        }

        boardController.SetCellsFromBoard(confirmedLines);
    }

    private bool CanPlaceBlock(int i, int j) //cell 1->i, block j
    {
        int lval = (j > 1 || (j == 1 && i - desc[j] > 0)) ? 1 : 0;
        for (int m = i; m > i - desc[j]; m--)
        {
            if (partial[m] == 1) {return false;} //if confirmed white, can't place block j
        }

        if (lval == 1 && partial[i - desc[j]] == 0) //not the first block && there isn't a white space to place (is black)
        {
            return false;
        }
        return true;
    }

    private void UpdateCellColor(int idx, int newColor, int j)
    {   
        //if (j < currentK) {return;} //if not the final stretch, ignore

        if (newPartial[idx] == -1) {newPartial[idx] = newColor;}
        else if (newPartial[idx] != newColor) {newPartial[idx] = 2;} //conflict, ignore this cell when updating the main board (confirmedLines)
        //else nothing, because then the cell is already that color
    }   

    //white = 1, black = 0;
    private int SolveLine(int i, int j) 
    {
        int lval = (j > 1 || (j == 1 && i - desc[j] > 0)) ? 1 : 0; //not the first block OR is the first block and not completely to the left (i - desc[j] > 0)

        if (i < 0 || j < 0) {return 0;}
        if (i == 0 && j == 0) {return 1;}

        if (solved[i,j] != -1) {return solved[i,j];}
        else
        {
            solved[i,j] = 0;
            if (SolveLine(i-1, j) > 0 && partial[i] != 0) //cell not black
            {
                UpdateCellColor(i, 1, j); //cell is white
                solved[i,j] = solved[i,j] + SolveLine(i-1, j);
            }
            if (SolveLine(i - desc[j] - lval, j-1) > 0 && CanPlaceBlock(i, j))
            {
                //color the whole block
                for (int m = i; m > i - desc[j]; m--)
                {
                    UpdateCellColor(m, 0, j); //block is black
                }
                if (lval == 1) {UpdateCellColor(i - desc[j], 1, j);} //if cell before the (black) block needs to be white (lval = 1), then so be it

                solved[i,j] = solved[i,j] + SolveLine(i - desc[j] - lval, j-1);
            }

            //Debug.Log("solved " + i.ToString() + " " + j.ToString() + " : " + solved[i,j].ToString() + " lval = " + lval.ToString());
            //DebugList(newPartial);

            return solved[i,j];
        }
    }
    #endregion

    private void DebugList(List<int> c)
    {
        //test
        string x = "";
        for (int i = 0; i < c.Count; i++)
        {
            x += c[i].ToString() + " ";
        }
        Debug.Log(x);
    }
}
