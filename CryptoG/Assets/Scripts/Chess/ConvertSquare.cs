using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ConvertSquare 
{
    public static Vector2Int NoteToSquare(string en)
    {
        if (en.Length == 1) {return UtilityFunc.nullVecInt; }
        Vector2Int stat = new Vector2Int();
        stat.x = (int) (en[0] - 'a');
        stat.y = (int) (en[1] - '1');

        return stat;
    }
}
