using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasInfoGetter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image image;
    [SerializeField] private DrawingBoardController boardController;

    private static float canvasWidth, canvasHeight, imgWidth, imgHeight, camWidth, camHeight;
    private int imgPixelWidth, imgPixelHeight;
    private static Vector2 offset;
    private Vector2Int size;
    private static float mul, blockSize;

    private void Awake()
    {
        canvasWidth = canvas.GetComponent<CanvasScaler>().referenceResolution.x;
        canvasHeight = canvas.GetComponent<CanvasScaler>().referenceResolution.y;

        Vector2 imagePos = image.rectTransform.anchoredPosition;
        camHeight = Camera.main.orthographicSize * 2;
        camWidth = camHeight / canvasHeight * canvasWidth;

        offset = new Vector2(imagePos.x / canvasWidth * camWidth, 
                             imagePos.y / canvasHeight * camHeight); 

        //some image info
        imgPixelWidth = (int) image.rectTransform.sizeDelta.x;
        imgPixelHeight = (int) image.rectTransform.sizeDelta.y;
        
        imgWidth = imgPixelWidth / canvasWidth * camWidth;
        imgHeight = imgPixelHeight / canvasHeight * camHeight;

        mul = boardController.multiplier;
        blockSize = imgWidth / imgPixelWidth * mul;
    }

    private void Start()
    {
        size = boardController.size;
        mul = boardController.multiplier;
    }

    public static Vector2Int PosToImagePos(Vector2 pos)
    {
        Vector2 cur = pos - offset; //to 0,0 of the pic
        cur = cur + new Vector2(imgWidth/2f, imgHeight/2f); //to bottom left of pic

        if (cur.x < 0 || cur.y < 0) {return UtilityFunc.nullVecInt;} //out of pic
        int xPos = Mathf.CeilToInt(cur.x / blockSize);
        int yPos = Mathf.CeilToInt(cur.y / blockSize);

        return new Vector2Int(xPos, yPos); 
    }   
}
