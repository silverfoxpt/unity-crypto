using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Awake()
    {
        blockDict       = FindObjectOfType<BlockPlaceBackgroundDict>();
        reRefList       = FindObjectOfType<ResourceList>();
        blockUser       = FindObjectOfType<BlockUserInteraction>();
        blockLoc        = FindObjectOfType<BlockLocationFinder>();
        blockIOQuery    = FindObjectOfType<BlockIOQuery>();

        blockMainSystem = GetComponent<BlockMainSystem>();
        conveyorStorage = GetComponent<BlockConveyorStorage>();
    }

    private int outSide, inMainSide, inExtra1, inExtra2;

    public void InputFromNormalBlock()
    {
        ResourceIOQuery listOfRequest = blockIOQuery.RequestQuery(blockMainSystem.topLeftPos);
    }
}
