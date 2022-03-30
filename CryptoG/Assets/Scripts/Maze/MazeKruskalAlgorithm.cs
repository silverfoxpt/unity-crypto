using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MazeKruskalAlgorithm : MazeCreationBase
{
    struct cell
    {
        public int x,y; 

        public cell(int a, int b) {this.x = a; this.y = b; }
    }

    struct edge
    {
        public int idx1, idx2; public int direction;

        public edge(int a, int b, int c) {this.idx1 = a; this.idx2 = b; direction = c;}
    }

    //copied needed
    private int[] dx = new int[4] {-1, +1, 0, 0}; 
    private int[] dy = new int[4] {0, 0, -1, +1};
    private Dictionary<int, int> oppositeWall = new Dictionary<int, int>() {
        {0, 1}, {1, 0}, //left right
        {2, 3}, {3, 2}, //up bottm
    };

    [Header("Setup")]
    [SerializeField] private float delay;

    [Header("References")] 
    [SerializeField] private MazeCreator mazeCreator;

    private int side;
    private int arrSize;
    private List<int> lab;
    private Dictionary<cell, int> cellToIdx;
    private Dictionary<int, cell> idxTocell;
    private List<edge> edges;

    //path compression
    private int Parent(int x)
    {
        if (lab[x] < 0) {return x;} return lab[x] = Parent(lab[x]);
    }

    //rank by size
    private void Merge(int x, int y)
    {
        if (lab[x] > lab[y]) {(x, y) = (y, x);} //now x tree is always bigger

        lab[x] += lab[y]; //merge y -> x
        lab[y] = x;
    }

    void Start()
    {
        //CreateNewMaze();
    }

    public override void CreateNewMaze()
    {
        InitializeArrays();
        StartCoroutine(CreateMaze());
    }

    IEnumerator CreateMaze()
    {
        //shuffle edge
        var rng = new System.Random();
        edges = edges.OrderBy(a => rng.Next()).ToList();

        foreach(var ed in edges)
        {
            int p1 = ed.idx1, p2 = ed.idx2;
            int a1 = Parent(p1), a2 = Parent(p2);
            
            if (a1 != a2) //different root -> merge
            {
                Merge(a1, a2);

                //find cell
                cell c1 = idxTocell[p1], c2 = idxTocell[p2];
                GameObject first = mazeCreator.GetSquare(c1.x, c1.y), sec = mazeCreator.GetSquare(c2.x, c2.y);

                //do some magic
                first.GetComponent<SquareWallController>().DisableSingleBorder(ed.direction);
                sec.GetComponent<SquareWallController>().DisableSingleBorder(oppositeWall[ed.direction]);

                first.GetComponent<SquareWallController>().LightUpSquare();
                sec.GetComponent<SquareWallController>().LightUpSquare();

                //wait
                yield return new WaitForSeconds(delay);

                //reverse the magic
                first.GetComponent<SquareWallController>().LightDownSquare();
                sec.GetComponent<SquareWallController>().LightDownSquare();
            }
        }
    }

    private bool withinBorder(int x, int y)
    {
        if (x < 0 || y < 0 || x >= side || y >= side) {return false;} return true;
    }

    private void InitializeArrays()
    {
        //refresh vars
        lab = new List<int>();
        cellToIdx = new Dictionary<cell, int>();
        idxTocell = new Dictionary<int, cell>();
        edges = new List<edge>();

        //get size(s)
        side = mazeCreator.GetSize();
        arrSize = side*side;

        //add elements
        for (int idx = 0; idx < arrSize; idx++) {lab.Add(-1);}
        for (int idx = 0; idx < side; idx++)
        {
            for (int j = 0; j < side; j++)
            {
                cell newCell = new cell(idx, j);
                int cellIdx = side*idx + j;

                cellToIdx[newCell] = cellIdx;
                idxTocell[cellIdx] = newCell;
            }
        }

        //add all edges
        for (int idx = 0; idx < side; idx++)
        {
            for (int j = 0; j < side; j++)
            {
                int newX, newY;
                for (int k = 0; k < 4; k++)
                {
                    newX = idx + dy[k]; newY = j + dx[k];
                    if (!withinBorder(newX, newY)) {continue;}

                    int id1 = cellToIdx[new cell(idx, j)];
                    int id2 = cellToIdx[new cell(newX, newY)];

                    edges.Add(new edge(id1, id2, k));
                }
            }
        }
    }
}
