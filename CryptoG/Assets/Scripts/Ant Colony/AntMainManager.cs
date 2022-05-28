using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntMainManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MainBoardController mainBoard;
    [SerializeField] private GameObject home;
    [SerializeField] private GameObject antPref;
    public string status = "drawing";

    [Header("Drawing")]
    [SerializeField] private Color nestColor = Color.blue;
    [SerializeField] private Color foodColor = Color.green;

    [Header("Ants")]
    [SerializeField] private int numAnts = 15;
    [SerializeField] private float maxSpeed = 2;
    [SerializeField] private float steerStrength = 2;
    [SerializeField] private float wanderStrength = 0.1f;

    private List<AntController> ants;

    private void Start()
    {
        InitializeBoard();
        SetFoodColor();
        CreateAllAnts();
    }

    private void CreateAllAnts()
    {
        ants = new List<AntController>();
        for (int i = 0; i < numAnts; i++)
        {
            var ant = Instantiate(antPref, -home.transform.position, Quaternion.identity, transform);
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

}
