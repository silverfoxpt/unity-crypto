using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TictactoeMinimax : MonoBehaviour
{
    struct tictacMove
    {
        public Vector2Int move;
        public int val;

        public tictacMove(Vector2Int m, int va) {move = m; val = va;}
    }

    [Header("References")]
    [SerializeField] private CellboardBoardController board;

    [Header("Debug")]
    [SerializeField] private int currentPhase = 1; //1 is X, 2 is O

    private static int size = 3;
    private List<List<int>> currentConfigs;

    void Start()
    {
        currentPhase = 0;
        InitializeCurrentConfig();
    }

    private void InitializeCurrentConfig()
    {
        currentConfigs = new List<List<int>>();
        for (int i = 0; i < size; i++)
        {
            currentConfigs.Add(new List<int>());
            for (int j = 0; j < size; j++)
            {
                currentConfigs[i].Add(0);
            }
        }

        //test
        currentPhase = 2;
        currentConfigs[0][0] = 1;
        currentConfigs[0][1] = 2;
        currentConfigs[2][1] = 2;

        ApplyConfig();
    }

    private void ApplyConfig()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                string con = "";
                switch(currentConfigs[i][j])
                {
                    case 0: con = " "; break;
                    case 1: con = "X"; break;
                    case 2: con = "O"; break;
                }

                board.SetCellText(i, j, con);
            }
        }
    }

    private List<List<int>> DeepCopy2D(List<List<int>> ori)
    {
        List<List<int>> res = new List<List<int>>();
        for (int i = 0; i < size; i++)
        {
            List<int> cur = new List<int>(ori[i]);
            res.Add(cur);
        }
        return res;
    }

    public void PlayBestMove()
    {
        var move = MinimaxMove(DeepCopy2D(currentConfigs), true);

        currentConfigs[move.move.x][move.move.y] = currentPhase;
        ApplyConfig();

        currentPhase = FlippedPhase();
    }

    private tictacMove MinimaxMove(List<List<int>> curConfig, bool maximizing)
    {
        //Debug.Log(curConfig.Equals(currentConfigs));
        int curMove = (maximizing) ? currentPhase : FlippedPhase();

        tictacMove bestMove = new tictacMove(UtilityFunc.nullVecInt, (maximizing) ? -2 : 2);
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (curConfig[i][j] == 0)
                {
                    List<List<int>> newConfig  = DeepCopy2D(curConfig);
                    newConfig[i][j] = curMove;

                    int check = CheckCurrentConfig(newConfig);
                    
                    if (check != 2) //game finished
                    {
                        if (check == 1 && currentPhase == 2) {check = -1;}
                        else if (check == -1 && currentPhase == 1) {check = 1;}

                        if (maximizing)
                        {
                            if (check > bestMove.val) {bestMove = new tictacMove(new Vector2Int(i, j), check);}
                        }
                        else
                        {
                            if (check < bestMove.val) {bestMove = new tictacMove(new Vector2Int(i, j), check);}
                        }
                    }
                    else 
                    {
                        if (check == 1 && currentPhase == 2) {check = -1;}
                        else if (check == -1 && currentPhase == 1) {check = 1;}

                        tictacMove bestGenMove = MinimaxMove(new List<List<int>>(newConfig), !maximizing);
                        if (maximizing)
                        {
                            if (bestGenMove.val > bestMove.val) {bestMove = new tictacMove(new Vector2Int(i, j), check);}
                        }
                        else
                        {
                            if (bestGenMove.val < bestMove.val) {bestMove = new tictacMove(new Vector2Int(i, j), check);}
                        }
                    }
                }
            }
        }
        return bestMove;
    }

    private int CheckCurrentConfig(List<List<int>> curConfig)
    {
        int xWin = 0, yWin = 0;

        //horizontal
        if (curConfig[0][0] == curConfig[0][1] && curConfig[0][1] == curConfig[0][2]) {xWin = (curConfig[0][0] == 1) ? 1 : 0; yWin = (curConfig[0][0] == 2) ? 1 : 0;}
        if (curConfig[1][0] == curConfig[1][1] && curConfig[1][1] == curConfig[1][2]) {xWin = (curConfig[1][0] == 1) ? 1 : 0; yWin = (curConfig[1][0] == 2) ? 1 : 0;}
        if (curConfig[2][0] == curConfig[2][1] && curConfig[2][1] == curConfig[2][2]) {xWin = (curConfig[2][0] == 1) ? 1 : 0; yWin = (curConfig[2][0] == 2) ? 1 : 0;}

        //vertical
        if (curConfig[0][0] == curConfig[1][0] && curConfig[1][0] == curConfig[2][2]) {xWin = (curConfig[0][0] == 1) ? 1 : 0; yWin = (curConfig[0][0] == 2) ? 1 : 0;}
        if (curConfig[0][1] == curConfig[1][1] && curConfig[1][1] == curConfig[2][1]) {xWin = (curConfig[0][1] == 1) ? 1 : 0; yWin = (curConfig[0][1] == 2) ? 1 : 0;}
        if (curConfig[0][2] == curConfig[1][2] && curConfig[1][2] == curConfig[2][2]) {xWin = (curConfig[0][2] == 1) ? 1 : 0; yWin = (curConfig[0][2] == 2) ? 1 : 0;}

        //diagonal
        if (curConfig[0][0] == curConfig[1][1] && curConfig[1][1] == curConfig[2][2]) {xWin = (curConfig[0][0] == 1) ? 1 : 0; yWin = (curConfig[0][0] == 2) ? 1 : 0;}
        if (curConfig[0][2] == curConfig[1][1] && curConfig[1][1] == curConfig[2][0]) {xWin = (curConfig[0][2] == 1) ? 1 : 0; yWin = (curConfig[0][2] == 2) ? 1 : 0;}
        
        //check
        if (xWin == 1) {return 1;}
        else if (yWin == 1) {return -1;}
        else
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (curConfig[i][j] == 0) {return 2;}
                }
            }
            return 0;
        }
    }

    private int FlippedPhase() {return (currentPhase == 1) ? 2 : 1;}
}
