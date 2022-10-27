using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Conveyor;
using UnityEngine.Tilemaps;

public class SandMiner : MonoBehaviour
{ 
    #region neccessary
    private BlockPlaceBackgroundDict blockDict;
    private ResourceList reRefList;
    private BlockUserInteraction blockUser;
    private BlockLocationFinder blockLoc;
    private BlockIOQuery blockIOQuery;

    private BlockMainSystem mainSystem;
    private BlockNormalStorage normalStorage;
    private BlockProducer producer;
    private BlockNormalOutputResource normalOutputResource;

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
        normalOutputResource = GetComponent<BlockNormalOutputResource>();
    }   

    private void Start()
    {
        if (mainSystem.isOriginal)
        {
            mainSystem.InitiateMainSystem();
            normalStorage.InitializeStorage();
            producer.InitializeProducer();
        }
    }

    private void Update()
    {
        if (!mainSystem.isOriginal)
        {
            producer.Produce();
            normalOutputResource.OutputToIOQuery(); // check if output-able
        }
    }
    #endregion
}
