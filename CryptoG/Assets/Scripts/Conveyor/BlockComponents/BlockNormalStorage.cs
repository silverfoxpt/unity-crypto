using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Conveyor;
using UnityEngine.Tilemaps;

public class BlockNormalStorage : MonoBehaviour
{
    private BlockPlaceBackgroundDict blockDict;
    private ResourceList reRefList;
    private BlockUserInteraction blockUser;
    private BlockLocationFinder blockLoc;
    private BlockIOQuery blockIOQuery;

    private BlockMainSystem mainSystem;

    private void Awake()
    {
        blockDict       = FindObjectOfType<BlockPlaceBackgroundDict>();
        reRefList       = FindObjectOfType<ResourceList>();
        blockUser       = FindObjectOfType<BlockUserInteraction>();
        blockLoc        = FindObjectOfType<BlockLocationFinder>();
        blockIOQuery    = FindObjectOfType<BlockIOQuery>();

        mainSystem = GetComponent<BlockMainSystem>();
    }

    #region storage
    [Header("Storage")]
    [SerializeField] private int _maxCapacity;
    public int maxCapacity {get {return _maxCapacity;}}

    [SerializeField] private List<bulkItem> _items;
    public List<bulkItem> items {get {return _items;} set {_items = value;}}

    public void AddToStorage(bulkItem item) 
    {
        foreach(var it in items)
        {
            if (it.id == item.id) //matched
            {
                it.itemCount += item.itemCount;
                return;
            }
        }
    }

    public void RemoveFromStorage(bulkItem item) 
    {
        if (!ItemAvailable(item.id, item.itemCount)) {return;} //not enough
        foreach(var it in items)
        {
            if (it.id == item.id) //matched
            {
                it.itemCount -= item.itemCount;
                return;
            }
        }
    }

    public void InitializeStorage()
    {
        foreach(var re in reRefList.resourceReferences)
        {
            bulkItem emp = new bulkItem(0, re.id);
            items.Add(emp);
        }
    }

    public bool ItemAvailable(int id, int numNeeded)
    {
        foreach(var it in items)
        {
            if (it.id == id) //matched
            {
                if (it.itemCount >= numNeeded) {return true;}
                return false;
            }
        }
        return false;
    }

    public bool StorageFull(int id) 
    {
        foreach(var it in items)
        {
            if (it.id == id) //matched
            {
                if (it.itemCount >= maxCapacity) {return true;}
                break;
            }
        }
        return false;
    }

    public bool ItemAddable(int id, int numAdd)
    {
        foreach(var it in items)
        {
            if (it.id == id) //matched
            {
                if (it.itemCount + numAdd > maxCapacity) {return false;}
                else {return true;}
            }
        }
        return false; //not needed anyway
    }
    #endregion
}
