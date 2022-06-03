using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBoardController : MonoBehaviour
{
    [Header("References")]
    public PenController pen;
    public DrawingBoardController board;
    public CanvasInfoGetter canvasInfo;

    public void SetBoardSize(Vector2Int size)
    {
        board.size = size;
        board.image.rectTransform.sizeDelta = size;
    }

    public void SetMultiplier(float m ) {board.multiplier = m;}

}
