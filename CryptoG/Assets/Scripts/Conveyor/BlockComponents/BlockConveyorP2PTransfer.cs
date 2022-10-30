using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Conveyor;

public class BlockConveyorP2PTransfer : MonoBehaviour
{
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

    [Header("Conveyor P2P")]
    public List<bool> blockInputOutput; //true -> input, false -> output

    private int[] dx = new int[4] {0, +1, 0, -1};
    private int[] dy = new int[4] {+1, 0, -1, 0};

    public int outSide, inMainSide, inExtra1, inExtra2;

    public void InitiateConveyorP2PInput()
    {
        //spin
        blockInputOutput = new List<bool>(blockInputOutput);

        int spin = blockMainSystem.blockOrientation;
        for (int i = 0; i < spin; i++) //spin the block
        {
            bool tmp = blockInputOutput[3];
            blockInputOutput[3] = blockInputOutput[2];
            blockInputOutput[2] = blockInputOutput[1];
            blockInputOutput[1] = blockInputOutput[0];
            blockInputOutput[0] = tmp;
        }

        //clear where is input/output
        for (int i = 0; i < 4; i++)
        {
            if (!blockInputOutput[i]) //output
            {
                outSide = i;
                switch(outSide)
                {
                    case 0: inMainSide = 2; inExtra1 = 1; inExtra2 = 3; break;
                    case 1: inMainSide = 3; inExtra1 = 2; inExtra2 = 0; break;
                    case 2: inMainSide = 0; inExtra1 = 1; inExtra2 = 3; break;
                    case 3: inMainSide = 1; inExtra1 = 2; inExtra2 = 0; break;
                }
            }
        }
    }

    //use on update()
    public bool InputableToMainInputSide()
    {
        return conveyorStorage.conveyorCheckpointsMarked[0];
    }

    //use on update
    public void CheckAndPushFromMainOutput()
    {
        //get next block sciprt
        Vector2Int nextPos = blockMainSystem.topLeftPos + new Vector2Int(dx[outSide], dy[outSide]);
        ScriptMediator sc = blockLoc.blockLocDict[nextPos];

        //check if next block is a conveyor
        if (sc.GetMainSystem().blockID != blockMainSystem.blockID) {return; }
    }
}
