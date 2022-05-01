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
    [SerializeField] private Color snowColor = Color.black;

    private float hexWidth, hexHeight;
    private List<List<HexSnowController>> hexes = new List<List<HexSnowController>>();
    private Dictionary<string, Vector2> dictProb;

    private float[] pFreeze = {1, 0.2f, 0.1f, 0, 0.2f, 0.1f, 0, 0.1f, 0.1f, 1, 1, 0};
    private float[] pMelt = {0, 0.7f, 0.5f, 0.5f, 0, 0, 0.3f, 0.5f, 0, 0.2f, 0.1f, 0};
    private string[] config = {
        "000001", "000011", "000101", "000111", 
        "001001", "001011", "001111", "010101",     
        "010111", "011011", "011111", "111111"
    };
    private const int state = 12;
    
    private void Start()
    {
        hexHeight = hexSize; hexWidth = hexHeight/2f * Mathf.Sqrt(3f);

        SetupConfig();
        CreateHexBoard();
        StartCoroutine(GenerateSnow());
    }

    private void SetupConfig()
    {
        dictProb = new Dictionary<string, Vector2>();

        for (int i = 0; i < state; i++)
        {
            string conf = config[i]; Vector2 cur = new Vector2(pFreeze[i], pMelt[i]);
            for (int j = 0; j < 6; j++)
            {
                string conf2 = ""; 
                conf2 += conf[3]; conf2 += conf[4]; conf2 += conf[5]; 
                conf2 += conf[0]; conf2 += conf[1]; conf2 += conf[2];

                string conf3 = ""; 
                conf3 += conf[0]; conf3 += conf[2]; conf3 += conf[1]; 
                conf3 += conf[3]; conf3 += conf[5]; conf3 += conf[4];

                if (!dictProb.ContainsKey(conf)) { dictProb.Add(conf, cur);}
                if (!dictProb.ContainsKey(conf2)) { dictProb.Add(conf2, cur);} //mirror
                if (!dictProb.ContainsKey(conf3)) { dictProb.Add(conf3, cur);} //mirror

                char tmp = conf[0]; conf = conf.Remove(0, 1); conf += tmp; //rotate
            }
        }
        dictProb.Add("000000", new Vector2(0f, 0f)); //0 case
    }

    IEnumerator GenerateSnow()
    {
        int x1 = hexLevel/2, y1 = perLevel/2;
        hexes[x1][y1].SetColor(snowColor, true);

        while(true)
        {
            List<List<int>> state = new List<List<int>>(); yield return new WaitForSeconds(delay);
            float p = UnityEngine.Random.Range(0f, 1f);

            for (int i = 0; i < hexLevel; i++)
            {
                state.Add(new List<int>());
                for (int j = 0; j < perLevel; j++)
                {
                    var cur = hexes[i][j];
                    string res = "";

                    res += (!cur.leftHex || !cur.leftHex.GetState()) ? '0' : '1';
                    res += (!cur.bottomLeftHex || !cur.bottomLeftHex.GetState()) ? '0' : '1';
                    res += (!cur.bottomRightHex || !cur.bottomRightHex.GetState()) ? '0' : '1';

                    res += (!cur.rightHex || !cur.rightHex.GetState()) ? '0' : '1';
                    res += (!cur.topRightHex || !cur.topRightHex.GetState()) ? '0' : '1';
                    res += (!cur.topLeftHex || !cur.topLeftHex.GetState()) ? '0' : '1';

                    Vector2 prob = dictProb[res];
                    
                    if (!cur.GetState()) //melted
                    {
                        //if (p < prob.x) {cur.SetColor(snowColor, true);} //freeze
                        state[i].Add((p < prob.x) ? 1 : 0);
                    }
                    else //frozen
                    {
                        //Debug.Log(prob.y);
                        //if (p < prob.y) {cur.RendOff();} //melt
                        state[i].Add((p < prob.y) ? 0 : 1);
                    }
                }
            }

            bool reached = false;
            for (int i = 0; i < hexLevel; i++)
            {
                for (int j = 0; j < perLevel; j++)
                {
                    if (state[i][j] == 1) 
                    {
                        hexes[i][j].SetColor(snowColor, true);
                        if (i == 0 || j == 0 || i == hexLevel-1 || j == perLevel-1) {reached = true;}
                    }
                    else {hexes[i][j].RendOff();}
                }
            }
            if (reached) {break;} //reach border, stop
        }
    }

    private void CreateHexBoard()
    {
        hexes = new List<List<HexSnowController>>();
        for (int i = 0; i < hexLevel; i++) { hexes.Add(new List<HexSnowController>()); }

        bool goLeft = true; Vector2 original = new Vector2(0f, 0f);
        original += new Vector2(-hexWidth*perLevel/2f, hexLevel*hexHeight*0.75f/2f);

        //create board
        for (int i = 0; i < hexLevel; i++)
        {
            original = (goLeft) ? GetBottomLeftPos(original) : GetBottomRightPos(original); goLeft = !goLeft; Vector2 cur = original;
            for (int j = 0; j < perLevel; j++)
            {
                var con = CreateHex(cur); cur = GetRightPos(cur);
                hexes[i].Add(con);
            }
        }

        //add neighbor for each hex
        for (int i = 0; i < hexLevel; i++)
        {
            for (int j = 0; j < perLevel; j++)
            {
                var con = hexes[i][j]; var pos = con.GetPos();
                for (int k1 = i-2; k1 <= i+2; k1++)
                {
                    for (int k2 = j-2; k2 <= j+2; k2++)
                    {
                        if (k1 < 0 || k1 >= hexLevel || k2 < 0 || k2 >= perLevel) {continue;}
                        var con2 = hexes[k1][k2];
                        
                        if (UtilityFunc.Dist(con2.GetPos(), GetTopRightPos(pos)) <= 0.00001f)       {con.topRightHex = con2;}
                        if (UtilityFunc.Dist(con2.GetPos(), GetTopLeftPos(pos)) <= 0.00001f)        {con.topLeftHex = con2;}
                        if (UtilityFunc.Dist(con2.GetPos(), GetRightPos(pos)) <= 0.00001f)          {con.rightHex = con2;}
                        if (UtilityFunc.Dist(con2.GetPos(), GetLeftPos(pos)) <= 0.00001f)           {con.leftHex = con2;}
                        if (UtilityFunc.Dist(con2.GetPos(), GetBottomRightPos(pos)) <= 0.00001f)    {con.bottomRightHex = con2;}
                        if (UtilityFunc.Dist(con2.GetPos(), GetBottomLeftPos(pos)) <= 0.00001f)     {con.bottomLeftHex = con2;}
                    }
                }
            }
        }
    }

    private Vector2 GetTopRightPos(Vector2 pos) { return new Vector2(pos.x + hexWidth/2, pos.y + hexHeight*0.75f); }
    private Vector2 GetTopLeftPos(Vector2 pos) { return new Vector2(pos.x - hexWidth/2, pos.y + hexHeight*0.75f); }

    private Vector2 GetLeftPos(Vector2 pos) { return new Vector2(pos.x - hexWidth, pos.y); }
    private Vector2 GetRightPos(Vector2 pos) { return new Vector2(pos.x + hexWidth, pos.y); }

    private Vector2 GetBottomRightPos(Vector2 pos) { return new Vector2(pos.x + hexWidth/2, pos.y - hexHeight*0.75f); }
    private Vector2 GetBottomLeftPos(Vector2 pos) { return new Vector2(pos.x - hexWidth/2, pos.y - hexHeight*0.75f); }

    private HexSnowController CreateHex(Vector2 pos)
    {
        GameObject hex = Instantiate(hexPref, pos, Quaternion.identity, transform);

        var con = hex.GetComponent<HexSnowController>();
        con.SetSize(hexSize); 
        con.RendOff(); //turn sprite renderer of to save the damn fps

        return con;
    }
}
