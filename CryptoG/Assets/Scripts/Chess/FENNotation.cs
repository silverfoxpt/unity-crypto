using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class FENNotation
{
    public static string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    public static void LoadBoardFromFen()
    {
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
}
