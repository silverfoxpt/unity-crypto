using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Conveyor
{
    [System.Serializable]
    public struct BlockTilebases
    {
        
    }

    interface IMainSystem
    {
        public int blockSize {get; set;}

        public List<List<bool>> blockOutput {get; set;}
        public List<List<bool>> blockInput {get; set;}

        public bool blockListToggle {get; set;} //true use white, false use black
        public List<int> whiteListID {get; set;}
        public List<int> blackListID {get; set;}

        public void InitiateMainSystem();
        public bool BlockPlaceable(Tilemap map, Vector2Int upLeftPos);
        public void PlaceBlock(Tilemap map, Vector2Int upLeftPos);
    }
}
