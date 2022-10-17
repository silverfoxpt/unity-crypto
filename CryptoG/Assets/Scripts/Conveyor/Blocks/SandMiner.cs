using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Conveyor;
using UnityEngine.Tilemaps;

public class SandMiner : MonoBehaviour, IMainSystem
{
    #region mainSys
    [Header("Main")]
    [SerializeField] private int _blockSize;
    public int blockSize {get {return _blockSize;} set{_blockSize = value;}}

    [SerializeField] private List<BlockInOutList> _blockInput;
    public List<BlockInOutList> blockInput {get {return _blockInput;} set{_blockInput = value;}}

    [SerializeField] private List<BlockInOutList> _blockOutput;
    public List<BlockInOutList> blockOutput {get {return _blockOutput;} set{_blockOutput = value;}}

    [SerializeField] private List<BlockTilebasesList> _blockTile;
    public List<BlockTilebasesList> blockTile {get {return _blockTile;} set {_blockTile = value;}}

    [SerializeField] private bool _blockListToggle; 
    public bool blockListToggle {get {return _blockListToggle;} set{_blockListToggle = value;}}

    [SerializeField] private List<int> _whiteListID;
    public List<int> whiteListID {get {return _whiteListID;} set{_whiteListID = value;}}

    [SerializeField] private List<int> _blackListID;
    public List<int> blackListID {get {return _blackListID;} set{_blackListID = value;}}
    #endregion

    private BlockPlaceDict blockDict;

    private void Awake()
    {
        blockDict = FindObjectOfType<BlockPlaceDict>();
    }

    public void InitiateMainSystem()
    {

    }

    public bool BlockPlaceable(Tilemap map, Vector2Int upLeftPos)
    {
        List<int> ids = new List<int>();
        for (int i = 0; i < blockSize; i++)
        {
            for (int j = 0; j < blockSize; j++)
            {
                Vector2Int pos = new Vector2Int(upLeftPos.x + i, upLeftPos.y - j);
                int id = blockDict.GetTileID(map, pos);
                ids.Add(id);
            }
        }
        if (blockListToggle) //true use white
        {
            foreach(var id in ids)
            {
                foreach(var allowedId in whiteListID)
                {
                    if (id == allowedId) {return true;}
                }
            }
        }
        else //false use black
        {
            foreach(var id in ids)
            {
                foreach(var disallowedId in blackListID)
                {
                    if (id == disallowedId) {return false;}
                }
            }
            return true;
        }
        return false;
    }

    public void PlaceBlock(Tilemap map, Vector2Int upLeftPos)
    {
        
    }
}
