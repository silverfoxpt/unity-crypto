using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineConstraint
{
    public int bombCount;
    public HashSet<Vector2Int> bombCells;

    public void Initialize() {bombCells = new HashSet<Vector2Int>(); bombCount = -1;}
}
