using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragPiece : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    private Vector2 originalPos = new Vector2();
    private SpriteRenderer spriteRenderer;

    private void Awake() 
    {
        spriteRenderer = GetComponent<SpriteRenderer>();    
    }

    public void OnBeginDrag(PointerEventData pointerEventData)
    {
        originalPos = transform.position;
        spriteRenderer.sortingOrder = 2;
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

        if (boardPos == UtilityFunc.nullVecInt || boardPos == oldBoardPos) { transform.position = originalPos;} //nothing happen
        else //make move, NOT COMPLETED
        {    
            int info = Board.board[oldBoardPos.x, oldBoardPos.y]; 

            Board.board[boardPos.x, boardPos.y] = info;
            Board.board[oldBoardPos.x, oldBoardPos.y] = Piece.None;

            BoardController.ClearBoard();
            BoardController.BuildBoard();
        }

        spriteRenderer.sortingOrder = 1;
    }

}
