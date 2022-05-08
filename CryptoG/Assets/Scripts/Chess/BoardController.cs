using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    [SerializeField] private GameObject squarePref;
    [SerializeField] private GameObject piecePlace;

    [Header("Colors")]
    [SerializeField] private Color blackSquare;
    [SerializeField] private Color whiteSquare;

    [Header("Pieces")]
    [SerializeField] private GameObject whiteParent;
    [SerializeField] private GameObject blackParent;

    public static List<List<GameObject>> boardSquares = new List<List<GameObject>>();
    private static Dictionary<int, GameObject> whitePieces = new Dictionary<int, GameObject>(), blackPieces = new Dictionary<int, GameObject>();

    private static List<GameObject> curBoardPieces = new List<GameObject>(); //hold current pieces on board
    private static Transform piecePlaceTransform;
    private static float acc;

    private void Start()
    {
        //InitializeBoard();
    }

    public void InitializeBoard()
    {
        PrepBoard();
        PrepPieces();
        BuildBoard();
    }

    /// <summary>
    /// Pair index of piece with its sprite
    /// </summary>
    private void PrepPieces()
    {
        for (int i = 0; i < 6; i++)
        {
            GameObject curW = whiteParent.transform.GetChild(i).gameObject;
            GameObject curB = blackParent.transform.GetChild(i).gameObject;

            whitePieces.Add(i+1, curW);
            blackPieces.Add(i+1, curB);
        }
    }

    /// <summary>
    /// Prepare an empty board. Called ONCE only.
    /// </summary>
    private void PrepBoard()
    {
        piecePlaceTransform = piecePlace.transform;

        acc = squarePref.transform.localScale.x; 
        Vector2 iniPos = new Vector2(-4 * acc, -4 * acc) + new Vector2(acc/2, acc/2);

        for (int files = 0; files < 8; files++) //left to right
        {
            boardSquares.Add(new List<GameObject>());
            for (int ranks = 0; ranks < 8; ranks++) //bottom to top
            {
                var sq = Instantiate(squarePref, iniPos + new Vector2(files * acc, ranks * acc), Quaternion.identity, transform);
                boardSquares[files].Add(sq);

                bool isWhite = (files + ranks) % 2 != 0; 
                Color col = (isWhite) ? whiteSquare : blackSquare;
                sq.GetComponent<SpriteRenderer>().color = col;
            }
        }
    }

    /// <summary>
    /// Fill (graphical) board with pieces (sprites) with info taken from (info) board
    /// </summary>
    public static void BuildBoard()
    {
        for (int files = 0; files < 8; files++) //left to right
        {
            for (int ranks = 0; ranks < 8; ranks++) //bottom to top
            {
                CreatePiece(new Vector2Int(files, ranks), Board.board[files, ranks]);
            }
        }
    }

    /// <summary>
    /// Clear the (graphical) board of pieces
    /// </summary>
    public static void ClearBoard()
    {
        foreach(var piece in curBoardPieces)
        {
            Destroy(piece);
        }
        curBoardPieces = new List<GameObject>();
    }

    /// <summary>
    /// Create a piece at position pos and piece's information info
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="info"></param>
    private static void CreatePiece(Vector2Int pos, int info)
    {
        Vector2Int breakdownInfo = Piece.PieceInfo(info);

        int color = breakdownInfo.x, piece = breakdownInfo.y;
        if (piece == Piece.None) {return;} //nothing there

        var newPiece = Instantiate(
            (color == Piece.White) ? whitePieces[piece] : blackPieces[piece], 
            boardSquares[pos.x][pos.y].transform.position, 
            Quaternion.identity,
            piecePlaceTransform
        );
        curBoardPieces.Add(newPiece);
    }

    public static Vector2Int FindPos(Vector2 pos)
    {
        Vector2 halfBoard = new Vector2(acc * 4, acc * 4);
        float full = acc * 8;

        Vector2 newPos = pos + halfBoard;
        if (newPos.x <= 0 || newPos.y <= 0 || newPos.x >= full || newPos.y >= full) { return UtilityFunc.nullVecInt;}

        return new Vector2Int(Mathf.FloorToInt(newPos.x / acc), Mathf.FloorToInt(newPos.y / acc));
    }
}
