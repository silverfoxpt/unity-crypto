using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxWorldController : MonoBehaviour
{

    [Serializable]
    struct blockRule
    {
        public int id;
        public Color color;
    }

    struct blockInfo
    {
        public int id;
        public float lifeTime;
        public float velocity;
        public Color col;
        public int updated;
    }

    [Header("Drawing board")]
    [SerializeField] private MainBoardController board;
    [SerializeField] private Vector2Int size;
    [SerializeField] private int currentDrawID = 0;
    [SerializeField] private int penSize = 10;

    [Space(10)] [Header("Draw elements")]
    [SerializeField] private List<blockRule> colors;

    [Space(10)]
    [Header("References")]
    [SerializeField] private ComputeShader applyTexComp;

    private blockInfo[] blocks, writeBlocks;

    void Start()
    {
        InitializePlayField();    
        InitializeDataArray();
    }

    private void InitializeDataArray()
    {
        blocks = new blockInfo[size.x * size.y];

        for (int j = 0; j < size.y; j++)
        {
            for (int i = 0; i < size.x; i++)
            {
                int toArr = ToOneDi(i, j); //Debug.Log(toArr);

                blockInfo newBlock = new blockInfo(); 
                newBlock.id = -1; //air
                newBlock.lifeTime = int.MaxValue;
                newBlock.col = board.board.originalColor;
                newBlock.updated = 0;

                blocks[toArr] = newBlock;
            }
        }
    }

    private void InitializePlayField()
    {
        board.SetBoardSize(size);
        board.board.CreateNewBoard();
        board.pen.RefreshPen();

        board.pen.allowedDraw = false;
        //board.board.SetPixelDirect(new Vector2Int(100, 100), Color.black, true);
    }

    private void Update()
    {
        CheckColoring();
        UpdateAllBlocks();

        //update
        WriteBlockToTex(blocks, board.pen.rend);
        board.board.SetTexture(board.pen.rend); 
    }

    private void WriteBlockToTex(blockInfo[] bo, RenderTexture currend)
    {
        int wholeSize = sizeof(int) + sizeof(float) * 6 + sizeof(int); //Debug.Log(wholeSize);        
        ComputeBuffer applyBuffer = new ComputeBuffer(size.x * size.y, wholeSize);
        applyBuffer.SetData(bo);

        int kernelIdx = applyTexComp.FindKernel("ApplyTex");
        applyTexComp.SetBuffer(kernelIdx, "blocks", applyBuffer);

        applyTexComp.SetInt("sizeX", size.x);
        applyTexComp.SetInt("sizeY", size.y);

        applyTexComp.SetTexture(kernelIdx, "rend", currend);
        applyTexComp.Dispatch(kernelIdx, size.x / 16, size.y / 16, 1); //buggy?

        applyBuffer.Dispose();
    }

    private void UpdateAllBlocks()
    {
        
    }

    private void CheckColoring()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Color cur = new Color();
            foreach (var rul in colors)
            {
                if (rul.id == currentDrawID) { cur = rul.color; break; }
            }
            
            Vector2Int pos = board.canvasInfo.PosToImagePos(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            //draw a square
            for (int i = pos.x - penSize/2; i <= pos.x + penSize/2; i++)
            {
                for (int j = pos.y - penSize/2; j <= pos.y + penSize/2; j++)
                {
                    int toArr = ToOneDi(i, j); //Debug.Log(toArr);
                    if (toArr < 0 || toArr >= size.x * size.y) {continue;} //skip

                    if (blocks[toArr].id == -1) //air
                    {
                        board.board.SetPixelDirect(new Vector2Int(i, j), cur, false);

                        //need revision
                        blockInfo newBlock = new blockInfo(); 
                        newBlock.id = currentDrawID;
                        newBlock.lifeTime = int.MaxValue;
                        newBlock.col = cur;
                        newBlock.updated = 0;

                        blocks[toArr] = newBlock;
                    }
                }
            }
            board.board.ApplyColor();
        }
    }

    private int ToOneDi(int i, int j)
    {
        int newX = size.y - 1 - j, newY = i;
        return (size.x * newX + (newY));
    }
}
