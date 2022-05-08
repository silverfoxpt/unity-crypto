using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Board 
{
    public static int[,] board;

    static Board()
    {
        board = new int[8,8];

        //board[0,0] = Piece.White | Piece.Bishop; //test
        //board[4,4] = Piece.Black | Piece.King; //test
    }
}
