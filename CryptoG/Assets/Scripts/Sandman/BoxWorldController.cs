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
        public int lifeTime;
        public float velocity;
        public Color col;
        public int updated;
    }

    struct cellInf
    {
        public int xPos, yPos;
    }

    [Header("Drawing board")]
    [SerializeField] private MainBoardController board;
    [SerializeField] private Vector2Int size;     
    [SerializeField] private int currentDrawID = 0;
    [SerializeField] private int penSize = 10;

    [Space(10)] [Header("Draw elements")]
    [SerializeField] private List<blockRule> colors;

    [Space(10)] 
    [Header("Compute Shader")]
    [SerializeField] private int breakRange = 16;

    [Space(10)]
    [Header("References")]
    [SerializeField] private ComputeShader applyTexComp;
    [SerializeField] private ComputeShader updateComp;

    [Space(10)]
    [Header("Others")]
    [SerializeField] private bool slowUp = true;
    [SerializeField] private float delay = 0.1f;

    [Space(10)]
    [SerializeField] private bool debug = false;
    [SerializeField] private Vector2Int debugPoint;

    private blockInfo[] blocks, writeBlocks;

    private cellInf[] spread1, spread2;

    #region startMethods
    void Start()
    {
        InitializePlayField();    
        InitializeDataArray();
        InitalizeBreakArray();

        //debug - place some test sand
        //PlaceTestBlocks();

        if (slowUp)
        {
            StartCoroutine(SlowUpdate());
        }
    }

    private void InitalizeBreakArray()
    {
        spread1 = new cellInf[size.x * size.y/2];
        spread2 = new cellInf[size.x * size.y/2];

        int c0 = 0, c1 = 0;

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                cellInf inf = new cellInf(); inf.xPos = i; inf.yPos = j;

                bool first = (i / breakRange) % 2 == 0;
                if (first) {spread1[c0] = inf; c0++;} else {spread2[c1] = inf; c1++;}
            }
        }
    }

    private void PlaceTestBlocks()
    {
        for (int i = 100 - penSize/2; i <= 100 + penSize/2; i++)
        {
            for (int j = 100 - penSize/2; j <= 100 + penSize/2; j++)
            {
                int toArr = ToOneDi(i, j); //Debug.Log(toArr);
                if (toArr < 0 || toArr >= size.x * size.y) {continue;} //skip

                if (blocks[toArr].id == -1) //air
                {
                    board.board.SetPixelDirect(new Vector2Int(i, j), Color.yellow, false);

                    //need revision
                    blockInfo newBlock = new blockInfo(); 
                    newBlock.id = currentDrawID;
                    newBlock.lifeTime = int.MaxValue;
                    newBlock.col = Color.yellow;
                    newBlock.updated = 0;

                    blocks[toArr] = newBlock;
                }
            }
        }
        board.board.ApplyColor();
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
    #endregion

    #region updateMethods
    IEnumerator SlowUpdate()
    {
        while(true)
        {
            CheckColoring();
            UpdateAllBlocks();

            //update
            WriteBlockToTex(blocks, board.pen.rend);
            board.board.SetTexture(board.pen.rend); 

            yield return new WaitForSeconds(delay);
        }
    }

    private void Update()
    {
        if (!slowUp)
        {
            CheckColoring();
            UpdateAllBlocks();

            //update
            WriteBlockToTex(writeBlocks, board.pen.rend);
            board.board.SetTexture(board.pen.rend); 

            //double buffer
            Array.Copy(writeBlocks, blocks, size.x * size.y);
        }

        if (debug)
        {
            string de = ""; int idx = ToOneDi(debugPoint.x, debugPoint.y);
            de += blocks[idx].id.ToString() + "\n";
            de += blocks[idx].col.ToString();

            Debug.Log(de);
        }
    }

    private void WriteBlockToTex(blockInfo[] bo, RenderTexture currend)
    {
        int wholeSize = sizeof(int) * 3 + sizeof(float) * 5; 
        ComputeBuffer applyBuffer = new ComputeBuffer(size.x * size.y, wholeSize);
        applyBuffer.SetData(bo);

        int kernelIdx = applyTexComp.FindKernel("ApplyTex");
        applyTexComp.SetBuffer(kernelIdx, "blocks", applyBuffer);

        applyTexComp.SetInt("sizeX", size.x);
        applyTexComp.SetInt("sizeY", size.y);

        applyTexComp.SetTexture(kernelIdx, "rend", currend);
        applyTexComp.Dispatch(kernelIdx, size.x / 16, size.y / 16, 1); //buggy?

        applyBuffer.GetData(bo); 

        applyBuffer.Dispose();
    }

    private void UpdateAllBlocks()
    {
        writeBlocks = new blockInfo[size.x * size.y];
        UpdateHalf(spread1); UpdateHalf(spread2);
    }

    private void UpdateHalf(cellInf[] curCells)
    {
        int wholeSize = sizeof(int) * 3 + sizeof(float) * 5;//Debug.Log(wholeSize);        
        ComputeBuffer updateBuffer = new ComputeBuffer(size.x * size.y, wholeSize);
        updateBuffer.SetData(blocks);

        ComputeBuffer updateWriteBuffer = new ComputeBuffer(size.x * size.y, wholeSize);
        updateWriteBuffer.SetData(writeBlocks);

        int whole2 = sizeof(int) * 2;
        ComputeBuffer updatePosBuffer = new ComputeBuffer(size.x * size.y / 2, whole2);
        updatePosBuffer.SetData(curCells);

        int kernelIdx = updateComp.FindKernel("UpTex");
        updateComp.SetBuffer(kernelIdx, "readBlock", updateBuffer);
        updateComp.SetBuffer(kernelIdx, "writeBlock", updateWriteBuffer);
        updateComp.SetBuffer(kernelIdx, "location", updatePosBuffer);

        updateComp.SetInt("sizeX", size.x);
        updateComp.SetInt("sizeY", size.y);

        updateComp.Dispatch(kernelIdx, size.x * size.y / 2 / 16, 1, 1); //buggy?
        updateWriteBuffer.GetData(writeBlocks);

        updateBuffer.Dispose();
        updateWriteBuffer.Dispose();
        updatePosBuffer.Dispose();
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
    #endregion
}
