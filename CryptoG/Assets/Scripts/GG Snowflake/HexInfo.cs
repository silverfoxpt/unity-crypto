using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexInfo 
{
    public Vector2 pos;
    public int att;
    public float vapor, quasi, ice;
    public Vector3Int tilePos; //prevent unneccesary get in update

    public float tmp1; //placeholder
 
    public HexInfo() {}
    public HexInfo(Vector2 p, int a, float q, float i, float v, Vector3Int tPos) {pos = p; att = a; vapor = v; quasi = q; ice = i; tilePos = tPos;}
}
