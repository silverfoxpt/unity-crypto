using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Conveyor;

public class ScriptMediator : MonoBehaviour
{
    public MonoBehaviour thisBlockScript;

    public IMainSystem GetMainSystemInterface()
    {
        return thisBlockScript as IMainSystem;
    }

    public IBlockStorage GetBlockStorage()
    {
        return thisBlockScript as IBlockStorage;
    }

    public IProduce GetProducer()
    {
        return thisBlockScript as IProduce;
    }

    public IInputResource GetInput()
    {
        return thisBlockScript as IInputResource;
    }
}
