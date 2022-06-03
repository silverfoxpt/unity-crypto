using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using C5;

public class TEST5 : MonoBehaviour
{
    private static Dictionary<int, int> dist;
    class CompareInt : IComparer<int>
    {
        public int Compare(int a, int b)
        {
            return (dist[a] < dist[b]) ? -1 : 1;
        }
    }
    void Start()
    {
        IntervalHeap<int> q = new IntervalHeap<int>(new CompareInt(), MemoryType.Normal);
        dist = new Dictionary<int, int>();

        dist.Add(3, 5);
        dist.Add(1, 4);
        dist.Add(5, 1);
        dist.Add(4, 3);
        
        dist[3] = 0;
        q.Add(3); 
        q.Add(1); 
        q.Add(5); 
        q.Add(4); 

        while(q.Count > 0) 
        {
            int x = q.FindMin(); q.DeleteMin(); //
            Debug.Log(x);
        }
        
        /*while(q.Count > 0) 
        {
            int x = q.FindMin(); q.DeleteMin();
            Debug.Log(x);
        }*/
    }
}
