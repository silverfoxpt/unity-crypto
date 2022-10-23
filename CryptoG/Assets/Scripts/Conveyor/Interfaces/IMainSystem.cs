using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Conveyor
{
    [System.Serializable]
    public struct BlockTilebasesList
    {
        public List<TileBase> row;
    }

    [System.Serializable]
    public struct BlockInOutList
    {
        public List<int> sides;
    }

    public interface IMainSystem
    {
        public bool isOriginal {get; set;}

        public int blockSize {get; set;}
        public int blockID {get;}
        public Vector2Int topLeftPos {get; set;}

        public List<BlockInOutList> blockOutput {get; set;}

        public List<BlockTilebasesList> blockTile {get; set;}

        public bool blockListToggle {get; set;} //true use white, false use black
        public List<int> whiteListID {get; set;}
        public List<int> blackListID {get; set;}

        public void InitiateMainSystem();
        public bool BlockPlaceable(Tilemap backgroundMap, Tilemap mainMap, Vector2Int upLeftPos);
        public void PlaceBlock(Tilemap backgroundMap, Tilemap mainMap, Vector2Int upLeftPos);
    }
}
