using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Conveyor;
using UnityEngine.Tilemaps;

public class SandMiner : MonoBehaviour, IMainSystem, IBlockStorage, IProduce, IOutputResource
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
            InitializeProducer();
        }
    }

    private void Update()
    {
        if (!isOriginal)
        {
            Produce();
            OutputToIOQuery(); // check if output-able
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

    public bool ItemAddable(int id, int numAdd)
    {
        foreach(var it in items)
        {
            if (it.id == id) //matched
            {
                if (it.itemCount + numAdd > maxCapacity) {return false;}
                else {return true;}
            }
        }
        return false; //not needed anyway
    }
    #endregion
    [Space(10)]

    #region producer
    [Header("Producer")]
    [SerializeField] private List<bulkItem> _requirements;
    public List<bulkItem> requirements {get {return _requirements;}}

    [SerializeField] private bulkItem _product;
    public bulkItem product {get {return _product;}}

    [SerializeField] private float _timeToProduce;
    public float timeToProduce {get {return _timeToProduce;}}

    [SerializeField] private float _produceTimer;
    public float produceTimer {get {return _produceTimer;} set { _produceTimer = value;}}

    public void Produce()
    {
        produceTimer += Time.deltaTime;
        if (produceTimer >= timeToProduce)
        {
            //check if able to produce
            bool able = true;
            foreach(bulkItem require in requirements)
            {
                if (!ItemAvailable(require.id, require.itemCount)) {able = false;}
            }
            if (StorageFull(product.id)) {able = false;}

            //do it
            if (able)
            {
                foreach(bulkItem require in requirements)
                {
                    RemoveFromStorage(require);
                }
                AddToStorage(product);
                produceTimer = 0f;
            }
        }
    }

    public void InitializeProducer()
    {
        produceTimer = 0f;
    }
    #endregion
    [Space(10)]

    #region outputResource
    [Header("Output Resource Control")]
    [SerializeField] private List<BlockInOutList> _blockOutput;
    public List<BlockInOutList> blockOutput {get {return _blockOutput;}}

    [SerializeField] private bool _toggleOutputList;
    public bool toggleOutputList {get {return _toggleOutputList;} set {_toggleOutputList = value;}}

    [SerializeField] private List<bulkItem> _outputWhiteList;
    public List<bulkItem> outputWhiteList {get {return _outputWhiteList;} }

    [SerializeField] private List<bulkItem> _outputBlackList;
    public List<bulkItem> outputBlackList {get {return _outputBlackList;} }

    public void OutputToIOQuery()
    {
        int[] sideX = new int[4] {0, +1, 0, -1};
        int[] sideY = new int[4] {+1, 0, -1, 0};

        if (toggleOutputList) //use whitelist
        {
            foreach(var item in items)
            {
                if (item.itemCount <= 0) {continue;} //no item, continue
                foreach (var allowedItem in outputWhiteList)
                {
                    if (allowedItem.id != item.id) {continue;} // item not in whitelist, continue

                    //check all available output 
                    for (int i = 0; i < 4; i++) //4 sides
                    {
                        for (int j = 0; j < blockSize; j++)
                        {
                            if (blockOutput[i].sides[j]) //output available
                            {
                                Vector2Int pos = new Vector2Int(-100000, -100000);
                                switch(i)
                                {
                                    case 0: pos = new Vector2Int(j,                 0) + topLeftPos; break; 
                                    case 1: pos = new Vector2Int(blockSize-1,       -j) + topLeftPos; break; 
                                    case 2: pos = new Vector2Int(j,                 -(blockSize-1)) + topLeftPos; break;
                                    case 3: pos = new Vector2Int(0,               -j) + topLeftPos; break;
                                }

                                blockIOQuery.AddQuery(pos + new Vector2Int(sideX[i], sideY[i]), 
                                                        pos, 
                                                        new bulkItem(1, item.id), 
                                                        this as IBlockStorage
                                );
                            }
                        }
                    }
                    break; //found item in whitelist, no need to search the whitelist further - use for optimization, doesn't effect code flow
                }
            }
        }
        else //use blacklist
        {

        }
    }
    #endregion
}
