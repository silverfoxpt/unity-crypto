using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Conveyor;
using UnityEngine.Tilemaps;

public class BlockNormalInputResource : MonoBehaviour
{
    private BlockPlaceBackgroundDict blockDict;
    private ResourceList reRefList;
    private BlockUserInteraction blockUser;
    private BlockLocationFinder blockLoc;
    private BlockIOQuery blockIOQuery;

    private BlockMainSystem mainSystem;
    private BlockNormalStorage normalStorage;

    private void Awake()
    {
        blockDict       = FindObjectOfType<BlockPlaceBackgroundDict>();
        reRefList       = FindObjectOfType<ResourceList>();
        blockUser       = FindObjectOfType<BlockUserInteraction>();
        blockLoc        = FindObjectOfType<BlockLocationFinder>();
        blockIOQuery    = FindObjectOfType<BlockIOQuery>();

        mainSystem = GetComponent<BlockMainSystem>();
        normalStorage = GetComponent<BlockNormalStorage>();
    }

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
        if (!toggleInputList) //blacklist
        {
            //check all available input points 
            for (int i = 0; i < mainSystem.blockSize; i++) //4 sides
            {
                for (int j = 0; j < mainSystem.blockSize; j++)
                {
                    //get pos of current cell (of block, as it can has multiple cells)
                    Vector2Int pos = new Vector2Int(j, -i) + mainSystem.topLeftPos;

                    ResourceIOQuery queryList = blockIOQuery.RequestQuery(pos); 
                    List<SingleResourceQuery> takableItem = new List<SingleResourceQuery>();

                    foreach (SingleResourceQuery queryItem in queryList.itemInfoList)
                    {
                        bool disallowed = false;
                        bulkItem curItem = queryItem.item;

                        //check blacklist
                        foreach(var disallowedItem in inputBlackList)
                        {
                            if (curItem.id == disallowedItem.id) {disallowed = true; break;} //disallowed item
                        }
                        if (disallowed) { continue;} //is item allowed?
                        if (!normalStorage.ItemAddable(curItem.id, curItem.itemCount)) { continue;} //can item be added without storage full?
                        
                        //query to take it
                        takableItem.Add(queryItem);
                    }
                    blockIOQuery.TakeQueryAndTransferAccept(takableItem, normalStorage, queryList, pos);
                }
            }
        }
        else 
        {

        }
    }
    #endregion
}
