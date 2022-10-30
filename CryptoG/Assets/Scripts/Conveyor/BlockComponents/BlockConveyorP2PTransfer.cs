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

    public void InitiateConveyorP2PTransfer()
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

    //used on CheckAndPushFromMainOutput()
    private bool InputableToMainInputSide(int neededInput)
    {
        return (!conveyorStorage.conveyorCheckpointsMarked[0]) && (inMainSide == neededInput); //empty && input correspond to output
    }

    //used on CheckAndPushFromMainOutput()
    private bool OutputableFromMainOutputSide()
    {
        return (conveyorStorage.conveyorCheckpointsMarked[conveyorStorage.capacityCheckpoints-1]) && //item reached last checkpoint
            (!conveyorStorage.conveyorResourceIsMoving[conveyorStorage.capacityCheckpoints-2]); //AND not moving from previous checkpoint
    }

    //use on update() -> emphasized on MAIN input/output ONLY. Side transfer would be handled on a different method
    public void CheckAndPushFromMainOutput()
    {
        //get next block sciprt
        Vector2Int nextPos = blockMainSystem.topLeftPos + new Vector2Int(dx[outSide], dy[outSide]); 
        ScriptMediator otherScript = blockLoc.blockLocDict[nextPos];

        if (!otherScript) {return;} //nothing there yet

        //check if next block is a conveyor
        if (otherScript.GetMainSystem().blockID != blockMainSystem.blockID) {return; }

        //check if NEXT conveyor can be inputed into the MAIN input -> Emphasized on MAIN
        if (!otherScript.GetBlockConveyorP2PTransfer().InputableToMainInputSide(inMainSide)) {return;} //because our inMainside would be equal to the other inMainSide

        //check if THIS conveyor have an item THAT HAD FINISHED MOVING INTO THE LAST SLOT
        if (!OutputableFromMainOutputSide()) {return;}

        //let's move it shall we!
        StartCoroutine(MoveFromThisConveyorToNextConveyor(otherScript.GetBlockConveyorStorage()));
    }

    //THIS IS A VERY INCONVIENT METHOD OF MOVING, which ONLY MOVES BECAUSE THE timeToMove is small enough AND we FORCE UPDATE the next conveyor to resolve the delay
    IEnumerator MoveFromThisConveyorToNextConveyor(BlockConveyorStorage nextConveyorStorage)
    {
        float timeToMove = conveyorStorage.timeToTravelBetweenCheckpoints;
        float curTime = 0f;

        int lastIdx = conveyorStorage.capacityCheckpoints-1;

        //approve the move
        GameObject res = conveyorStorage.conveyorObjects[lastIdx];

        conveyorStorage.conveyorObjects[lastIdx] = null; //no objects here anymore!
        conveyorStorage.conveyorCheckpointsMarked[lastIdx] = false; //no objects here anymore!

        nextConveyorStorage.conveyorObjects[0] = res; //objects here!

        //MOVE -> careful here, don't mess with storage movement
        while(curTime <= timeToMove)
        {
            curTime += Time.deltaTime;
            yield return null;

            res.transform.position = Vector2.Lerp(conveyorStorage.conveyorCheckpointsPosition[lastIdx],
                nextConveyorStorage.conveyorCheckpointsPosition[0],
                curTime/timeToMove
            );
        }
        
        //object move complete, ONLY checks when move complete, else ConveyorStorage move function will be confused very badly
        nextConveyorStorage.conveyorCheckpointsMarked[0] = true; 
        nextConveyorStorage.ForceUpdatePosition(0); //force update first position to not cause delay
    }
}
