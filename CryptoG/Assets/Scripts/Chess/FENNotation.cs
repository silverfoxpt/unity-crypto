using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class FENNotation
{
    public static string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    public static void LoadBoardFromFen()
    {
        //fen = "3K4/8/1p5N/6p1/8/p3Q2N/8/3kN3 w - - 0 1"; //test
        fen = "k1K5/7p/6N1/8/1Pp5/2nP4/8/8 b - b3 0 1"; //test enpassant

        //prep board
        BoardController.ClearBoard();

        string [] sp = fen.Split(' ');
        string curPos = sp[0];

        string[] fi = curPos.Split('/');
        List<string> ranks = fi.ToList(); ranks.Reverse();

        string num = "0123456789";

        for (int i = 0; i < 8; i++) //ranks 1->8
        {
            int cur = 0;
            foreach(char c in ranks[i])
            {
                if (num.Contains(c)) //space(s)
                {
                    int x = (c - '0'); int tmp2 = cur;
                    for (int j = tmp2; j < tmp2 + x; j++)
                    {
                        Board.board[j, i] = Piece.None;
                        cur++;
                    }
                }
                else // a piece
                {
                    Board.board[cur, i] = Piece.ToPiece(c); cur++;
                }
            }
        }        
        //build new graphical board
        BoardController.BuildBoard();
    }

    public static int MoveOrder()
    {
        string [] sp = fen.Split(' ');
        return (sp[1] == "w") ? Piece.White : Piece.Black;
    }

    public static Vector2Int EnPassantMove()
    {
        string [] sp = fen.Split(' ');
        string en = sp[3];

        return ConvertSquare.NoteToSquare(en);
    }
}
