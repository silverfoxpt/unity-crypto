using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using C5;
using System;

public class DjikstraMazeSolve : MonoBehaviour
{
    private static Dictionary<Vector2Int, int> dist;
    struct node : IComparable<node>
    {
        public Vector2Int v; public int lab;
        public node(Vector2Int v, int d) {this.v = v; this.lab = d;}

        public int CompareTo(node b)
        {
            return (lab < b.lab) ? -1 : 1;
        }
        public bool Valid() {return lab == dist[this.v];}
    }
    [SerializeField] private PathMapGenerator pathMap;
    [SerializeField] private MainBoardController board;

    private Dictionary<Vector2Int, List<Vector2Int>> ne;
    private Dictionary<Vector2Int, Vector2Int> prev;
    private Vector2Int size;
    private int inf = 1000000000;

    private int compare(Vector2Int x, Vector2Int y) {return (dist[x] < dist[y]) ? -1 : 1;}

    struct PriorityQueueSlow
    {
        public List<Vector2Int> val;

        public void Initialize() { val = new List<Vector2Int>(); }
        public void Insert(Vector2Int v) { val.Add(v); }

        public void ApplySort()
        {
            var sorted = val.OrderBy(x => dist[x]);
            val = sorted.ToList();
        }

        public void Remove(Vector2Int v) {val.Remove(v);}
        public Vector2Int Peek() {return val[0]; }
        public bool Empty() {return val.Count == 0;}
        public bool Contain(Vector2Int v) {return val.Contains(v);}
    }

    void Start()
    {
        //DjikstraSlow();
        DjikstraFast();
    }

    private void DjikstraFast()
    {
        ne = pathMap.ne; size = pathMap.size;
        dist = new Dictionary<Vector2Int, int>();
        prev = new Dictionary<Vector2Int, Vector2Int>();

        IntervalHeap<node> q = new IntervalHeap<node>(MemoryType.Normal);

        for (int i = 0; i< size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                Vector2Int pos = new Vector2Int(i, j);
                dist.Add(pos, inf);
                prev.Add(pos, UtilityFunc.nullVecInt);

                q.Add(new node(pos, dist[pos]));
            }
        }

        Vector2Int start = new Vector2Int(0, 0);
        Vector2Int stop = new Vector2Int(size.x - 1, size.y - 1);
        dist[start] = 0; q.Add(new node(start, 0));

        //real DjikstraSlow shit
        while(q.Count > 0)
        {
            node u = q.FindMin(); q.DeleteMin();
            if (!u.Valid()) {continue;}

            foreach (var v in ne[u.v])
            {
                if (dist[u.v] + 1 < dist[v]) //1 is cost
                {
                    dist[v] = dist[u.v] + 1;
                    prev[v] = u.v;
                    q.Add(new node(v, dist[v]));
                }
            }
        }

        //traceback
        if (dist[stop] == inf) { return;} //no path
        Vector2Int cur = stop;
        while(cur != start)
        {
            board.board.SetPixelDirect(cur, Color.blue, true);
            cur = prev[cur];
        }
    }

    private void DjikstraSlow()
    {
        ne = pathMap.ne; size = pathMap.size;
        dist = new Dictionary<Vector2Int, int>();
        prev = new Dictionary<Vector2Int, Vector2Int>();
        var q = new PriorityQueueSlow(); q.Initialize();

        for (int i = 0; i< size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                Vector2Int pos = new Vector2Int(i, j);
                dist.Add(pos, inf);
                prev.Add(pos, UtilityFunc.nullVecInt);

                q.Insert(pos);
            }
        }
        Vector2Int start = new Vector2Int(0, 0);
        Vector2Int stop = new Vector2Int(size.x - 1, size.y - 1);
        dist[start] = 0;

        //real DjikstraSlow shit
        while(!q.Empty())
        {
            Vector2Int u = q.Peek(); q.Remove(u);

            foreach (var v in ne[u])
            {
                if (!q.Contain(v)) {continue;}
                if (dist[u] + 1 < dist[v]) //1 is cost
                {
                    dist[v] = dist[u] + 1;
                    prev[v] = u;
                    q.ApplySort();
                }
            }
        }

        //traceback
        if (dist[stop] == inf) {return;} //no path
        Vector2Int cur = stop;
        while(cur != start)
        {
            board.board.SetPixelDirect(cur, Color.blue, true);
            cur = prev[cur];
        }
    }
}
