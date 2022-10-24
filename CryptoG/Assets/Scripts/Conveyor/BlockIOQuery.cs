using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Conveyor;

public class ResourceIOQuery
{
    public List<SingleResourceQuery> itemList;

    public ResourceIOQuery() 
    {
        itemList = new List<SingleResourceQuery>();
    }
}

public struct SingleResourceQuery
{
    public bulkItem item;
    public IBlockStorage connectedStorage;

    public SingleResourceQuery(bulkItem it, IBlockStorage sto)
    {
        item = it;
        connectedStorage = sto;
    }
}

struct positionPair
{
    public Vector2Int to;
    public Vector2Int fro;

    public positionPair(Vector2Int t, Vector2Int f) {to = t; fro = f;}
}

public class BlockIOQuery : MonoBehaviour
{
    private int mapSize = UniversalInfo.mapSize;

    //note, a single query can only handle a single cell -> if multiple block input to the same cell,
    //that cell can only accept the item of the last block
    public Dictionary<Vector2Int, ResourceIOQuery> resourceQueryDict;
    private Dictionary<positionPair, bool> queryPlaced;

    private void Awake()
    {
        resourceQueryDict = new Dictionary<Vector2Int, ResourceIOQuery>();
        queryPlaced = new Dictionary<positionPair, bool>();

        for (int i = -mapSize; i <= mapSize; i++)
        {
            for (int j = -mapSize; j <= mapSize; j++)
            {
                resourceQueryDict.Add(new Vector2Int(i, j), new ResourceIOQuery());

                for (int a = -mapSize; a <= mapSize; a++)
                {
                    for (int b = -mapSize; b <= mapSize; b++)
                    {
                        queryPlaced.Add(new positionPair(new Vector2Int(i, j), new Vector2Int(a, b)), false);
                    }
                }
            }
        }
    }

    //add 1 item to query of that cell
    public void AddQuery(Vector2Int dest, Vector2Int origin, bulkItem item, IBlockStorage store)
    {
        //already a query for that origin - destination pair -> block that query
        //else marked that query
        if (queryPlaced[new positionPair(origin, dest)]) {return;}
        else {queryPlaced[new positionPair(origin, dest)] = true;}

        ResourceIOQuery newQuery = new ResourceIOQuery();
        newQuery.itemList.Add(new SingleResourceQuery(item, store));

        resourceQueryDict[dest] = newQuery;
    }

    //take reference to LIST of items from query of that cell
    public ResourceIOQuery RequestQuery(Vector2Int dest)
    {
        var tmp = resourceQueryDict[dest];
        return tmp;
    }
}
