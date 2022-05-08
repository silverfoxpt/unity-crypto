using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessController : MonoBehaviour
{
    private BoardController boardController;

    private void Awake()
    {
        boardController = FindObjectOfType<BoardController>();
    }

    private void Start()
    {
        boardController.InitializeBoard();
        FENNotation.LoadBoardFromFen();
    }
}
