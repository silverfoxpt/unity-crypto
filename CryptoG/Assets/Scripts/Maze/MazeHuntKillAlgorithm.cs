using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MazeHuntKillAlgorithm : MazeCreationBase
{
    struct cell
    {
        public int x,y; 

        public cell(int a, int b) {this.x = a; this.y = b; }
    }

    struct cellDir
    {
        public cell c; public int dir;

        public cellDir(cell a, int b) {this.c = a; this.dir = b;}
    }

    [Header("Set up")]
    [SerializeField] private int startX;
    [SerializeField] private int startY;
    [SerializeField] private float delay = 0.01f;

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
        CreateNewMaze();
    }

    public override void CreateNewMaze()
    {
        InitializeVars();

        StartCoroutine(CreateMaze());
    }

    IEnumerator CreateMaze()
    {
        while(!AllCellVisited()) //not filled 
        {
            //hunt
            cell hunted = new cell(-1, -1);
            for (int idx = 0; idx < side; idx++)
            {
                bool found = false;
                for (int j = 0; j < side; j++)
                {
                    int curX = idx, curY = j;
                    for (int k = 0; k < 4; k++)
                    {
                        int newX = curX + dy[k];
                        int newY = curY + dx[k];
                        if (!withinBorder(newX, newY) || visited[new cell(newX, newY)]) {continue;}

                        hunted = new cell(curX, curY); found = true; break;
                    }
                    mazeCreator.GetSquare(curX, curY).GetComponent<SquareWallController>().LightUpSquareSecondary();
                    yield return new WaitForSeconds(delay);

                    if (found) { break;}
                }
                if (found) {break;}
            }

            mazeCreator.LightDownAllSquare();

            //kill
            yield return StartCoroutine(KillAndDestroy(hunted));
            mazeCreator.LightDownAllSquare();
        }
    }

    private bool AllCellVisited()
    {
        foreach(var pair in visited)
        {
            if (!pair.Value) {return false;}
        }
        return true;
    }

    IEnumerator KillAndDestroy(cell curCell)
    {
        GameObject cur = mazeCreator.GetSquare(curCell.x, curCell.y);
        cur.GetComponent<SquareWallController>().LightUpSquare();

        visited[curCell] = true;
        int curX = curCell.x, curY = curCell.y;

        yield return new WaitForSeconds(delay);

        List<cellDir> goToable = new List<cellDir>();
        for (int k = 0; k < 4; k++)
        {
            int newX = curX + dy[k];
            int newY = curY + dx[k];

            if (!withinBorder(newX, newY) || visited[new cell(newX, newY)]) {continue;}
            goToable.Add(new cellDir(new cell(newX, newY), k));
        }
        if (goToable.Count == 0) {yield break;}

        //random walk
        rng = new System.Random();
        goToable = goToable.OrderBy(a => rng.Next()).ToList();

        //disable wall
        GameObject otherCell = mazeCreator.GetSquare(goToable[0].c.x, goToable[0].c.y);
        cur.GetComponent<SquareWallController>().DisableSingleBorder(goToable[0].dir);
        otherCell.GetComponent<SquareWallController>().DisableSingleBorder(oppositeWall[goToable[0].dir]);

        yield return StartCoroutine(KillAndDestroy(goToable[0].c));
    }   

    private bool withinBorder(int x, int y)
    {
        if (x < 0 || y < 0 || x >= side || y >= side) {return false;} return true;
    }

    private void InitializeVars()
    {
        side = mazeCreator.GetSize();
        visited = new Dictionary<cell, bool>();

        for (int idx = 0; idx < side; idx++)
        {
            for (int j = 0; j < side; j++)
            {
                visited.Add(new cell(idx, j), false);
            }
        }
    }
}
