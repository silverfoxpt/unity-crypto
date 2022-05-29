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
    [SerializeField] private GameObject home;
    [SerializeField] private GameObject antPref;
    [SerializeField] private ComputeShader antPath;
    public string status = "drawing";

    [Header("Drawing")]
    [SerializeField] private Color nestColor = Color.blue;
    [SerializeField] private Color foodColor = Color.green;

    [Header("Ants")]
    [SerializeField] private int numAnts = 15;
    [SerializeField] private float maxSpeed = 2;
    [SerializeField] private float steerStrength = 2;
    [SerializeField] private float wanderStrength = 0.1f;
    [SerializeField] private float detectAngle = 40;
    [SerializeField] private float detectStrength = 3;
  
    private List<AntController> ants;
    private antPackage[] antInfo;

    private void Start()
    {
        InitializeBoard();
        SetFoodColor();
        //CreateAllAnts();
    }

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

            ants.Add(con);
        }
    }

    private void InitializeBoard()
    {
        mainBoard.pen.RefreshPen();
        mainBoard.board.CreateNewBoard();
        mainBoard.pen.allowedDraw = true;
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

    private void Update()
    {
        if (status == "sim")
        {
            AntFindFood();
        }
    }

    private void AntFindFood()
    {
        for (int i = 0; i < numAnts; i++)
        {
            var ant = ants[i];

            antPackage pack = new antPackage();
            pack.pos = mainBoard.canvasInfo.PosToImagePos(ant.transform.position);
            pack.dir = ant.velocity;
            pack.status = ant.foodStat;
            pack.desDir = ant.desiredDirection;

            antInfo[i] = pack;
        }

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
        }
        mainBoard.board.SetTexture(mainBoard.pen.rend);

        compBuff.Dispose();
    }
}
