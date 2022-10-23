using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Conveyor
{
    public interface IOutputResource
    {
        public List<BlockInOutList> blockInput {get;}
        
        public bool toggleOutputList {get; set;} //true use whitelist, vice versa
        public List<bulkItem> outputWhiteList {get;}
        public List<bulkItem> outputBlackList {get;}

        public void OutputToIOQuery();
    }
}
