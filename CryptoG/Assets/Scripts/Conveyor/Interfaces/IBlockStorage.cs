using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Conveyor
{
    [System.Serializable]
    public class bulkItem
    {
        public int itemCount;
        public int id;

        public bulkItem(int iC, int i)
        {
            itemCount = iC; id = i;
        }
    }

    public interface IBlockStorage
    {
        public int maxCapacity {get;}
        public List<bulkItem> items {get; set;}

        public void InitializeStorage();
        public void AddToStorage(bulkItem item);
        public void RemoveFromStorage(bulkItem item);
        public bool ItemAvailable(int id, int numNeeded);
        public bool StorageFull(int id);
        public bool ItemAddable(int id, int numAdd);
    }
}
