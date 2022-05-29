using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenController : MonoBehaviour
{
    [Header("Options")]
    [SerializeField] private int penType = 0; //0 circle, 1 square
    [SerializeField] private int penSize = 2;
    [SerializeField] private Color col = Color.black;
    [SerializeField] private bool eraser = false;

    [Header("References")]
    [SerializeField] private ComputeShader compShade;
    [SerializeField] private DrawingBoardController drawBoard;
    [SerializeField] private CanvasInfoGetter canvasInfo;

    private Vector2Int size;
    public RenderTexture rend;
    private Color ori;
    private bool ranned = false;
    public bool allowedDraw = false;

    private void Start()
    {
        //RefreshPen();
    }

    public void RefreshPen()
    {
        size = drawBoard.size;
        ori = drawBoard.originalColor;

        rend = new RenderTexture(size.x, size.y, 24);
        rend.enableRandomWrite = true;
        rend.Create();
    }

    void Update()
    {
        if (!ranned)
        {
            ranned = true;
            RenderTexture.active = rend;
            Graphics.Blit(drawBoard.imageTex, rend);
        }

        if (Input.GetKey(KeyCode.Mouse0) && allowedDraw) //when pressed
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int pos = canvasInfo.PosToImagePos(mousePos);

            //drawBoard.SetPixelDirect(pos); //test
            //compute shader
            compShade.SetInt("posX", pos.x);
            compShade.SetInt("posY", pos.y);
            compShade.SetInt("penType", penType);
            compShade.SetInt("penSize", penSize);
            compShade.SetVector("col", (eraser) ? ori : col);
            compShade.SetTexture(compShade.FindKernel("Draw"), "Result", rend);

            compShade.Dispatch(compShade.FindKernel("Draw"), size.x / 8, size.y / 8, 1);
            drawBoard.SetTexture(rend);
        }
    }

    public void ChangePenSize(float sz)
    {
        penSize = (int) sz;
    }

    public void ChangePenColor(Color c) {col = c;}
}
