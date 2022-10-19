using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Conveyor;
using UnityEngine.Tilemaps;

public class SandMiner : MonoBehaviour, IMainSystem, IBlockStorage, IProduce
{
    private void Awake()
    {
        blockDict = FindObjectOfType<BlockPlaceBackgroundDict>();
        reRefList = FindObjectOfType<ResourceList>();
    }

    private void Start()
    {
        InitiateMainSystem();
        InitializeStorage();
        InitializeProducer();
    }

    private void Update()
    {
        Produce();
    }

    #region mainSys
    [Header("Main")]
    [SerializeField] private int _blockSize;
    public int blockSize {get {return _blockSize;} set{_blockSize = value;}}

    [SerializeField] private int _blockID;
    public int blockID {get {return _blockID;} }

    private Vector2Int _topLeftPos;
    public Vector2Int topLeftPos {get {return _topLeftPos;} set {_topLeftPos = value;}}

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

    private BlockPlaceBackgroundDict blockDict;

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

    private ResourceList reRefList;

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
        foreach(var it in items)
        {
            if (it.id == item.id) //matched
            {
                it.itemCount -= item.itemCount;
                if (it.itemCount <= 0) {it.itemCount = 0;}
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
    #endregion
    //[Space(10)]

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
            
        }
    }

    public void InitializeProducer()
    {
        produceTimer = 0f;
    }
    #endregion
}