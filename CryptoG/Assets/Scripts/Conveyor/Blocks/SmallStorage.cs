using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Conveyor;
using UnityEngine.Tilemaps;

public class SmallStorage : MonoBehaviour
{
    #region neccessary
    private BlockPlaceBackgroundDict blockDict;
    private ResourceList reRefList;
    private BlockUserInteraction blockUser;
    private BlockLocationFinder blockLoc;
    private BlockIOQuery blockIOQuery;

    private BlockMainSystem mainSystem;
    private BlockNormalStorage normalStorage;
    private BlockNormalInputResource normalInputResource;

    private void Awake()
    {
        blockDict       = FindObjectOfType<BlockPlaceBackgroundDict>();
        reRefList       = FindObjectOfType<ResourceList>();
        blockUser       = FindObjectOfType<BlockUserInteraction>();
        blockLoc        = FindObjectOfType<BlockLocationFinder>();
        blockIOQuery    = FindObjectOfType<BlockIOQuery>();

        mainSystem = GetComponent<BlockMainSystem>();
        normalStorage = GetComponent<BlockNormalStorage>();
        normalInputResource = GetComponent<BlockNormalInputResource>();
    }   

    private void Start()
    {
        if (mainSystem.isOriginal)
        {
            mainSystem.InitiateMainSystem();
            normalStorage.InitializeStorage();
        }
    }

    private void Update()
    {
        if (!mainSystem.isOriginal)
        {
            normalInputResource.InputFromIOQuery(); // check if output-able
        }
    }
    #endregion
}

