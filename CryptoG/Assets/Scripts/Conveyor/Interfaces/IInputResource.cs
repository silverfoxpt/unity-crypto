using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Conveyor
{
    public interface IInputResource
    {
        public List<BlockInOutList> blockInput {get; set;}

        public bool toggleInputList {get; set;} //true use whitelist, vice versa
        public List<bulkItem> inputWhiteList {get;}
        public List<bulkItem> inputBlackList {get;}

        public void InputToIOQuery();
    }
}
