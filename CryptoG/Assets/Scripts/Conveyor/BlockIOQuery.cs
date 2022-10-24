using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Conveyor;

public class ResourceIOQuery
{
    public List<bulkItem> ite;
    public IBlockStorage originalOutputStorage;
}

public class BlockIOQuery : MonoBehaviour
{
    private int mapSize = UniversalInfo.mapSize;

    //note, a single query can only handle a single cell -> if multiple block input to the same cell,
    //that cell can only accept the item of the last block
    public Dictionary<Vector2Int, ResourceIOQuery> resourceQueryDict;

    private void Awake()
    {
        resourceQueryDict = new Dictionary<Vector2Int, ResourceIOQuery>();

        for (int i = -mapSize; i <= mapSize; i++)
        {
            for (int j = -mapSize; j <= mapSize; j++)
            {
                resourceQueryDict.Add(new Vector2Int(i, j), null);
            }
        }
    }

    public bool QueryStillExist(Vector2Int pos)
    {
        return resourceQueryDict[pos].originalOutputStorage != null;
    }

    //add 1 item to query of that cell
    public void AddQuery(Vector2Int dest, bulkItem item, IBlockStorage store)
    {
        //no need to check if there's an old query there, as everything will recirculate after a while
        ResourceIOQuery newQuery = new ResourceIOQuery();
        newQuery.ite.Add(item);
        newQuery.originalOutputStorage = store;

        resourceQueryDict[dest] = newQuery;
    }

    //take reference to LIST of items from query of that cell
    public ResourceIOQuery RemoveQuery(Vector2Int dest)
    {
        var tmp = resourceQueryDict[dest];
        resourceQueryDict[dest] = null;

        return tmp;
    }
}
