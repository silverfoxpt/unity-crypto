using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Conveyor
{
    interface IProduce
    {
        public List<bulkItem> requirements {get; }
        public bulkItem product {get;}

        public float timeToProduce {get; }
        public float produceTimer {get; set;}

        public void InitializeProducer(); 
        public void Produce();
    }
}
