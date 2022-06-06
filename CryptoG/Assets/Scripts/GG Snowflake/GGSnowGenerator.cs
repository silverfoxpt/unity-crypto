using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Tilemaps;
using UnityEngine.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using System;

public class GGSnowGenerator : MonoBehaviour
{
    struct hexNeighbor
    {
        public HexInfo myHex;
        public HexInfo topRight, topLeft, right, left, bottomRight, bottomLeft;

        public hexNeighbor(HexInfo my, HexInfo topR, HexInfo topL, HexInfo r, HexInfo l, HexInfo botR, HexInfo botL)
        {
            this.myHex = my;
            this.topLeft = topL; this.topRight = topR;
            this.left = l; this.right = r;
            this.bottomLeft = botL; this.bottomRight = botR;
        }
    }

    [Header("Tilemap refs")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tile tilePref;

    [Header("Board options")]
    [SerializeField] private int boardRow = 200;
    [SerializeField] private int boardCol = 200;

    [Header("Hex options")]
    [SerializeField] private float hexSize = 0.2f;

    [Header("Snow options")]
    [SerializeField] private float p;
    [SerializeField] private float k;
    [SerializeField] private float u;
    [SerializeField] private float y;
    [SerializeField] private float b;
    [SerializeField] private float theta0;
    [SerializeField] private float alpha;

    [Header("Other")]
    //[SerializeField] private float delay = 0.2f;
    [SerializeField] private float simDelay = 0.1f;
    [SerializeField] private Color snowColor = Color.blue;

    //private
    private float hexHeight, hexWidth;
    private List<List<HexInfo>> hexesTmp;
    private List<List<hexNeighbor>> hexes;
    private Dictionary<Vector3Int, HexInfo> hexPos;
    private List<hexNeighbor> nonBorder, border, attached;

    private void Start()
    {
        CreateBoard();
        StartCoroutine(SimulateSnow());
    }

    IEnumerator SimulateSnow()
    {
        GenerateSets();

        while(true)
        {
            yield return new WaitForSeconds(simDelay);

            Diffuse();
            Freeze();
            Atttachment();

            GenerateSets();
            Melts();
            //GenerateSets();
        }
    }

    private void Melts()
    {
        foreach (hexNeighbor hex in border)
        {
            float tmp1 = hex.myHex.quasi, tmp2 = hex.myHex.ice;
            hex.myHex.quasi = (1-u) * tmp1;
            hex.myHex.ice = (1-y) * tmp2;
            hex.myHex.vapor += u * tmp1 + y * tmp2;
        }
    }

    private void Atttachment()
    {
        foreach(hexNeighbor hex in border)
        {
            int count = 0; 
            if (hex.topLeft != null && hex.topLeft.att == 1)            {count++;}
            if (hex.topRight != null && hex.topRight.att == 1)          {count++;}
            if (hex.left != null && hex.left.att == 1)                  {count++;}
            if (hex.right != null && hex.right.att == 1)                {count++;}
            if (hex.bottomLeft != null && hex.bottomLeft.att == 1)      {count++;}
            if (hex.bottomRight != null && hex.bottomRight.att == 1)    {count++;}

            if (count <= 2)
            {
                if (hex.myHex.quasi >= b) 
                {
                    hex.myHex.att = 1;
                    CreateNewHexagonEx(hex.myHex.tilePos);
                }
            }
            else if (count == 3)
            {
                if (hex.myHex.quasi >= 1) 
                {
                    hex.myHex.att = 1;
                    CreateNewHexagonEx(hex.myHex.tilePos);
                }
                else 
                {
                    float sum = 0;
                    if (hex.topLeft != null)        {sum += hex.topLeft.vapor;}
                    if (hex.topRight != null)       {sum += hex.topRight.vapor;}
                    if (hex.left != null)           {sum += hex.left.vapor;}
                    if (hex.right != null)          {sum += hex.right.vapor;}
                    if (hex.bottomLeft != null)     {sum += hex.bottomLeft.vapor;}
                    if (hex.bottomRight != null)    {sum += hex.bottomRight.vapor;}
                    if (hex.myHex.quasi >= alpha && sum <= theta0) 
                    {
                        hex.myHex.att = 1;
                        CreateNewHexagonEx(hex.myHex.tilePos);
                    }
                }
            }
            else
            {
                hex.myHex.att = 1;
                CreateNewHexagonEx(hex.myHex.tilePos);
            }
        }
    }

    private void Freeze()
    {
        //freeze
        foreach (hexNeighbor hex in border)
        {
            hex.myHex.quasi += (1-k)*hex.myHex.vapor;
            hex.myHex.ice += k*hex.myHex.vapor;
            hex.myHex.vapor = 0;
        }
    }

    private void Diffuse()
    {
        //diffuse
        //nonborder sites
        foreach (hexNeighbor hex in nonBorder)
        {
            float sum = 0;
            sum += hex.myHex.vapor;
            if (hex.topLeft != null)        { sum += hex.topLeft.vapor; }       else { sum += hex.myHex.vapor; }
            if (hex.topRight != null)       { sum += hex.topRight.vapor; }      else { sum += hex.myHex.vapor; }
            if (hex.left != null)           { sum += hex.left.vapor; }          else { sum += hex.myHex.vapor; }
            if (hex.right != null)          { sum += hex.right.vapor; }         else { sum += hex.myHex.vapor; }
            if (hex.bottomLeft != null)     { sum += hex.bottomLeft.vapor; }    else { sum += hex.myHex.vapor; }
            if (hex.bottomRight != null)    { sum += hex.bottomRight.vapor; }   else { sum += hex.myHex.vapor; }

            hex.myHex.tmp1 = sum / 7;
        }
        foreach (hexNeighbor hex in nonBorder)
        {
            hex.myHex.vapor = hex.myHex.tmp1;
        }

        //border sites
        foreach (hexNeighbor hex in border)
        {
            float sum = 0;
            sum += hex.myHex.vapor;
            if (hex.topLeft != null && hex.topLeft.att == 0)            { sum += hex.topLeft.vapor; }       else { sum += hex.myHex.vapor; }
            if (hex.topRight != null && hex.topRight.att == 0)          { sum += hex.topRight.vapor; }      else { sum += hex.myHex.vapor; }
            if (hex.left != null && hex.left.att == 0)                  { sum += hex.left.vapor; }          else { sum += hex.myHex.vapor; }
            if (hex.right != null && hex.right.att == 0)                { sum += hex.right.vapor; }         else { sum += hex.myHex.vapor; }
            if (hex.bottomLeft != null && hex.bottomLeft.att == 0)      { sum += hex.bottomLeft.vapor; }    else { sum += hex.myHex.vapor; }
            if (hex.bottomRight != null && hex.bottomRight.att == 0)    { sum += hex.bottomRight.vapor; }   else { sum += hex.myHex.vapor; }

            hex.myHex.tmp1 = sum / 7;
        }
        foreach (hexNeighbor hex in border)
        {
            hex.myHex.vapor = hex.myHex.tmp1;
        }
    }

    private void GenerateSets()
    {
        nonBorder = new List<hexNeighbor>();
        border = new List<hexNeighbor>();
        attached = new List<hexNeighbor>();

        for (int i = 0; i < boardRow; i++)
        {
            for (int j = 0; j < boardCol; j++)
            {
                var tmp = hexes[i][j];
                if (tmp.myHex.att == 1) { attached.Add(tmp); }
                else if (CheckNeighbor(tmp)) { border.Add(tmp); }
                else { nonBorder.Add(tmp); }
            }
        }
    }

    private bool CheckNeighbor(hexNeighbor tmp)
    {
        if (tmp.topRight != null && tmp.topRight.att == 1) {return true;}
        if (tmp.topLeft != null && tmp.topLeft.att == 1) {return true;}
        if (tmp.right != null && tmp.right.att == 1) {return true;}
        if (tmp.left != null && tmp.left.att == 1) {return true;}
        if (tmp.bottomRight != null && tmp.bottomRight.att == 1) {return true;}
        if (tmp.bottomLeft != null && tmp.bottomLeft.att == 1) {return true;}
        return false;

    }

    private void CreateBoard()
    {
        hexesTmp = new List<List<HexInfo>>();
        hexPos = new Dictionary<Vector3Int, HexInfo>();

        tilemap.transform.localScale = new Vector3(hexSize, hexSize, 1f);
        hexHeight = hexSize; hexWidth = hexHeight/2f * Mathf.Sqrt(3f);

        bool goLeft = true; Vector2 startingPos = new Vector2(0f, 0f);
        startingPos += new Vector2(-hexWidth*boardCol/2f, boardRow*hexHeight*0.75f/2f);

        //create board
        for (int i = 0; i < boardRow; i++)
        {
            startingPos = (goLeft) ? GetBottomLeftPos(startingPos) : GetBottomRightPos(startingPos); goLeft = !goLeft; Vector2 currentPos = startingPos;
            hexesTmp.Add(new List<HexInfo>());

            for (int j = 0; j < boardCol; j++)
            {
                //CreateNewHexagon(currentPos); //not yet

                var tPos = tilemap.WorldToCell(currentPos);
                hexesTmp[i].Add(new HexInfo(currentPos, 0, 0f, 0f, p, tPos));
                currentPos = GetRightPos(currentPos);

                hexPos.Add(tPos, hexesTmp[i][j]);
            }
        }

        //plant middle seed
        int mid1 = boardRow/2-1, mid2 = boardCol/2+1;

        var tmp = hexesTmp[mid1][mid2];
        tmp.att = 1; tmp.ice = 1f;

        hexesTmp[mid1][mid2] = tmp;
        CreateNewHexagonEx(hexesTmp[mid1][mid2].tilePos);

        //create neighbors
        hexes = new List<List<hexNeighbor>>();
        for (int i = 0; i < boardRow; i++)
        {
            hexes.Add(new List<hexNeighbor>());
            for (int j = 0; j < boardCol; j++)
            {
                HexInfo tR = null, tL = null, R = null, L = null, bR = null, bL = null; 
                HexInfo hex = hexesTmp[i][j];
                Vector2 pos = hex.pos;

                Vector2 topRight = GetTopRightPos(pos); Vector3Int tpRight = tilemap.WorldToCell(topRight);
                if (hexPos.ContainsKey(tpRight)) {tR = hexPos[tpRight];}

                Vector2 topLeft = GetTopLeftPos(pos); Vector3Int tpLeft = tilemap.WorldToCell(topLeft);
                if (hexPos.ContainsKey(tpLeft)) {tL = hexPos[tpLeft];}

                Vector2 right = GetRightPos(pos); Vector3Int ri = tilemap.WorldToCell(right);
                if (hexPos.ContainsKey(ri)) {R = hexPos[ri];}

                Vector2 left = GetLeftPos(pos); Vector3Int le = tilemap.WorldToCell(left);
                if (hexPos.ContainsKey(le)) {L = hexPos[le];}

                Vector2 bottomRight = GetBottomRightPos(pos); Vector3Int btRight = tilemap.WorldToCell(bottomRight);
                if (hexPos.ContainsKey(btRight)) {bR = hexPos[btRight];}

                Vector2 bottomLeft = GetBottomLeftPos(pos); Vector3Int btLeft = tilemap.WorldToCell(bottomLeft);
                if (hexPos.ContainsKey(btLeft)) {bL = hexPos[btLeft];}

                hexes[i].Add(new hexNeighbor(hex, tR, tL, R, L, bR, bL));
            }
        }
    }

    private void CreateNewHexagon(Vector2 currentPos)
    {
        Vector3Int tilePos = tilemap.WorldToCell(currentPos);

        tilemap.SetTile(tilePos, tilePref);
        tilemap.SetTileFlags(tilePos, TileFlags.None);
        //tilemap.SetColor(tilePos, UtilityFunc.GetRandColor()); //test
    }

    private void CreateNewHexagonEx(Vector3Int pos)
    {
        tilemap.SetTile(pos, tilePref);
        tilemap.SetTileFlags(pos, TileFlags.None);
        tilemap.SetColor(pos, snowColor);
    }

    private Vector2 GetTopRightPos(Vector2 pos) { return new Vector2(pos.x + hexWidth/2, pos.y + hexHeight*0.75f); }
    private Vector2 GetTopLeftPos(Vector2 pos) { return new Vector2(pos.x - hexWidth/2, pos.y + hexHeight*0.75f); }

    private Vector2 GetLeftPos(Vector2 pos) { return new Vector2(pos.x - hexWidth, pos.y); }
    private Vector2 GetRightPos(Vector2 pos) { return new Vector2(pos.x + hexWidth, pos.y); }

    private Vector2 GetBottomRightPos(Vector2 pos) { return new Vector2(pos.x + hexWidth/2, pos.y - hexHeight*0.75f); }  
    private Vector2 GetBottomLeftPos(Vector2 pos) { return new Vector2(pos.x - hexWidth/2, pos.y - hexHeight*0.75f); }
}
 