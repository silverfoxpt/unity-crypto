using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineManagerSweep : MonoBehaviour
{
    [SerializeField] private MinesweepAI aiMine;
    [SerializeField] private MinesweepBoard board;
    [SerializeField] private bool runAI = false;
    void Start()
    {
        RefreshBoard(true);
    }

    public void RefreshBoard(bool newBoard)
    {
        board.InitializeBoard(newBoard);
        if (runAI) { StartCoroutine(aiMine.SolveMinesweep());} 
    }
}
