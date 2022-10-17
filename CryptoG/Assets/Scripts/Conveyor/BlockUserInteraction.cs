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

    [Header("References")]
    [SerializeField] private Tilemap backgroundTilemap;
    [SerializeField] private Tilemap mainTilemap;

    private List<GameObject> duplicates;
    private List<IMainSystem> originalBlockMainInterface;

    private void Awake()
    {
        duplicates = new List<GameObject>();

        //save those interfaces for later use
        originalBlockMainInterface = new List<IMainSystem>();
        foreach(var originalBlock in originalBlocksScript)
        {
            var mainSysScript = originalBlock.GetComponent<ScriptMediator>().thisBlockScript as IMainSystem;
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
                int c = 0;
                foreach(var originalBlock in originalBlocksScript)
                {
                    //check id
                    if (originalBlockMainInterface[c].blockID != blockChosenID) {continue;}

                    //duplicate original block
                    var dup = Instantiate(originalBlock, Vector3.zero, Quaternion.identity, transform);
                    duplicates.Add(dup);

                    //start system
                    var mainSysScript = dup.GetComponent<ScriptMediator>().thisBlockScript as IMainSystem;
                    mainSysScript.PlaceBlock(backgroundTilemap, mainTilemap, (Vector2Int) mouseCellPos);

                    c++;
                }
            }
        }
    }
}
