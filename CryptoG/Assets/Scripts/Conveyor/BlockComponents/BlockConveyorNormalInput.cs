using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Conveyor;

public class BlockConveyorNormalInput : MonoBehaviour
{
    private int[] dx = new int[4] {0, +1, 0, -1};
    private int[] dy = new int[4] {+1, 0, -1, 0};

    private BlockPlaceBackgroundDict blockDict;
    private ResourceList reRefList;
    private BlockUserInteraction blockUser;
    private BlockLocationFinder blockLoc;
    private BlockIOQuery blockIOQuery;

    private BlockMainSystem blockMainSystem;
    private BlockConveyorStorage conveyorStorage;
    private BlockConveyorP2PTransfer conveyorP2PTransfer;

    private void Awake()
    {
        blockDict       = FindObjectOfType<BlockPlaceBackgroundDict>();
        reRefList       = FindObjectOfType<ResourceList>();
        blockUser       = FindObjectOfType<BlockUserInteraction>();
        blockLoc        = FindObjectOfType<BlockLocationFinder>();
        blockIOQuery    = FindObjectOfType<BlockIOQuery>();

        blockMainSystem = GetComponent<BlockMainSystem>();
        conveyorStorage = GetComponent<BlockConveyorStorage>();
        conveyorP2PTransfer = GetComponent<BlockConveyorP2PTransfer>();
    }

    private int outSide, inMainSide, inExtra1, inExtra2;

    public void InitiateConveyorNormalInput()
    {
        outSide = conveyorP2PTransfer.outSide;
        inMainSide = conveyorP2PTransfer.inMainSide;
        inExtra1 = conveyorP2PTransfer.inExtra1;
        inExtra2 = conveyorP2PTransfer.inExtra2;
    }

    public void InputFromNormalBlock()
    {
        ResourceIOQuery listOfRequest = blockIOQuery.RequestQuery(blockMainSystem.topLeftPos);
        if (listOfRequest.itemInfoList.Count == 0) {return;} //no request

        //else check request
        List<SingleResourceQuery> toBeRemoved = new List<SingleResourceQuery>();
        foreach(var req in listOfRequest.itemInfoList)
        {
            //main input
            bool accepted = InputMainFromNormalBlock(req);
            if (accepted) { toBeRemoved.Add(req); }//remove the query and the item from storage block
        }

        blockIOQuery.TakeQueryAndRemoveFromOrigin(toBeRemoved, listOfRequest, blockMainSystem.topLeftPos);
    }

    private bool InputMainFromNormalBlock(SingleResourceQuery itemInfo)
    {
        Vector2Int hypoInput = blockMainSystem.topLeftPos + new Vector2Int(dx[inMainSide], dy[inMainSide]);

        if (hypoInput != itemInfo.origin) {return false;} //not correct position
        if (!conveyorStorage.PositionCleared(0)) {return false;} //not empty

        resourceCon reference = reRefList.GetResource(itemInfo.item.id);
        GameObject createdResource = CreateResourceVisualization(reference);

        //force add
        conveyorStorage.ForceAddObjectToPosition(createdResource, 0);
        return true;
    }

    private GameObject CreateResourceVisualization(resourceCon reference)
    {
        GameObject newResource = new GameObject("Resource Item");

        var rend = newResource.AddComponent<SpriteRenderer>();
        rend.sprite = reference.img;
        rend.sortingOrder = reRefList.sortingOrderForResource;
        
        newResource.transform.localScale = new Vector3(reRefList.imgScale, reRefList.imgScale, 1f);
        return newResource;
    }
}
