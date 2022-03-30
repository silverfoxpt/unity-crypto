using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MazePrimAlgorithm : MazeCreationBase
{
    struct cell
    {
        public int x,y; 

        public cell(int a, int b) {this.x = a; this.y = b; }
    }

    struct cellDir
    {
        public cell c; public int idx;

        public cellDir(cell a, int b) {this.c = a; this.idx = b;}
    }

    [Header("Setup")]
    [SerializeField] private int startX = 0;
    [SerializeField] private int startY = 0;
    [SerializeField] private float delay = 0.01f;

    [Header("References")]
    [SerializeField] private MazeCreator mazeCreator;

    private List<cell> frontier; 
    private Dictionary<cell, bool> visited; // im lazy
    private float side;
    private static System.Random rng;
    private GameObject curStartCell = null;

    private int[] dx = new int[4] {-1, +1, 0, 0}; 
    private int[] dy = new int[4] {0, 0, -1, +1};
    private Dictionary<int, int> oppositeWall = new Dictionary<int, int>() {
        {0, 1}, {1, 0}, //left right
        {2, 3}, {3, 2}, //up bottm
    };

    private void Start()
    {
        CreateNewMaze();
    }

    public override void CreateNewMaze()
    {
        InitializeVars();
        StartCoroutine(CreateMaze());
    }

    IEnumerator CreateMaze()
    {
        yield return new WaitForSeconds(delay);
        List<cellDir> visitedCells;

        while (frontier.Count > 0) //not empty, search
        {
            rng = new System.Random();
            frontier = frontier.OrderBy(a => rng.Next()).ToList(); //randomize and take a front

            cell newFront = frontier[0]; frontier.Remove(newFront); 

            //change some color
            frontier.Remove(new cell(startX, startY));
            curStartCell.GetComponent<SquareWallController>().LightDownSquare();
            curStartCell = mazeCreator.GetSquare(newFront.x, newFront.y);
            curStartCell.GetComponent<SquareWallController>().LightUpSquareTertiary(); //now it's cur center

            startX = newFront.x; startY = newFront.y;
            visited[new cell(startX, startY)] = true;

            //add new frontier cell
            visitedCells = new List<cellDir>();
            for (int idx = 0; idx < 4; idx++)
            {
                int newX = startX + dy[idx];    
                int newY = startY + dx[idx];
                if (!withinBorder(newX, newY)) {continue;} 

                //find some non frontier cell to harass
                if (visited[new cell(newX, newY)] && !frontier.Contains(new cell(newX, newY)))
                {
                    visitedCells.Add(new cellDir(new cell(newX, newY), idx));
                }
                else if (!visited[new cell(newX, newY)])//not visited -> frontier
                {
                    //process some leftover
                    visited[new cell(newX, newY)] = true;

                    frontier.Add(new cell(newX, newY));
                    GameObject cel2 = mazeCreator.GetSquare(newX, newY);
                    cel2.GetComponent<SquareWallController>().LightUpSquareSecondary();
                }
            }

            //choose a visited cell (before) to merge with
            rng = new System.Random();
            visitedCells = visitedCells.OrderBy(a => rng.Next()).ToList(); //randomize 

            cellDir cel = visitedCells[0]; //there's always one cell, prim's workings
            GameObject mergeSquare = mazeCreator.GetSquare(cel.c.x, cel.c.y);
            int dir = cel.idx;

            curStartCell.GetComponent<SquareWallController>().DisableSingleBorder(dir);
            mergeSquare.GetComponent<SquareWallController>().DisableSingleBorder(oppositeWall[dir]);
            
            yield return new WaitForSeconds(delay);
        }
        mazeCreator.LightDownAllSquare();
    }

    private bool withinBorder(int x, int y)
    {
        if (x < 0 || y < 0 || x >= side || y >= side) {return false;} return true;
    }

    private void InitializeVars()
    {
        side = mazeCreator.GetSize();
        frontier = new List<cell>();
        visited = new Dictionary<cell, bool>();

        mazeCreator.GetSquare(startX, startY).GetComponent<SquareWallController>().LightUpSquareTertiary();
        curStartCell = mazeCreator.GetSquare(startX, startY);
        visited[new cell(startX, startY)] = true;

        for (int idx = 0; idx < 4; idx++)
        {
            int newX = startX + dy[idx];    
            int newY = startY + dx[idx];
            if (!withinBorder(newX, newY)) {continue;}

            frontier.Add(new cell(newX, newY));
            GameObject cel = mazeCreator.GetSquare(newX, newY);
            cel.GetComponent<SquareWallController>().LightUpSquareSecondary();
        }

        for (int idx = 0; idx < side; idx++)
        {
            for (int j = 0; j < side; j++)
            {
                visited[new cell(idx, j)] = false;
            }
        }
        visited[new cell(startX, startY)] = true;
    }
}

