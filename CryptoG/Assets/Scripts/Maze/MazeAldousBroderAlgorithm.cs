using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MazeAldousBroderAlgorithm : MazeCreationBase
{
    struct cell
    {
        public int x,y; 

        public cell(int a, int b) {this.x = a; this.y = b; }
    }

    [Header("Setup")]
    [SerializeField] private int startX;
    [SerializeField] private int startY;
    [SerializeField] private float delay;

    [Header("References")]
    [SerializeField] private MazeCreator mazeCreator;

    private Dictionary<cell, bool> visited;
    private int side;

    private int[] dx = new int[4] {-1, +1, 0, 0}; 
    private int[] dy = new int[4] {0, 0, -1, +1};
    private Dictionary<int, int> oppositeWall = new Dictionary<int, int>() {
        {0, 1}, {1, 0}, //left right
        {2, 3}, {3, 2}, //up bottm
    };
    private List<int> shuffler = new List<int>() {0,1,2,3};
    private static System.Random rng;
    
    void Start()
    {
        //CreateNewMaze();
    }

    public override void CreateNewMaze()
    {
        StartCoroutine(CreateMaze());
    }

    IEnumerator CreateMaze()
    {
        InitializeVars();
        yield return StartCoroutine(CreateMaze(startX, startY));
        mazeCreator.LightDownAllSquare();
    }

    IEnumerator CreateMaze(int curX, int curY)
    {
        if (mazeCreator.CheckLightUpCell() >= side*side) { yield break; }

        visited[new cell(curX, curY)] = true;
        GameObject curSquare = mazeCreator.GetSquare(curX, curY);
        curSquare.GetComponent<SquareWallController>().LightUpSquareTertiary();

        rng = new System.Random();
        shuffler = shuffler.OrderBy(a => rng.Next()).ToList(); 

        for (int idx = 0; idx < shuffler.Count; idx++)
        {
            int newX = curX + dy[shuffler[idx]];
            int newY = curY + dx[shuffler[idx]];
            if (!withinBorder(newX, newY)) {continue;}

            yield return new WaitForSeconds(delay);
            curSquare.GetComponent<SquareWallController>().LightUpSquare();

            if (!visited[new cell(newX, newY)])
            {
                //merge walls
                curSquare.GetComponent<SquareWallController>().DisableSingleBorder(shuffler[idx]);
                mazeCreator.GetSquare(newX, newY).GetComponent<SquareWallController>().DisableSingleBorder(oppositeWall[shuffler[idx]]);
            }

            yield return StartCoroutine(CreateMaze(newX, newY)); break;
        }
        if (mazeCreator.CheckLightUpCell() >= side*side) { yield break; }

        curSquare.GetComponent<SquareWallController>().LightUpSquareTertiary();
        yield return new WaitForSeconds(delay);
        curSquare.GetComponent<SquareWallController>().LightUpSquare();
    }

    private void InitializeVars()
    {
        side = mazeCreator.GetSize();
        visited = new Dictionary<cell, bool>();

        for (int idx = 0; idx < side; idx++)
        {
            for (int j = 0; j < side; j++)
            {
                visited[new cell(idx, j)] = false;
            }
        }
        visited[new cell(startX, startY)] = true;
    }

    private bool withinBorder(int x, int y)
    {
        if (x < 0 || y < 0 || x >= side || y >= side) {return false;} return true;
    }
}
