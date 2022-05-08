using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class PieceMoveGenerator 
{
    private static List<Vector2Int> verHori = new List<Vector2Int>() {
        new Vector2Int(0, 1),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0),
    };

    private static List<Vector2Int> diagonal = new List<Vector2Int>() {
        new Vector2Int(1, 1),
        new Vector2Int(-1, -1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, 1),
    };

    private static List<Vector2Int> combo = new List<Vector2Int>();

    public static List<Vector2Int> GetMoves(Vector2Int pos)
    {
        if (combo.Count == 0) {InitializeCombo(); }
        
        List<Vector2Int> moves = new List<Vector2Int>(); 
        int info = Board.board[pos.x, pos.y];
        int curColor = FENNotation.MoveOrder();

        if (!Piece.IsColor(info, curColor)) {return moves;} //incorrect side, empty

        if (Piece.IsType(info, Piece.Rook))
        {
            foreach(Vector2Int move in verHori) //rooks move horizontally and vertically
            {
                Vector2Int tmp = pos;
                for (int i = 0; i < 8; i++)
                {
                    tmp += move; if (OutOfBounds(tmp)) {break;}
                    int curPlaceInfo = Board.board[tmp.x, tmp.y];

                    if (curPlaceInfo != Piece.None) // something there
                    {
                        if (!Piece.IsColor(curPlaceInfo, curColor)) { moves.Add(tmp); break; } //eatable, break
                        else {break;} // our side, break
                    }
                    else { moves.Add(tmp); } //nothing there
                }
            }
        }
        else if (Piece.IsType(info, Piece.Bishop))
        {
            foreach(Vector2Int move in diagonal) //bishops moves diagonally
            {
                Vector2Int tmp = pos;
                for (int i = 0; i < 8; i++)
                {
                    tmp += move; if (OutOfBounds(tmp)) {break;}
                    int curPlaceInfo = Board.board[tmp.x, tmp.y];

                    if (curPlaceInfo != Piece.None) // something there
                    {
                        if (!Piece.IsColor(curPlaceInfo, curColor)) { moves.Add(tmp); break; } //eatable, break
                        else {break;} // our side, break
                    }
                    else { moves.Add(tmp); } //nothing there
                }
            }
        }

        return moves;
    }

    private static void InitializeCombo()
    {
        combo = new List<Vector2Int>(verHori.Count + diagonal.Count);
        combo.AddRange(verHori);
        combo.AddRange(diagonal);
    }

    private static bool OutOfBounds(Vector2Int pos)
    {
        return (pos.x < 0 || pos.y < 0 || pos.x > 7 || pos.y > 7);
    }
}
