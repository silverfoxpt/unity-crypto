using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Piece
{
    public static int None = 0;
    public static int Pawn = 1;
    public static int Knight = 2;
    public static int Bishop = 3;
    public static int Rook = 4;
    public static int Queen = 5;
    public static int King = 6;

    public static int White = 8;
    public static int Black = 16;

    public static Vector2Int PieceInfo(int inp)
    {
        Vector2Int res = new Vector2Int();
        res.y = UtilityFunc.IsBitSet(inp, 0)        + UtilityFunc.IsBitSet(inp, 1) * 2      + UtilityFunc.IsBitSet(inp, 2) * 4;
        res.x = UtilityFunc.IsBitSet(inp, 3) * 8    + UtilityFunc.IsBitSet(inp, 4) * 16;

        return res; 
    }
}
