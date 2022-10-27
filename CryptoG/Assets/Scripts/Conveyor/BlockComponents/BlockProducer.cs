using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Conveyor;
using UnityEngine.Tilemaps;

public class BlockProducer : MonoBehaviour
{
    private BlockPlaceBackgroundDict blockDict;
    private ResourceList reRefList;
    private BlockUserInteraction blockUser;
    private BlockLocationFinder blockLoc;
    private BlockIOQuery blockIOQuery;

    private BlockMainSystem mainSystem;
    private BlockNormalStorage normalStorage;

    private void Awake()
    {
        blockDict       = FindObjectOfType<BlockPlaceBackgroundDict>();
        reRefList       = FindObjectOfType<ResourceList>();
        blockUser       = FindObjectOfType<BlockUserInteraction>();
        blockLoc        = FindObjectOfType<BlockLocationFinder>();
        blockIOQuery    = FindObjectOfType<BlockIOQuery>();

        mainSystem = GetComponent<BlockMainSystem>();
        normalStorage = GetComponent<BlockNormalStorage>();
    }   

    #region producer
    [Header("Producer")]
    [SerializeField] private List<bulkItem> _requirements;
    public List<bulkItem> requirements {get {return _requirements;}}

    [SerializeField] private bulkItem _product;
    public bulkItem product {get {return _product;}}

    [SerializeField] private float _timeToProduce;
    public float timeToProduce {get {return _timeToProduce;}}

    [SerializeField] private float _produceTimer;
    public float produceTimer {get {return _produceTimer;} set { _produceTimer = value;}}

    public void Produce()
    {
        produceTimer += Time.deltaTime;
        if (produceTimer >= timeToProduce)
        {
            //check if able to produce
            bool able = true;
            foreach(bulkItem require in requirements)
            {
                if (!normalStorage.ItemAvailable(require.id, require.itemCount)) {able = false;}
            }
            if (normalStorage.StorageFull(product.id)) {able = false;}

            //do it
            if (able)
            {
                foreach(bulkItem require in requirements)
                {
                    normalStorage.RemoveFromStorage(require);
                }
                normalStorage.AddToStorage(product);
                produceTimer = 0f;
            }
        }
    }

    public void InitializeProducer()
    {
        produceTimer = 0f;
    }
    #endregion
}
