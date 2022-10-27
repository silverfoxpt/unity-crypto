using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Conveyor;
using UnityEngine.Tilemaps;

public class BlockUserInteraction : MonoBehaviour
{
    [Header("Main options")]
    [SerializeField] private List<GameObject> originalBlocksScript;

    [Header("User test options")]
    [SerializeField] private int userActionID = 1;
    [SerializeField] private int blockChosenID = 100; //sand miner
    [SerializeField] public int blockMainOrientation = 0;

    [Header("References")]
    [SerializeField] public Tilemap backgroundTilemap;
    [SerializeField] public Tilemap mainTilemap;

    public List<GameObject> duplicates;
    private List<BlockMainSystem> originalBlockMainInterface;

    private BlockLocationFinder blockLocate;

    private void Awake()
    {
        blockLocate = FindObjectOfType<BlockLocationFinder>();
        duplicates = new List<GameObject>();

        //save those interfaces for later use
        originalBlockMainInterface = new List<BlockMainSystem>();
        foreach(var originalBlock in originalBlocksScript)
        {
            BlockMainSystem mainSysScript = originalBlock.GetComponent<ScriptMediator>().GetMainSystem();
            originalBlockMainInterface.Add(mainSysScript);
        }
    }

    void Update()
    {
        OnUserClick();
    }

    private void OnUserClick()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            var mouseCellPos = 
                mainTilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            
            if (userActionID == 1) //create block
            {
                CreateNewBlock(mouseCellPos);
            }
        }
    }

    private void CreateNewBlock(Vector3Int mouseCellPos)
    {
        int c = 0;
        foreach (var originalBlock in originalBlocksScript)
        {
            //check id
            if (originalBlockMainInterface[c].blockID != blockChosenID) 
            { 
                c++; continue;
            }

            //duplicate original block
            var dup = Instantiate(originalBlock, Vector3.zero, Quaternion.identity, transform);
            duplicates.Add(dup);

            //start system
            BlockMainSystem mainSysScript = dup.GetComponent<ScriptMediator>().GetMainSystem();
            mainSysScript.PlaceBlock(backgroundTilemap, mainTilemap, (Vector2Int)mouseCellPos);
            mainSysScript.isOriginal = false;

            blockLocate.AddNewBlock(dup); 
            c++;
        }
    }
}
