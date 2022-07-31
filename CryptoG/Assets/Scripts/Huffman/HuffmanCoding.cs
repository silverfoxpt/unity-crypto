using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using C5;
using System.Linq;

public class HuffmanCoding : MonoBehaviour
{
    struct cell : IComparable<cell>
    {
        public string info;
        public int freq;
        public int idx;

        public int parIdx, leftIdx, rightIdx;

        public int CompareTo(cell other)
        {
            //could be buggy;
            if (freq < other.freq) { return -1; }
            else if (freq > other.freq) {return 1; }
            return 0;
        }
    }

    [Header("Params")]
    [SerializeField] private string inputString = "bibiby";

    private List<cell> cells;
    private IntervalHeap<cell> pq = new IntervalHeap<cell>();

    private void Start()
    {
        GenerateStartCells();
        HuffmanEncode();
    }

    private void HuffmanEncode()
    {
        foreach(var c in cells)
        {
            pq.Add(c);
        }

        while(pq.Count > 1)
        {
            var leftCell = pq.DeleteMin();
            var rightCell = pq.DeleteMin();

            cell newParCell = new cell();
            newParCell.info = "NULL";
            newParCell.freq = leftCell.freq + rightCell.freq;

            newParCell.leftIdx = leftCell.idx;
            newParCell.rightIdx = rightCell.idx;
            newParCell.idx = cells.Count;

            cells.Add(newParCell);
            pq.Add(newParCell);
        }
        

        //traverse tree
        TravelHuffmanTree(cells[cells.Count - 1], "");
    }

    private void TravelHuffmanTree(cell cur, string bin)
    {
        if (cur.leftIdx == -1 && cur.rightIdx == -1)
        {
            Debug.Log(cur.info + " : " + bin); return; //print and break
        }

        if (cur.leftIdx != -1) //travel left
        {
            TravelHuffmanTree(cells[cur.leftIdx], bin + "0");
        }
        if (cur.rightIdx != -1) //travelRight
        {
            TravelHuffmanTree(cells[cur.rightIdx], bin + "1");
        }
    }

    private void GenerateStartCells()
    {
        cells = new List<cell>();
        string newS = new string(inputString.ToCharArray().Distinct().ToArray());

        for (int i = 0; i < newS.Length; i++)
        {
            string cur = "" + newS[i];

            var curCell = new cell(); 
            curCell.freq = inputString.Split(newS[i]).Length - 1;
            curCell.info = cur;
            curCell.leftIdx = curCell.rightIdx = -1; //none down
            curCell.idx = i;

            cells.Add(curCell);
        }
    }
}
