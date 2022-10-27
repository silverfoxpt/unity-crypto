using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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

    [System.Serializable]
    public struct BlockTilebasesList
    {
        public List<TileBase> row;
    }

    [System.Serializable]
    public struct BlockInOutList
    {
        public List<bool> sides;
    }


}
