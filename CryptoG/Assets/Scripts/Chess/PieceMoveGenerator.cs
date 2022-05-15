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

    private static List<Vector2Int> knight = new List<Vector2Int>() {
        new Vector2Int(-2, +1),
        new Vector2Int(-1, +2),

        new Vector2Int(+1, +2),
        new Vector2Int(+2, +1),

        new Vector2Int(+2, -1),
        new Vector2Int(+1, -2),

        new Vector2Int(-1, -2),
        new Vector2Int(-2, -1),
    };

    private static List<Vector2Int> combo = new List<Vector2Int>();

    public static List<Vector2Int> GetMoves(Vector2Int pos)
    {
        if (combo.Count == 0) {InitializeCombo(); }
        
        List<Vector2Int> moves = new List<Vector2Int>(); 
        int info = Board.board[pos.x, pos.y];
        int curColor = FENNotation.MoveOrder();

        if (!Piece.IsColor(info, curColor)) {return moves;} //incorrect side, empty

        if      (Piece.IsType(info, Piece.Rook))
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
        else if (Piece.IsType(info, Piece.Queen))
        {
            foreach(Vector2Int move in combo) //Queens moves all
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
        else if (Piece.IsType(info, Piece.Knight))
        {
            foreach(Vector2Int move in knight) //Kinght moves L-shaped
            {
                Vector2Int tmp = pos;

                tmp += move; if (OutOfBounds(tmp)) {continue;}
                int curPlaceInfo = Board.board[tmp.x, tmp.y];

                if (curPlaceInfo != Piece.None) // something there
                {
                    if (!Piece.IsColor(curPlaceInfo, curColor)) { moves.Add(tmp); continue;} //eatable, break
                    else {continue; } // our side, break
                }
                else { moves.Add(tmp); } //nothing there
            }
        }
        else if (Piece.IsType(info, Piece.Pawn)) //4 move types
        {
            Vector2Int enPass = FENNotation.EnPassantMove(); 
            if (Piece.IsColor(info, Piece.White)) //white pawns
            {
                Vector2Int cur; 

                //top left
                cur = new Vector2Int(-1, +1); 
                var tmp     = pos; tmp += cur; //new pos
                var en      = tmp + new Vector2Int(0, -1); //en passant pos

                if (!OutOfBounds(tmp) && !OutOfBounds(en))
                {    
                    int atInfo      = Board.board[tmp.x, tmp.y]; 
                    int enPassant   = Board.board[en.x, en.y];

                    if ((atInfo != Piece.None && !Piece.IsColor(atInfo, curColor)) || //eatable piece there
                        (enPass == tmp)) //en passant 
                    {
                        moves.Add(tmp);
                    }
                }

                //top right 
                cur = new Vector2Int(+1, +1); 
                tmp     = pos; tmp += cur; //new pos
                en      = tmp + new Vector2Int(0, -1); //en passant pos

                if (!OutOfBounds(tmp) && !OutOfBounds(en))
                {    
                    int atInfo      = Board.board[tmp.x, tmp.y]; 
                    int enPassant   = Board.board[en.x, en.y];

                    if ((atInfo != Piece.None && !Piece.IsColor(atInfo, curColor)) || //eatable piece there
                        (enPass == tmp)) //en passant 
                    {
                        moves.Add(tmp);
                    }
                }

                //one forward
                cur = new Vector2Int(0, +1);
                tmp = pos; tmp += cur;

                if (!OutOfBounds(tmp))
                {
                    int atInfo      = Board.board[tmp.x, tmp.y]; 
                    if (atInfo == Piece.None) //empty space ahead
                    {
                        moves.Add(tmp);
                    }
                }

                //two forward - only available on second rank
                cur = new Vector2Int(0, +2);
                tmp = pos; tmp += cur;
                var tmp2 = pos + new Vector2Int(0, -1);

                if (!OutOfBounds(tmp))
                {
                    int atInfo      = Board.board[tmp.x, tmp.y]; 
                    int atInfo2     = Board.board[tmp2.x, tmp2.y];

                    if (atInfo == Piece.None && tmp.y == 3 && atInfo2 == Piece.None) //2 empty space ahead && destination on fourth rank
                    {
                        moves.Add(tmp);
                    }
                }
            }
            else //black pawns
            {
                Vector2Int cur; 

                //bottom left
                cur = new Vector2Int(-1, -1); 
                var tmp     = pos; tmp += cur; //new pos
                var en      = tmp + new Vector2Int(0, +1); //en passant pos

                if (!OutOfBounds(tmp) && !OutOfBounds(en))
                {    
                    int atInfo      = Board.board[tmp.x, tmp.y]; 
                    int enPassant   = Board.board[en.x, en.y];

                    if ((atInfo != Piece.None && !Piece.IsColor(atInfo, curColor)) || //eatable piece there
                        (enPass == tmp)) //en passant 
                    {
                        moves.Add(tmp);
                    }
                }

                //top right 
                cur = new Vector2Int(+1, -1); 
                tmp     = pos; tmp += cur; //new pos
                en      = tmp + new Vector2Int(0, +1); //en passant pos

                if (!OutOfBounds(tmp) && !OutOfBounds(en))
                {    
                    int atInfo      = Board.board[tmp.x, tmp.y]; 
                    int enPassant   = Board.board[en.x, en.y];

                    if ((atInfo != Piece.None && !Piece.IsColor(atInfo, curColor)) || //eatable piece there
                        (enPass == tmp)) //en passant 
                    {
                        moves.Add(tmp);
                    }
                }

                //one forward
                cur = new Vector2Int(0, -1);
                tmp = pos; tmp += cur;

                if (!OutOfBounds(tmp))
                {
                    int atInfo      = Board.board[tmp.x, tmp.y]; 
                    if (atInfo == Piece.None) //empty space ahead
                    {
                        moves.Add(tmp);
                    }
                }

                //two forward - only available on second rank
                cur = new Vector2Int(0, -2);
                tmp = pos; tmp += cur;
                var tmp2 = pos + new Vector2Int(0, -1);

                if (!OutOfBounds(tmp))
                {
                    int atInfo      = Board.board[tmp.x, tmp.y]; 
                    int atInfo2     = Board.board[tmp2.x, tmp2.y];

                    if (atInfo == Piece.None && tmp.y == 4 && atInfo2 == Piece.None) //2 empty space ahead && destination on fourth rank
                    {
                        moves.Add(tmp);
                    }
                }
            }
        }
        else if (Piece.IsType(info, Piece.King))
        {
            
        }

        return moves;
    }

    public static List<Vector2Int> GetAllMoves(int[,] board)
    {
        return null;
    }

    public static List<Vector2Int> GetLegalMoves(List<Vector2Int> moves)
    {
        int[,] copy = Board.board.Clone() as int[,];

        foreach(Vector2Int move in moves)
        {
            
        }
        return null;
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
