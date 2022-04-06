using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellularAutomaton : MonoBehaviour
{
    [SerializeField] private GameObject cellPref;

    [Header("Options")]
    [SerializeField] private int width = 400;
    [SerializeField] private int height = 200;
    [SerializeField] private float cellSize = 0.1f;
    [SerializeField] [Range(0, 255)] private int level = 90;

    [Header("Animation options")]
    [SerializeField] private float delay = 0.1f;

    private List<List<CellController> > cells;

    public void SetWidth(int wi) {width = wi;}
    public void SetHeight(int he) {height = he;}
    public void SetSize(float sz) {cellSize = sz;}
    public void SetDelay(float del) {delay = del;}
    public void SetLevel(int lv) {level = lv;}

    private void Start()
    {
        //ExecuteOrder66(true);
    }

    public void ExecuteOrder66(bool useAnim)
    {
        DestroyEverything();
        CreateScreen();
        if (!useAnim) { CalculateNextStages(); }
        else {StartCoroutine(CalculateNextStagesAnim());}
    }

    private void CalculateNextStages()
    {
        string bin = System.Convert.ToString(level, 2); //Debug.Log(bin);
        while(bin.Length < 8) {bin = '0' + bin;}

        List<int> result = new List<int>();
        for (int idx = 0; idx < 8; idx++)
        {
            result.Insert(0, bin[idx] - '0');
        }

        //calculation
        for (int idx = 1; idx < height; idx++)
        {
            List<int> prevList = new List<int>();
            for (int j = 0; j < width; j++)
            {
                prevList.Add(cells[idx-1][j].GetColorCode());
            }

            List<int> newList = new List<int>(); newList.Add(0);
            for (int j = 1; j < width-1; j++)
            {
                int code = prevList[j+1] + prevList[j] * 2 + prevList[j-1] * 4;
                newList.Add(result[code]);
            }
            newList.Add(0);

            for (int j = 0; j < width; j++)
            {
                cells[idx][j].SetColor((newList[j] == 0) ? Color.white : Color.black);
            }
        }
    }

    IEnumerator CalculateNextStagesAnim()
    {
        string bin = System.Convert.ToString(level, 2); //Debug.Log(bin);
        while(bin.Length < 8) {bin = '0' + bin;}

        List<int> result = new List<int>();
        for (int idx = 0; idx < 8; idx++)
        {
            result.Insert(0, bin[idx] - '0');
        }

        //calculation
        for (int idx = 1; idx < height; idx++)
        {
            yield return new WaitForSeconds(delay);
            List<int> prevList = new List<int>();
            for (int j = 0; j < width; j++)
            {
                prevList.Add(cells[idx-1][j].GetColorCode());
            }

            List<int> newList = new List<int>(); newList.Add(0);
            for (int j = 1; j < width-1; j++)
            {
                int code = prevList[j+1] + prevList[j] * 2 + prevList[j-1] * 4;
                newList.Add(result[code]);
            }
            newList.Add(0);

            for (int j = 0; j < width; j++)
            {
                cells[idx][j].SetColor((newList[j] == 0) ? Color.white : Color.black);
            }
            
        }
    }

    private void CreateScreen()
    {
        cells = new List<List<CellController>>();
        Vector2 offset = new Vector2(-width/2f*cellSize, height/2f*cellSize);

        for (int idx = 0; idx < height; idx++)
        {
            cells.Add(new List<CellController>());
            for (int j = 0; j < width; j++)
            {
                GameObject newCell = Instantiate(cellPref, new Vector3(j * cellSize, -idx * cellSize) + (Vector3) offset, Quaternion.identity, transform);

                var cellCon = newCell.GetComponent<CellController>();
                cellCon.SetSize(cellSize);
                cellCon.SetColor(Color.white);

                cells[idx].Add(cellCon);
            }
        }

        cells[0][Mathf.FloorToInt(width/2f)-1].SetColor(Color.black);
    }

    private void DestroyEverything()
    {
        foreach(Transform child in transform) {Destroy(child.gameObject);}
    }
}
