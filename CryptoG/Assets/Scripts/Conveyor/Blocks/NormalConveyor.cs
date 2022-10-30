using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalConveyor : MonoBehaviour
{
    #region neccessary
    private BlockPlaceBackgroundDict blockDict;
    private ResourceList reRefList;
    private BlockUserInteraction blockUser;
    private BlockLocationFinder blockLoc;
    private BlockIOQuery blockIOQuery;

    private BlockMainSystem mainSystem;
    private BlockConveyorStorage conveyorStorage;
    private BlockConveyorP2PTransfer conveyorP2PTransfer;

    [Header("Debug")]
    [SerializeField] private bool debug = false;
    [SerializeField] private GameObject testObj;

    private void Awake()
    {
        blockDict       = FindObjectOfType<BlockPlaceBackgroundDict>();
        reRefList       = FindObjectOfType<ResourceList>();
        blockUser       = FindObjectOfType<BlockUserInteraction>();
        blockLoc        = FindObjectOfType<BlockLocationFinder>();
        blockIOQuery    = FindObjectOfType<BlockIOQuery>();

        mainSystem = GetComponent<BlockMainSystem>();
        conveyorStorage = GetComponent<BlockConveyorStorage>();
        conveyorP2PTransfer = GetComponent<BlockConveyorP2PTransfer>();
    }   

    private void Start()
    {
        if (mainSystem.isOriginal)
        {
            mainSystem.InitiateMainSystem();
        }        
        conveyorStorage.InitiateConveyorStorage(); //need reset position, so outside please

        if (!mainSystem.isOriginal)
        {
            //if (debug) { conveyorStorage.ForceAddObjectToPosition(testObj, 0); } //debug
            conveyorP2PTransfer.InitiateConveyorP2PTransfer();
            conveyorP2PTransfer.CheckAndPushFromMainOutput();
        }
    }

    private void Update()
    {
        if (!mainSystem.isOriginal)
        {
            conveyorStorage.PushResourceForward();
        }
    }
    #endregion
}
