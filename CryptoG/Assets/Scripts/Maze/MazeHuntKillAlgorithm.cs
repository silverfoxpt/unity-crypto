using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeHuntKillAlgorithm : MonoBehaviour
{
    struct cell
    {
        public int x,y; 

        public cell(int a, int b) {this.x = a; this.y = b; }
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
        InitializeVars();
    }

    private void InitializeVars()
    {
        side = mazeCreator.GetSize();
    }
}
