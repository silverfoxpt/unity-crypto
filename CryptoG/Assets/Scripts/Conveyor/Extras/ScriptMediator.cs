using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Conveyor;

public class ScriptMediator : MonoBehaviour
{
    public BlockMainSystem GetMainSystem()
    {
        return gameObject.GetComponent<BlockMainSystem>();
    }

    public BlockNormalStorage GetBlockStorage()
    {
        return gameObject.GetComponent<BlockNormalStorage>();
    }

    public BlockProducer GetProducer()
    {
        return gameObject.GetComponent<BlockProducer>();
    }

    public BlockNormalInputResource GetNormalInput()
    {
        return gameObject.GetComponent<BlockNormalInputResource>();
    }

    public BlockNormalOutputResource GetNormalOutput()
    {
        return gameObject.GetComponent<BlockNormalOutputResource>();
    }

    //for conveyors
    public BlockConveyorP2PTransfer GetBlockConveyorP2PTransfer()
    {
        return gameObject.GetComponent<BlockConveyorP2PTransfer>();
    }

    public BlockConveyorStorage GetBlockConveyorStorage()
    {
        return gameObject.GetComponent<BlockConveyorStorage>();
    }
}
