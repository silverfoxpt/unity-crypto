using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellboardBoardController : MonoBehaviour
{
    [Header("Options")]
    [SerializeField] private int rowCount = 3;
    [SerializeField] private int columnCount = 3;

    [Space(10)]
    [SerializeField] private float size = 1f;
    [SerializeField] private float spacing = 0.1f;

    [Space(10)]
    [SerializeField] private bool enableText = false;

    [Header("References")] 
    [SerializeField] private GameObject cellPref; 

    private List<List<CellboardCellController>> cells;

    void Start()
    {
        CreateAllCells();   
    }

    private void CreateAllCells()
    {
        float startX, startY;
        startY = size * rowCount / 2f - size/2f + spacing;
        startX = -size * columnCount / 2f + size/2f - spacing;

        cells = new List<List<CellboardCellController>>();

        for (int i = 0; i < rowCount; i++)
        {
            cells.Add(new List<CellboardCellController>());
            for (int j = 0; j < columnCount; j++)
            {
                GameObject newCell = Instantiate(cellPref, new Vector2(startX, startY), Quaternion.identity, transform);
                var con = newCell.GetComponent<CellboardCellController>();
                con.SetSize(size);
                con.SetText(" ");
                if (!enableText) {con.OffCanvas();}

                cells[i].Add(con);
                startX += size + spacing;
            }
            startX = -size * columnCount / 2f + size/2f - spacing;
            startY -= size + spacing;
        }
    }

    public void SetCellText(int x, int y, string tex)
    {
        cells[x][y].SetText(tex);
    }
}
