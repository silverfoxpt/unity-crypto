using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Conveyor;
using UnityEngine.Tilemaps;

public class SmallStorage : MonoBehaviour, IMainSystem, IBlockStorage, IInputResource
{
    #region neccessary
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

    private void Start()
    {
        if (isOriginal)
        {
            InitiateMainSystem();
            InitializeStorage();
        }
    }

    private void Update()
    {
        if (!isOriginal)
        {
            InputFromIOQuery(); //scan for inputs
        }
    }
    #endregion

    #region mainSys
    [SerializeField] private bool _isOriginal = false;
    public bool isOriginal {get {return _isOriginal;} set {_isOriginal = value;}}

    [Header("Main")]
    [SerializeField] private int _blockSize;
    public int blockSize {get {return _blockSize;} set{_blockSize = value;}}

    [SerializeField] private int _blockID;
    public int blockID {get {return _blockID;} }

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

    public void InitiateMainSystem() {}

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
        for (int i = 0; i < blockSize; i++)
        {
            for (int j = 0; j < blockSize; j++)
            {
                Vector2Int pos = new Vector2Int(upLeftPos.x + j, upLeftPos.y - i);
                mainMap.SetTile((Vector3Int) pos, blockTile[i].row[j]);
            }
        }
    }
    #endregion
    [Space(10)]

    #region storage
    [Header("Storage")]
    [SerializeField] private int _maxCapacity;
    public int maxCapacity {get {return _maxCapacity;}}

    [SerializeField] private List<bulkItem> _items;
    public List<bulkItem> items {get {return _items;} set {_items = value;}}

    public void AddToStorage(bulkItem item) 
    {
        foreach(var it in items)
        {
            if (it.id == item.id) //matched
            {
                it.itemCount += item.itemCount;
                return;
            }
        }
    }

    public void RemoveFromStorage(bulkItem item) 
    {
        if (!ItemAvailable(item.id, item.itemCount)) {return;} //not enough
        foreach(var it in items)
        {
            if (it.id == item.id) //matched
            {
                it.itemCount -= item.itemCount;
                return;
            }
        }
    }

    public void InitializeStorage()
    {
        foreach(var re in reRefList.resourceReferences)
        {
            bulkItem emp = new bulkItem(0, re.id);
            items.Add(emp);
        }
    }

    public bool ItemAvailable(int id, int numNeeded)
    {
        foreach(var it in items)
        {
            if (it.id == id) //matched
            {
                if (it.itemCount >= numNeeded) {return true;}
                return false;
            }
        }
        return false;
    }

    public bool StorageFull(int id) 
    {
        foreach(var it in items)
        {
            if (it.id == id) //matched
            {
                if (it.itemCount >= maxCapacity) {return true;}
                break;
            }
        }
        return false;
    }
    #endregion
    [Space(10)]

    #region inputResource
    [Header("Input Resource Control")]
    [SerializeField] private List<BlockInOutList> _blockInput;
    public List<BlockInOutList> blockInput {get {return _blockInput;} }

    [SerializeField] private bool _toggleInputList;
    public bool toggleInputList {get {return _toggleInputList;} set {_toggleInputList = value;}}

    [SerializeField] private List<bulkItem> _inputWhiteList;
    public List<bulkItem> inputWhiteList {get {return _inputWhiteList;}}

    [SerializeField] private List<bulkItem> _inputBlackList;
    public List<bulkItem> inputBlackList {get {return _inputBlackList;}}

    public void InputFromIOQuery() 
    {
        int[] sideX = new int[4] {-1, 0, +1, 0};
        int[] sideY = new int[4] {0, +1, 0, -1};
        
        
        if (!toggleInputList) //blacklist
        {
            //check all available input points 
            for (int i = 0; i < 4; i++) //4 sides
            {
                for (int j = 0; j < blockSize; j++)
                {
                    //get pos of current cell (of block, as it can has multiple cells)
                    Vector2Int pos = new Vector2Int(-100000, -100000);
                    switch(i)
                    {
                        case 0: pos = new Vector2Int(0,               j); break; 
                        case 1: pos = new Vector2Int(j,               blockSize-1); break;
                        case 2: pos = new Vector2Int(blockSize-1,     j); break; 
                        case 3: pos = new Vector2Int(j,               0); break; 
                    }

                    ResourceIOQuery queryList = blockIOQuery.RequestQuery(pos);
                    List<SingleResourceQuery> unusedItem = new List<SingleResourceQuery>();

                    foreach (SingleResourceQuery queryItem in queryList.itemList)
                    {
                        bool takeItem = false;
                        foreach(var disallowedItem in inputBlackList)
                        {
                            bulkItem curItem = queryItem.item;
                            if (curItem.id == disallowedItem.id) {continue;} //disallowed item
                            if (queryItem.connectedStorage.ItemAvailable(curItem.id, curItem.itemCount)) {continue;} //not enough item in storage of origin block

                            //take it!
                            takeItem = true;
                            items.Add(curItem); //add to destionation (my) storage
                            queryItem.connectedStorage.RemoveFromStorage(curItem); //remove from origin storage
                            break;
                        }

                        if (!takeItem) {unusedItem.Add(queryItem); }
                    }
                    queryList.itemList = unusedItem; //assign unused queries back to manager
                }
            }
        }
    }
    #endregion
}
