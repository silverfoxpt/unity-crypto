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

    /*[SerializeField] private GameObject testObj;
    [SerializeField] private GameObject testObj2;
    private void Start() //test ONLY
    { 
        InitiateConveyorStorage();
        if (!blockMainSystem.isOriginal) {ForceAddObjectToPosition(testObj, 0); ForceAddObjectToPosition(testObj2, 2); }
    }
    private void Update() {if (!blockMainSystem.isOriginal) {PushResourceForward();}}*/

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
            Vector2 posWorld = (Vector2) blockUser.mainTilemap.CellToWorld((Vector3Int) pos) + new Vector2(UniversalInfo.tileSize/2f, UniversalInfo.tileSize/2f); 

            Vector2 cur = new Vector2(posWorld.x - UniversalInfo.tileSize/2f, posWorld.y) + new Vector2(distBetweenCheckpoints * (i+1), 0f);
            Debug.Log(cur);
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
            //oh, before that, reset resource position to avoid any discrepancies
            for (int i = 0; i < capacityCheckpoints; i++) 
            {
                if (!conveyorObjects[i]) {continue;}
                conveyorObjects[i].transform.position = conveyorCheckpointsPosition[i];
            }

            //first off, reset everything
            movementTimer = 0f;
            for (int i = 0; i < capacityCheckpoints; i++) {conveyorResourceIsMoving[i] = false;}

            //then, check if items continues to be movable - GO FROM END TO START!
            for (int i = capacityCheckpoints-2; i >= 0; i--) //skip last
            {
                if (!conveyorCheckpointsMarked[i+1] && conveyorCheckpointsMarked[i]) //next is empty, this is filled
                {
                    conveyorCheckpointsMarked[i+1] = true;
                    conveyorCheckpointsMarked[i] = false;

                    conveyorObjects[i+1] = conveyorObjects[i];
                    conveyorObjects[i] = null;

                    conveyorResourceIsMoving[i] = true;
                }
            }
        }
    }

    public bool PositionCleared(int pos)
    {
        return !conveyorCheckpointsMarked[pos];
    }

    public void ForceAddObjectToPosition(GameObject obj, int pos)
    {
        if (PositionCleared(pos))
        {
            conveyorObjects[pos] = obj;
            conveyorCheckpointsMarked[pos] = true;

            obj.transform.position = conveyorCheckpointsPosition[pos];
        }
    }
    #endregion
}