using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Conveyor;

public class BlockMainSystem : MonoBehaviour
{
    private BlockPlaceBackgroundDict blockDict;
    private ResourceList reRefList;
    private BlockUserInteraction blockUser;
    private BlockLocationFinder blockLoc;
    private BlockIOQuery blockIOQuery;

    private void Awake()
    {
        blockDict       = FindObjectOfType<BlockPlaceBackgroundDict>();
        reRefList       = FindObjectOfType<ResourceList>();
        blockUser       = FindObjectOfType<BlockUserInteraction>();
        blockLoc        = FindObjectOfType<BlockLocationFinder>();
        blockIOQuery    = FindObjectOfType<BlockIOQuery>();
    }

    #region mainSys
    [SerializeField] private bool _isOriginal = false;
    public bool isOriginal {get {return _isOriginal;} set {_isOriginal = value;}}

    [Header("Main")]
    [SerializeField] private int _blockSize;
    public int blockSize {get {return _blockSize;} set{_blockSize = value;}}

    [SerializeField] private int _blockID;
    public int blockID {get {return _blockID;} }

    [SerializeField] private int _blockOrientation;
    public int blockOrientation {get {return _blockOrientation;} set {_blockOrientation = value;}}

    private Vector2Int _topLeftPos;
    public Vector2Int topLeftPos {get {return _topLeftPos;} set {_topLeftPos = value;}}

    [SerializeField] private List<BlockTilebasesList> _blockTile;
    public List<BlockTilebasesList> blockTile {get {return _blockTile;} set {_blockTile = value;}}

    [SerializeField] private bool _blockListToggle; 
    public bool blockListToggle {get {return _blockListToggle;} set{_blockListToggle = value;}}

    [SerializeField] private List<int> _whiteListID;
    public List<int> whiteListID {get {return _whiteListID;} set{_whiteListID = value;}}

    [SerializeField] private List<int> _blackListID;
    public List<int> blackListID {get {return _blackListID;} set{_blackListID = value;}}

    public void InitiateMainSystem() { }

    public bool BlockPlaceable(Tilemap backgroundMap, Tilemap mainMap, Vector2Int upLeftPos)
    {
        List<int> ids = new List<int>();

        //check main map
        for (int i = 0; i < blockSize; i++)
        {
            for (int j = 0; j < blockSize; j++)
            {
                Vector2Int pos = new Vector2Int(upLeftPos.x + j, upLeftPos.y - i);
                TileBase tile = mainMap.GetTile((Vector3Int) pos);
                
                if (tile) //not empty
                {return false;}
            }
        }

        // check background map
        for (int i = 0; i < blockSize; i++)
        {
            for (int j = 0; j < blockSize; j++)
            {
                Vector2Int pos = new Vector2Int(upLeftPos.x + j, upLeftPos.y - i);
                int id = blockDict.GetTileID(backgroundMap, pos);
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

        //replaceable!
        return false;
    }

    public void PlaceBlock(Tilemap backgroundMap, Tilemap mainMap, Vector2Int upLeftPos)
    {
        if (!BlockPlaceable(backgroundMap, mainMap, upLeftPos))
        {
            Debug.LogError("Not placeable!"); 
            Destroy(this.gameObject);
            return;
        }

        topLeftPos = upLeftPos; //for later use
        SetBlockOrientation(blockUser.blockMainOrientation); //rotate tiles

        for (int i = 0; i < blockSize; i++)
        {
            for (int j = 0; j < blockSize; j++)
            {
                Vector2Int pos = new Vector2Int(upLeftPos.x + j, upLeftPos.y - i);
                mainMap.SetTile((Vector3Int) pos, blockTile[i].row[j]);

                //rotate
                Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, -90f * blockOrientation), Vector3.one);
                mainMap.SetTransformMatrix((Vector3Int) (new Vector2Int(j, -i) + topLeftPos), matrix);
            }
        }
    }

    private void SetBlockOrientation(int ori)
    {
        blockOrientation = ori;
        for (int turns = 0; turns < ori; turns++)
        {
            //transpose
            for (int i = 0; i < blockSize; i++)
            {
                for (int j = 0; j < blockSize; j++)
                {
                    if (j > i) {continue;}

                    var tmp = blockTile[i].row[j];
                    blockTile[i].row[j] = blockTile[j].row[i];
                    blockTile[j].row[i] = tmp;
                }
            }

            //reverse
            for (int i = 0; i < blockSize; i++)
            {
                for (int j = 0; j < blockSize; j++)
                {
                    var tmp = blockTile[i].row[j];
                    blockTile[i].row[j] = blockTile[i].row[blockSize-i-1];
                    blockTile[i].row[blockSize-i-1] = tmp;
                }
            }
        }
    }
    #endregion
}
