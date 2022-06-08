using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinesweepBoard : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject cellPref;
    
    [Header("Options")]
    [SerializeField] private Vector2Int size;
    [SerializeField] private float squareSize;
    [SerializeField] private float spacing = 0.02f;

    private List<List<MineCell>> cells;

    void Start()
    {
        CreateBoard();   
    }

    private void CreateBoard()
    {
        cells = new List<List<MineCell>>();

        float startX = -size.x * squareSize / 2f, startY = size.y * squareSize / 2f;
        for (int i = 0; i < size.x; i++)
        {
            cells.Add(new List<MineCell>());
            for (int j = 0; j < size.y; j++)
            {
                var cell = Instantiate(cellPref, new Vector2(startX, startY), Quaternion.identity, transform);

                var comp = cell.GetComponent<MineCell>();
                comp.SetSize(squareSize); comp.SetText("");

                cells[i].Add(comp); startX += squareSize + spacing;
            }
            startX = -size.x * squareSize / 2f; startY -= squareSize + spacing;
        }
    }
}
