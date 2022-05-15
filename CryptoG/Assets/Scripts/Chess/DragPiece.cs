using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragPiece : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerClickHandler
{
    private Vector2 originalPos = new Vector2();
    private SpriteRenderer spriteRenderer;

    private void Awake() 
    {
        spriteRenderer = GetComponent<SpriteRenderer>();    
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        BoardController.ColorBoardNormal();
        ColorMoveable();
    }

    private void ColorMoveable()
    {
        Vector2Int boardPos = BoardController.FindPos(transform.position);
        List<Vector2Int> movesAvail = PieceMoveGenerator.GetMoves(boardPos);

        foreach (var pos in movesAvail)
        {
            BoardController.ColorSquare(pos);
        }
    }

    public void OnBeginDrag(PointerEventData pointerEventData)
    {
        originalPos = transform.position;
        spriteRenderer.sortingOrder = 2;

        BoardController.ColorBoardNormal();
        ColorMoveable();
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 mousePos = Input.mousePosition;
        transform.position = (Vector2) Camera.main.ScreenToWorldPoint(mousePos);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2Int boardPos = BoardController.FindPos(transform.position);
        Vector2Int oldBoardPos = BoardController.FindPos(originalPos);

        if (boardPos == UtilityFunc.nullVecInt || boardPos == oldBoardPos || 
            !PieceMoveGenerator.GetMoves(oldBoardPos).Contains(boardPos)) 
                { transform.position = originalPos;} //nothing happen
        else //make move, NOT COMPLETED
        {
            int info = Board.board[oldBoardPos.x, oldBoardPos.y]; 

            Board.board[boardPos.x, boardPos.y] = info;
            Board.board[oldBoardPos.x, oldBoardPos.y] = Piece.None;

            BoardController.ClearBoard();
            BoardController.BuildBoard();
        }

        spriteRenderer.sortingOrder = 1;
        BoardController.ColorBoardNormal();
    }

}
