using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Conveyor;

public class BlockConveyorStorage : MonoBehaviour
{
    private BlockPlaceBackgroundDict blockDict;
    private ResourceList reRefList;
    private BlockUserInteraction blockUser;
    private BlockLocationFinder blockLoc;
    private BlockIOQuery blockIOQuery;

    private BlockMainSystem blockMainSystem;

    private void Awake()
    {
        blockDict       = FindObjectOfType<BlockPlaceBackgroundDict>();
        reRefList       = FindObjectOfType<ResourceList>();
        blockUser       = FindObjectOfType<BlockUserInteraction>();
        blockLoc        = FindObjectOfType<BlockLocationFinder>();
        blockIOQuery    = FindObjectOfType<BlockIOQuery>();

        blockMainSystem = GetComponent<BlockMainSystem>();
    }

    #region conveyorStorage
    [Header("Conveyor Storage")]
    public int capacityCheckpoints;
    public float timeToTravelBetweenCheckpoints;

    [Space(10)]
    public List<GameObject> conveyorObjects;
    public List<bool> conveyorCheckpointsMarked;
    public List<Vector2> conveyorCheckpointsPosition;
    public List<bool> conveyorResourceIsMoving;

    private float distBetweenCheckpoints;
    private float movementTimer = 0f;

    public void InitiateConveyorStorage()
    {
        conveyorCheckpointsMarked = new List<bool>();
        conveyorObjects = new List<GameObject>();
        conveyorCheckpointsPosition = new List<Vector2>();
        conveyorResourceIsMoving = new List<bool>();

        distBetweenCheckpoints = UniversalInfo.tileSize / (capacityCheckpoints);
        movementTimer = timeToTravelBetweenCheckpoints;

        for (int i = 0; i < capacityCheckpoints; i++)
        {
            conveyorCheckpointsMarked.Add(false);
            conveyorResourceIsMoving.Add(false);
            conveyorObjects.Add(null);

            //detect where this tile is - HARDCODE WARNING!!!!!!!!!!!
            Vector2Int pos = blockMainSystem.topLeftPos; 
            Vector2 posWorld = (Vector2) blockUser.mainTilemap.CellToWorld((Vector3Int) pos) + 
                new Vector2(UniversalInfo.tileSize/2f, UniversalInfo.tileSize/2f); //change to middle of cell

            Vector2 cur = Vector2.zero;
            if (blockMainSystem.blockOrientation == 0) //left to right
            {
                cur = new Vector2(posWorld.x - UniversalInfo.tileSize/2f, posWorld.y) + new Vector2(distBetweenCheckpoints * (i+1), 0f);
            }
            else if (blockMainSystem.blockOrientation == 2) //right to left
            {
                cur = new Vector2(posWorld.x + UniversalInfo.tileSize/2f, posWorld.y) - new Vector2(distBetweenCheckpoints * (i+1), 0f);
            }
            else if (blockMainSystem.blockOrientation == 1) //up to down
            {
                cur = new Vector2(posWorld.x, posWorld.y + UniversalInfo.tileSize/2f) - new Vector2(0f, distBetweenCheckpoints * (i+1));
            }
            else if (blockMainSystem.blockOrientation == 3) //down to up
            {
                cur = new Vector2(posWorld.x, posWorld.y - UniversalInfo.tileSize/2f) + new Vector2(0f, distBetweenCheckpoints * (i+1));
            }
            
            conveyorCheckpointsPosition.Add(cur);
        }
    }

    //updated in Update()
    public void PushResourceForward()
    {
        movementTimer += Time.deltaTime; 
        
        //update position if needed
        for (int i = 0; i < capacityCheckpoints-1; i++) //skip last
        {
            if (conveyorResourceIsMoving[i] && conveyorObjects[i+1])
            {
                //push!
                conveyorObjects[i+1].transform.position = Vector2.Lerp(conveyorCheckpointsPosition[i], conveyorCheckpointsPosition[i+1], 
                    movementTimer/timeToTravelBetweenCheckpoints);
            }
        }

        //reached destination! -> check if they can be moved again and let the loop repeat
        if (movementTimer >= timeToTravelBetweenCheckpoints)
        {
            //first off, reset everything
            movementTimer = 0f;
            for (int i = 0; i < capacityCheckpoints; i++) {conveyorResourceIsMoving[i] = false;}

            //then, check if items continues to be movable - GO FROM END TO START!
            for (int i = capacityCheckpoints-2; i >= 0; i--) //skip last
            {
                ForceUpdatePosition(i);
            }
        }
    }

    public bool PositionCleared(int pos)
    {
        return (!conveyorCheckpointsMarked[pos]) && (!conveyorResourceIsMoving[pos]);
    }

    //debug only
    public void ForceAddObjectToPosition(GameObject obj, int pos)
    {
        if (PositionCleared(pos))
        {
            conveyorObjects[pos] = obj;
            conveyorCheckpointsMarked[pos] = true;

            obj.transform.position = conveyorCheckpointsPosition[pos];
        }
    }

    //if first position don't have anything moving (to the second position), update it! Use for P2P transportation
    public void ForceUpdatePosition(int i, bool P2P = false)
    {
        if ((!conveyorCheckpointsMarked[i+1]) && (conveyorCheckpointsMarked[i])) //next is empty, this is filled
        {
            if (P2P) {Debug.Log(movementTimer); }
            
            conveyorCheckpointsMarked[i+1] = true;
            conveyorCheckpointsMarked[i] = false;

            conveyorObjects[i+1] = conveyorObjects[i];
            conveyorObjects[i] = null;

            conveyorResourceIsMoving[i] = true;
        }
    }
    #endregion
}
