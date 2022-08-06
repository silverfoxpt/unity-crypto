using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WFCTile")]
public class TileBox : ScriptableObject
{
    [Header("Info")]
    [SerializeField] private Sprite tileImage;
    [SerializeField] private int rotation;

    [Header("Ruleset socket")]
    [SerializeField] private string up;
    [SerializeField] private string right;
    [SerializeField] private string down;
    [SerializeField] private string left;

    #region setter
    public void SetSprite(Sprite x) {tileImage = x;}
    public void SetRotate(int r) {rotation = r;}

    public void SetUp(string u) {up = u;}
    public void SetRight(string u) {right = u;}
    public void SetDown(string u) {down = u;}
    public void SetLeft(string u) {left = u;}
    #endregion

    #region getter
    public Sprite GetSprite() {return tileImage;}
    public int GetRotate() {return rotation;}

    public string GetUp() {return up;}
    public string GetRight() {return right;}
    public string GetDown() {return down;}
    public string GetLeft() {return left;}
    #endregion
}
