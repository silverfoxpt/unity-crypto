using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Conveyor;


public class BlockLocationFinder : MonoBehaviour
{
    private int mapRange;
    public Dictionary<Vector2Int, ScriptMediator> blockLocDict;

    private void Awake()
    {
        mapRange = UniversalInfo.mapSize;
        blockLocDict = new Dictionary<Vector2Int, ScriptMediator>();
        for (int i = -mapRange; i <= mapRange; i++)
        {
            for (int j = -mapRange; j <= mapRange; j++)
            {
                blockLocDict.Add(new Vector2Int(i, j), null);
            }
        }
    }

    public void AddNewBlock(GameObject block)
    {
        BlockMainSystem main = block.GetComponent<ScriptMediator>().GetMainSystem();
        for (int i = 0; i < main.blockSize; i++)
        {
            for (int j = 0; j < main.blockSize; j++)
            {
                Vector2Int pos = new Vector2Int(main.topLeftPos.x + j, main.topLeftPos.y - i);
                blockLocDict[pos] = block.GetComponent<ScriptMediator>();
            }
        }
    }

    public void RemoveBlock(GameObject block)
    {
        BlockMainSystem main = block.GetComponent<ScriptMediator>().GetMainSystem();
        for (int i = 0; i < main.blockSize; i++)
        {
            for (int j = 0; j < main.blockSize; j++)
            {
                Vector2Int pos = new Vector2Int(main.topLeftPos.x + j, main.topLeftPos.y - i);
                blockLocDict[pos] = null;
            }
        }
    }

    public void DebugBlock()
    {
        
    }
}
