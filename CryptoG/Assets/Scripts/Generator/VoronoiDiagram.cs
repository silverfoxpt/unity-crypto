using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiDiagram : MonoBehaviour
{
    struct point
    {
        public int xPos, yPos;
        public Color col;
    }

    [Header("References")]
    [SerializeField] private MainBoardController board;
    [SerializeField] private ComputeShader computeMin;

    [Header("Params")]
    [SerializeField] private int numCells = 20;
    [SerializeField] private int power = 1;

    private point[] points;
    private Vector2Int size;

    void Start()
    {
        InitializeBoard(); 
        InitializePoints();  

        GenerateVoronoi();
    }

    private void GenerateVoronoi()
    {
        int idx = computeMin.FindKernel("Minkowski");
        computeMin.SetTexture(idx, "map", board.pen.rend);

        int stride = sizeof(int) * 2 + sizeof(float) * 4;
        ComputeBuffer buff = new ComputeBuffer(numCells, stride);
        buff.SetData(points);
        computeMin.SetBuffer(idx, "pos", buff);

        computeMin.SetInt("power", power);
        computeMin.SetInt("numCells", numCells);

        computeMin.Dispatch(idx, size.x / 8, size.y / 8, 1);

        buff.Dispose();
        board.board.SetTexture(board.pen.rend);
    }

    private void InitializePoints()
    {
        points = new point[numCells];
        size = board.board.size;

        for (int i = 0; i < numCells; i++)
        {
            point p = new point();
            p.xPos = UnityEngine.Random.Range(0, size.x);
            p.yPos = UnityEngine.Random.Range(0, size.y);
            p.col = UtilityFunc.GetRandColor();

            points[i] = p;
        }
    }

    private void InitializeBoard()
    {
        board.board.CreateNewBoard();
        board.pen.RefreshPen();

        board.pen.allowedDraw = false;
    }
}
