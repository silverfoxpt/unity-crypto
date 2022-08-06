using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageBoardSpread : MonoBehaviour
{
    [Header("Params")]
    [SerializeField] private int imageSize;

    private List<List<Image>> images;

    public void CreateAllImages(Vector2Int size)
    {
        images = new List<List<Image>>();
        float startX = (size.x % 2 == 0) ? (-size.x / 2.0f + 0.5f) * imageSize : (-size.x / 2.0f) * imageSize;
        float startY = (size.y % 2 == 0) ? (size.y / 2.0f + 0.5f) * imageSize : (size.y / 2.0f) * imageSize;

        for (int i = 0; i < size.x; i++)
        {
            images.Add(new List<Image>());
            for (int j = 0; j < size.y; j++)
            {
                var newObj = new GameObject("Image Clone", typeof(Image));
                newObj.transform.SetParent(transform, false);

                Image img = newObj.GetComponent<Image>();
                images[i].Add(img);

                RectTransform rect = newObj.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(startX, startY);
                rect.sizeDelta = new Vector2(imageSize, imageSize);

                startX += imageSize;
            }
            startX = (size.x % 2 == 0) ? (-size.x / 2.0f + 0.5f) * imageSize : (-size.x / 2.0f) * imageSize;
            startY -= imageSize;
        }
    }

    public void SetTile(Vector2Int idx, TileBox tile)
    {
        images[idx.x][idx.y].sprite = tile.GetSprite();

        int rot = tile.GetRotate();
        images[idx.x][idx.y].gameObject.GetComponent<RectTransform>().Rotate(new Vector3(0f, 0f, 360 - rot * 90));
    }

    public Sprite GetImg(Vector2Int idx) {return images[idx.x][idx.y].sprite; }

    public void ClearAllImage(Vector2Int size)
    {
        for (int i = 0; i < size.y; i++)
        {
            for (int j = 0; j < size.x; j++)
            {
                images[i][j].sprite = null;
                images[i][j].gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }
    }
}
