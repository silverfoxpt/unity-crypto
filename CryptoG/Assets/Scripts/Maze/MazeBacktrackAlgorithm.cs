using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MazeBacktrackAlgorithm : MonoBehaviour
{
    public struct cell
    {
        public int x, y;

        public cell(int a, int b) 
        {
            this.x = a; this.y = b;
        }
    }

    [Header("Setup")]
    [SerializeField] private int startX = 0;
    [SerializeField] private int startY = 0;
    [SerializeField] private float delay = 0.2f;

    [Header("Reference")]
    [SerializeField] private MazeCreator mazeCreator;

    private int side;
    private List<List<bool>> visited;
    private int[] dx = new int[4] {-1, +1, 0, 0}; 
    private int[] dy = new int[4] {0, 0, -1, +1};
    private List<int> shuffleList = new List<int>() {0, 1, 2, 3};
    private static System.Random rng = new System.Random();
    private Dictionary<int, int> oppositeWall = new Dictionary<int, int>() {
        {0, 1}, {1, 0}, //left right
        {2, 3}, {3, 2}, //up bottm
    };

    private void Start()
    {
        InitializeBoard();
        StartCoroutine(CreateMaze());
    }

    IEnumerator CreateMaze()
    {
        while (true)
        {
            yield return StartCoroutine(ScoutMazeRandom(startX, startY));

            cell newStart = GetRandomUnvisitedCell();
            if (newStart.x == -1) { break; } //no cell left
            startX = newStart.x; startY = newStart.y;
        }
        mazeCreator.LightDownAllSquare();
    }

    //prepare visited
    private void InitializeBoard()
    {
        visited = new List<List<bool>>();
        side = mazeCreator.GetSize();

        for (int idx = 0; idx < side; idx++)
        {
            visited.Add(new List<bool>());
            for (int j = 0; j < side; j++)
            {
                visited[idx].Add(false);
            }
        }
    }

    private bool withinBorder(int x, int y)
    {
        if (x < 0 || y < 0 || x >= side || y >= side) {return false;} return true;
    }

    IEnumerator ScoutMazeRandom(int curX, int curY)
    {
        yield return new WaitForSeconds(delay); //pause
        Debug.Log("Cur cell: " + curX.ToString() + " " + curY.ToString());
        visited[curX][curY] = true;
        GameObject curSquare = mazeCreator.GetSquare(curX, curY); curSquare.GetComponent<SquareWallController>().LightUpSquare();

        rng = new System.Random();
        shuffleList = shuffleList.OrderBy(a => rng.Next()).ToList();

        foreach (int idx in shuffleList)
        {
            //check
            int newX = curX + dy[idx];
            int newY = curY + dx[idx];
            if (!withinBorder(newX, newY) || visited[newX][newY]) {continue;}

            //remove border
            Debug.Log(idx.ToString() + " " + oppositeWall[idx].ToString());
            GameObject neighbor = mazeCreator.GetSquare(newX, newY);

            curSquare.GetComponent<SquareWallController>().DisableSingleBorder(idx);
            neighbor.GetComponent<SquareWallController>().DisableSingleBorder(oppositeWall[idx]);

            //visit
            yield return StartCoroutine(ScoutMazeRandom(newX, newY)); break;
        }
    }

    private cell GetRandomUnvisitedCell()
    {
        cell res = new cell(-1, -1); //no cell failsafe
        for (int idx = 0; idx < side; idx++)
        {
            bool found = false;
            for (int j = 0; j < side; j++)
            {
                if (!visited[idx][j]) {res.x = idx; res.y = j; found = true; break;}
            }
            if (found) {break;}
        }
        return res;
    }
}

