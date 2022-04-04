using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierPointController : MonoBehaviour
{
    private Vector2 topRightBounds;
    private bool draggable = true;
    private float leftBound, rightBound, topBound, bottomBound;

    public void SetBounds(Vector2 bo) 
    {
        topRightBounds = bo;
        leftBound = -Mathf.Abs(topRightBounds.x); rightBound = -leftBound;
        topBound = Mathf.Abs(topRightBounds.y); bottomBound = -topBound;

    }

    public void LockDrag() {draggable = false;}
    public void UnlockDrag() {draggable = true;}

    private void OnMouseDrag() 
    {
        if (draggable)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.x = Mathf.Clamp(mousePos.x, leftBound, rightBound);
            mousePos.y = Mathf.Clamp(mousePos.y, bottomBound, topBound);

            transform.position = mousePos;
        }
    }
}


