using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellSnowflakeManager : MonoBehaviour
{
    [SerializeField] private GameObject hexPref;
    [SerializeField] private float hexSize = 1f;

    [Space(15)]
    [SerializeField] private int hexLevel = 5;
    [SerializeField] private int perLevel = 20;
    [SerializeField] private float delay = 1f;

    private float hexLengthSize = 0f;
    private List<List<HexSnowController>> hexes = new List<List<HexSnowController>>();
    

    private void Start()
    {
        hexLengthSize = hexSize * ((-1 + Mathf.Sqrt(7f)) / 3f);

        CreateHexBoard();
    }

    private void CreateHexBoard()
    {
        hexes = new List<List<HexSnowController>>();
        for (int i = 0; i < hexLevel; i++) { hexes.Add(new List<HexSnowController>()); }

        bool goLeft = true; Vector2 original = new Vector2(0f, 0f);
        original += new Vector2(-hexLevel*hexSize/2f, perLevel*(hexSize/2f + hexLengthSize/2f) /2f);

        for (int i = 0; i < hexLevel; i++)
        {
            original = (goLeft) ? GetBottomLeftPos(original) : GetBottomRightPos(original); goLeft = !goLeft;
            Vector2 cur = original;

            for (int j = 0; j < perLevel; j++)
            {
                var con = CreateHex(cur); cur = GetRightPos(cur);
                hexes[i].Add(con);
            }
        }
        
        /*var con1 = CreateHex(new Vector2(0,0));

        hexes = new List<List<HexSnowController>>();
        for (int i = 0; i < hexLevel; i++) { hexes.Add(new List<HexSnowController>()); }
        hexes[0].Add(con1);

        for (int i = 1; i < hexLevel; i++)
        {
            //create hex
            yield return new WaitForSeconds(delay);
            for (int j = 0; j < hexes[i-1].Count; j++)
            {
                var con = hexes[i-1][j]; 
                
                if (!con.topRightHex)
                {
                    var newCon = CreateHex(GetTopRightPos(con.GetPos())); hexes[i].Add(newCon);
                }
                if (!con.topLeftHex)
                {
                    var newCon = CreateHex(GetTopLeftPos(con.GetPos())); hexes[i].Add(newCon);
                }
                if (!con.leftHex)
                {
                    var newCon = CreateHex(GetLeftPos(con.GetPos())); hexes[i].Add(newCon);
                }
                if (!con.rightHex)
                {
                    var newCon = CreateHex(GetRightPos(con.GetPos())); hexes[i].Add(newCon);
                }
                if (!con.bottomLeftHex)
                {
                    var newCon = CreateHex(GetBottomLeftPos(con.GetPos())); hexes[i].Add(newCon);
                }
                if (!con.bottomRightHex)
                {
                    var newCon = CreateHex(GetBottomRightPos(con.GetPos())); hexes[i].Add(newCon);
                }

                //check neighbors
                for (int k = 0; k < hexes[i].Count; k++)
                {
                    var pos = hexes[i][k].GetPos(); var newCon = hexes[i][k];
                    for (int m = i-1; m <= i; m++) //neighbors level
                    {
                        foreach(HexSnowController con2 in hexes[m])
                        {
                            if (UtilityFunc.Dist(con2.GetPos(), GetTopRightPos(pos)) <= 0.00001f)       {newCon.topRightHex = con2;}
                            if (UtilityFunc.Dist(con2.GetPos(), GetTopLeftPos(pos)) <= 0.00001f)        {newCon.topLeftHex = con2;}
                            if (UtilityFunc.Dist(con2.GetPos(), GetRightPos(pos)) <= 0.00001f)          {newCon.rightHex = con2;}
                            if (UtilityFunc.Dist(con2.GetPos(), GetLeftPos(pos)) <= 0.00001f)           {newCon.leftHex = con2;}
                            if (UtilityFunc.Dist(con2.GetPos(), GetBottomRightPos(pos)) <= 0.00001f)    {newCon.bottomRightHex = con2;}
                            if (UtilityFunc.Dist(con2.GetPos(), GetBottomLeftPos(pos)) <= 0.00001f)     {newCon.bottomLeftHex = con2;}
                        }
                    }
                }

                for (int k = 0; k < hexes[i-1].Count; k++)
                {
                    var pos = hexes[i-1][k].GetPos(); var newCon = hexes[i-1][k];
                    foreach(HexSnowController con2 in hexes[i])
                    {
                        if (UtilityFunc.Dist(con2.GetPos(), GetTopRightPos(pos)) <= 0.00001f)       {newCon.topRightHex = con2;}
                        if (UtilityFunc.Dist(con2.GetPos(), GetTopLeftPos(pos)) <= 0.00001f)        {newCon.topLeftHex = con2;}
                        if (UtilityFunc.Dist(con2.GetPos(), GetRightPos(pos)) <= 0.00001f)          {newCon.rightHex = con2;}
                        if (UtilityFunc.Dist(con2.GetPos(), GetLeftPos(pos)) <= 0.00001f)           {newCon.leftHex = con2;}
                        if (UtilityFunc.Dist(con2.GetPos(), GetBottomRightPos(pos)) <= 0.00001f)    {newCon.bottomRightHex = con2;}
                        if (UtilityFunc.Dist(con2.GetPos(), GetBottomLeftPos(pos)) <= 0.00001f)     {newCon.bottomLeftHex = con2;}
                    }
                }
            }
        }*/
    }

    private Vector2 GetTopRightPos(Vector2 pos) { return new Vector2(pos.x + hexSize/2, pos.y + hexSize/2 + hexLengthSize/2); }
    private Vector2 GetTopLeftPos(Vector2 pos) { return new Vector2(pos.x - hexSize/2, pos.y + hexSize/2 + hexLengthSize/2); }

    private Vector2 GetLeftPos(Vector2 pos) { return new Vector2(pos.x - hexSize, pos.y); }
    private Vector2 GetRightPos(Vector2 pos) { return new Vector2(pos.x + hexSize, pos.y); }

    private Vector2 GetBottomRightPos(Vector2 pos) { return new Vector2(pos.x + hexSize/2, pos.y - hexSize/2 - hexLengthSize/2); }
    private Vector2 GetBottomLeftPos(Vector2 pos) { return new Vector2(pos.x - hexSize/2, pos.y - hexSize/2 - hexLengthSize/2); }


    private HexSnowController CreateHex(Vector2 pos)
    {
        GameObject hex = Instantiate(hexPref, pos, Quaternion.identity, transform);

        var con = hex.GetComponent<HexSnowController>();
        con.SetSize(hexSize);
        con.SetColor(UtilityFunc.GetRandColor());

        return con;
    }
}
