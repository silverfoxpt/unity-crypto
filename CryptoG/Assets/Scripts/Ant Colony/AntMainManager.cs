using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntMainManager : MonoBehaviour
{
    struct antPackage
    {
        public Vector2Int pos;
        public Vector2Int foodPos;
        public Vector2 dir;
        public Vector2 desDir;
        public int status;
    }

    [Header("References")]
    [SerializeField] private MainBoardController mainBoard;
    [SerializeField] private MainBoardController pheroBoardBlue;
    [SerializeField] private MainBoardController pheroBoardRed;

    [Space(10)]
    [SerializeField] private GameObject home;
    [SerializeField] private GameObject antPref;
    [SerializeField] private ComputeShader antPath;
    [SerializeField] private ComputeShader antPhero;
    public string status = "drawing";

    [Header("Drawing")]
    [SerializeField] private Color nestColor = Color.blue;
    [SerializeField] private Color foodColor = Color.green;

    [Header("Ants")]
    [SerializeField] private int numAnts = 15;

    [Space(10)]
    [SerializeField] private float maxSpeed = 2;
    [SerializeField] private float steerStrength = 2;
    [SerializeField] private float wanderStrength = 0.1f;

    [Space(10)]
    [SerializeField] private float detectAngle = 40;
    [SerializeField] private float detectStrength = 3;

    [Space(10)]
    [SerializeField] private float turnAngle = 20;
    [SerializeField] private int turnStrength = 3;

    [Space(10)]
    [SerializeField] private float maxDistFromCenter = 5;
    [SerializeField] private bool useInverseDist = false;

    [Header("Pheromones")]
    [SerializeField] private Color toHomeColor = Color.blue;
    [SerializeField] private Color toFoodColor = Color.red;
    [SerializeField] private float diffuseAmount = 0.05f;
  
    private List<AntController> ants;
    private antPackage[] antInfo;

    private void Start()
    {
        InitializeBoard();
        SetFoodColor();

        //CreateAllAnts();
    }

    #region preSim
    private void CreateAllAnts()
    {
        ants = new List<AntController>();
        for (int i = 0; i < numAnts; i++)
        {
            var ant = Instantiate(antPref, home.transform.position, Quaternion.identity, transform);
            var con = ant.GetComponent<AntController>();
            con.maxSpeed = maxSpeed;
            con.steerStrength = steerStrength;
            con.wanderStrength = wanderStrength;
            con.SetAntMapSize(mainBoard.canvasInfo.GetMapRealBound());

            ants.Add(con);
        }
    }

    private void InitializeBoard()
    {
        mainBoard.pen.RefreshPen();
        mainBoard.board.CreateNewBoard();
        mainBoard.pen.allowedDraw = true;

        pheroBoardBlue.pen.RefreshPen();
        pheroBoardBlue.board.CreateNewBoard();
        pheroBoardBlue.pen.allowedDraw = false;

        pheroBoardRed.pen.RefreshPen();
        pheroBoardRed.board.CreateNewBoard();
        pheroBoardRed.pen.allowedDraw = false;
    }

    public void SetNestColor() { mainBoard.pen.ChangePenColor(nestColor);}
    public void SetFoodColor() { mainBoard.pen.ChangePenColor(foodColor);}

    public void StartSimulation()
    {
        CreateAllAnts();
        mainBoard.pen.allowedDraw = false; //can't draw
        
        antInfo = new antPackage[numAnts];
        status = "sim";
    }
    #endregion

    private void Update()
    {
        if (status == "sim")
        {
            AntInitiateShaderPackage();
            AntFindFood();
            AntDropFeromones();

            AntInitiateShaderPackage(); //retake
            AntFollowFeromones();
            PheromonesDiffuse();
        }
    }

    private void PheromonesDiffuse()
    {
        //copy
        RenderTexture.active = pheroBoardBlue.pen.rend;
        Graphics.Blit(pheroBoardBlue.board.imageTex, pheroBoardBlue.pen.rend);

        //blueboard
        antPhero.SetTexture(antPhero.FindKernel("AntPhero"), "pheroMap", pheroBoardBlue.pen.rend);
        antPhero.SetFloat("incAmount", diffuseAmount);

        antPhero.Dispatch(antPhero.FindKernel("AntPhero"), 
            pheroBoardBlue.board.size.x / 8, pheroBoardBlue.board.size.y / 8, 1);

        pheroBoardBlue.board.SetTexture(pheroBoardBlue.pen.rend);

        //copy
        RenderTexture.active = pheroBoardRed.pen.rend;
        Graphics.Blit(pheroBoardRed.board.imageTex, pheroBoardRed.pen.rend);

        //redboard
        antPhero.SetTexture(antPhero.FindKernel("AntPhero"), "pheroMap", pheroBoardRed.pen.rend);
        antPhero.SetFloat("incAmount", diffuseAmount);

        antPhero.Dispatch(antPhero.FindKernel("AntPhero"), 
            pheroBoardRed.board.size.x / 8, pheroBoardRed.board.size.y / 8, 1);

        pheroBoardRed.board.SetTexture(pheroBoardRed.pen.rend);
    }

    private void AntInitiateShaderPackage()
    {
        for (int i = 0; i < numAnts; i++)
        {
            var ant = ants[i];

            antPackage pack = new antPackage();
            pack.pos = mainBoard.canvasInfo.PosToImagePos(ant.transform.position);
            pack.dir = ant.velocity;
            pack.status = ant.foodStat;
            pack.desDir = ant.desiredDirection;
            pack.foodPos = ant.foodPos;

            antInfo[i] = pack;
        }
    }

    private void AntFollowFeromones()
    {
        for (int i = 0; i < numAnts; i++)
        {
            if (ants[i].foodStat == 1) {continue;}

            var dir = antInfo[i].desDir;
            var pos = mainBoard.canvasInfo.PosToImagePos(ants[i].transform.position);

            Vector2 forward = dir.normalized * turnStrength;
            Vector2 left = Quaternion.Euler(0f, 0f, turnAngle) * forward;
            Vector2 right = Quaternion.Euler(0f, 0f, 360f - turnAngle) * forward;

            var posF = new Vector2Int((int) (pos + forward).x, (int) (pos + forward).y);
            var posL = new Vector2Int((int) (pos + left).x, (int) (pos + left).y);
            var posR = new Vector2Int((int) (pos + right).x, (int) (pos + right).y);

            if (OutOfBounds(posF)) {posF = UtilityFunc.nullVecInt;}
            if (OutOfBounds(posL)) {posF = UtilityFunc.nullVecInt;}
            if (OutOfBounds(posR)) {posF = UtilityFunc.nullVecInt;}

            //take pheromone sample
            float valF, valR, valL;
            if (antInfo[i].status < 2) //follow red trail to food
            {
                var colF = pheroBoardRed.board.GetPixelDirect(posF);
                var colL = pheroBoardRed.board.GetPixelDirect(posL);
                var colR = pheroBoardRed.board.GetPixelDirect(posR);

                valF = (colF.a > 0) ? colF.r : 0; 
                valL = (colL.a > 0) ? colL.r : 0; 
                valR = (colR.a > 0) ? colR.r : 0; 
            }
            else if (antInfo[i].status == 2)
            {
                var colF = pheroBoardBlue.board.GetPixelDirect(posF);
                var colL = pheroBoardBlue.board.GetPixelDirect(posL);
                var colR = pheroBoardBlue.board.GetPixelDirect(posR);

                valF = (colF.a > 0) ? colF.b : 0; 
                valL = (colL.a > 0) ? colL.b : 0; 
                valR = (colR.a > 0) ? colR.b : 0; 
            }
            else {valF = 0; valR = 0; valL = 0;}            

            //change direction
            if (valF >= valL && valF >= valR) {} //keep on walking
            else if (valF <= valL && valF <= valR) //turn somewhere
            {
                ants[i].desiredDirection = (valL < valR) ? right : left;
            }
            else if (valL <= valR)  { ants[i].desiredDirection = right; }
            else if (valL >= valR)  { ants[i].desiredDirection = left; }
        }
    }

    private bool OutOfBounds(Vector2Int pos)
    {
        Vector2Int size = mainBoard.board.size;
        if (pos.x < 0 || pos.x >= size.x || pos.y < 0 || pos.y >= size.y) {return false;}
        return true;
    }

    private void AntDropFeromones()
    {
        for (int i = 0; i < numAnts; i++)
        {
            Vector2Int pos = mainBoard.canvasInfo.PosToImagePos(ants[i].transform.position);
            Color col = (ants[i].foodStat < 2) 
                ? pheroBoardBlue.board.GetPixelDirect(pos)
                : pheroBoardRed.board.GetPixelDirect(pos); 

            if (useInverseDist)
            {
                if (ants[i].foodStat < 2) 
                {
                    var inverseDist = 1/((ants[i].transform.position - home.transform.position).magnitude / maxDistFromCenter);
                    col.b = Mathf.Clamp(inverseDist, 0, 1);
                    col.a = Mathf.Clamp(inverseDist, 0, 1);
                }
                else  
                {
                    var inverseDist = 1/((pos - ants[i].foodPos).magnitude / maxDistFromCenter);                
                    col.r = Mathf.Clamp(inverseDist, 0, 1);
                    col.a = Mathf.Clamp(inverseDist, 0, 1);
                }
                col.a = 1;
            }
            else
            {
                if (ants[i].foodStat < 2) {col.b = 1;} else {col.r = 1;}
                col.a = 1;
            }

            if (ants[i].foodStat < 2) { pheroBoardBlue.board.SetPixelDirect(pos, col, false);}
            else { pheroBoardRed.board.SetPixelDirect(pos, col, false);}
        }
        
        pheroBoardBlue.board.imageTex.Apply(); //apply after
        pheroBoardRed.board.imageTex.Apply(); //apply after
    }

    private void AntFindFood()
    {
        antPath.SetTexture(antPath.FindKernel("AntPathFind"), "foodMap", mainBoard.pen.rend);

        int strides = sizeof(int) * 2 + sizeof(int) * 2 +
                        sizeof(float) * 2 + sizeof(float) * 2 + sizeof(int);
        ComputeBuffer compBuff = new ComputeBuffer(numAnts, strides);
        compBuff.SetData(antInfo);

        antPath.SetBuffer(antPath.FindKernel("AntPathFind"), "antInfo", compBuff);
        antPath.SetInt("num", numAnts);
        antPath.SetFloat("angle", detectAngle);
        antPath.SetFloat("strength", detectStrength);
        antPath.SetVector("emptyColor", mainBoard.board.originalColor);

        Vector2Int homePos = mainBoard.canvasInfo.PosToImagePos(home.transform.position);
        antPath.SetInt("homeX", homePos.x); antPath.SetInt("homeY", homePos.y);

        antPath.Dispatch(antPath.FindKernel("AntPathFind"),
            mainBoard.board.size.x / 8, mainBoard.board.size.y / 8, 1);

        compBuff.GetData(antInfo);
        for (int i = 0; i < numAnts; i++)
        {
            ants[i].desiredDirection = antInfo[i].desDir;
            ants[i].foodStat = antInfo[i].status;
            ants[i].foodPos = antInfo[i].foodPos;

            //if (antInfo[i].status == 1) { Debug.Log(antInfo[i].foodPos); }
        }
        mainBoard.board.SetTexture(mainBoard.pen.rend);

        compBuff.Dispose();
    }
}
