using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Conveyor;
using UnityEngine.Tilemaps;

public class BlockNormalOutputResource : MonoBehaviour
{
    private BlockPlaceBackgroundDict blockDict;
    private ResourceList reRefList;
    private BlockUserInteraction blockUser;
    private BlockLocationFinder blockLoc;
    private BlockIOQuery blockIOQuery;

    private BlockMainSystem mainSystem;
    private BlockNormalStorage normalStorage;
    private BlockProducer producer;

    private void Awake()
    {
        blockDict       = FindObjectOfType<BlockPlaceBackgroundDict>();
        reRefList       = FindObjectOfType<ResourceList>();
        blockUser       = FindObjectOfType<BlockUserInteraction>();
        blockLoc        = FindObjectOfType<BlockLocationFinder>();
        blockIOQuery    = FindObjectOfType<BlockIOQuery>();

        mainSystem = GetComponent<BlockMainSystem>();
        normalStorage = GetComponent<BlockNormalStorage>();
        producer = GetComponent<BlockProducer>();
    }   

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
            foreach(var item in normalStorage.items)
            {
                if (item.itemCount <= 0) {continue;} //no item, continue
                foreach (var allowedItem in outputWhiteList)
                {
                    if (allowedItem.id != item.id) {continue;} // item not in whitelist, continue

                    //check all available output 
                    for (int i = 0; i < 4; i++) //4 sides
                    {
                        for (int j = 0; j < mainSystem.blockSize; j++)
                        {
                            if (blockOutput[i].sides[j]) //output available
                            {
                                Vector2Int pos = new Vector2Int(-100000, -100000);
                                switch(i)
                                {
                                    case 0: pos = new Vector2Int(j,                             0)                          + mainSystem.topLeftPos; break; 
                                    case 1: pos = new Vector2Int(mainSystem.blockSize-1,       -j)                          + mainSystem.topLeftPos; break; 
                                    case 2: pos = new Vector2Int(j,                             -(mainSystem.blockSize-1))  + mainSystem.topLeftPos; break;
                                    case 3: pos = new Vector2Int(0,                             -j)                         + mainSystem.topLeftPos; break;
                                }

                                blockIOQuery.AddQuery(pos + new Vector2Int(sideX[i], sideY[i]), 
                                                        pos, 
                                                        new bulkItem(1, item.id), 
                                                        normalStorage
                                );
                            }
                        }
                    }
                    //break; //found item in whitelist, no need to search the whitelist further - use for optimization, doesn't effect code flow
                }
            }
        }
        else //use blacklist
        {

        }
    }
    #endregion
}
