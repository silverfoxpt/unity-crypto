using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class MineClick : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Vector2Int pos; //debug purpose
    private MinesweepBoard board;
    private MinesweepAI ai;

    private void Start()
    {
        board = FindObjectOfType<MinesweepBoard>();
        ai = FindObjectOfType<MinesweepAI>();
    }

    public void OnPointerDown(PointerEventData pointerEventData) 
    {
        if (pointerEventData.button == PointerEventData.InputButton.Left) {board.ProcessClick(0, pos); ai.PrintCell(pos);}
        else if (pointerEventData.button == PointerEventData.InputButton.Right) {board.ProcessClick(1, pos);}
    }

    public void SetPos(Vector2Int p) {pos = p;}
}
