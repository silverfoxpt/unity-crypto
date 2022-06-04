using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using C5;

public class AStarMazeSolve : MonoBehaviour
{
    private static Dictionary<Vector2Int, int> dist;
    private static Dictionary<Vector2Int, int> hCost;
    struct node : IComparable<node>
    {
        public Vector2Int v; public int lab; 
        public node(Vector2Int v, int d) {this.v = v; this.lab = d;}

        public int CompareTo(node b)
        {
            int first = lab + hCost[this.v], sec = b.lab + hCost[b.v];
            if (first == sec) {return (hCost[this.v] < hCost[b.v]) ? -1 : 1;} //prioritize hCost
            else {return (first < sec) ? -1 : 1;}
        }
        public bool Valid() {return lab == dist[this.v];}
    }
    [SerializeField] private PathMapGenerator pathMap;
    [SerializeField] private MainBoardController board;
    [SerializeField] private float delay = 0.1f;
    [SerializeField] private Color root;
    [SerializeField] private Color neighbor;

    private Dictionary<Vector2Int, List<Vector2Int>> ne;
    private Dictionary<Vector2Int, Vector2Int> prev;
    private Vector2Int size;
    private int inf = 1000000000;

    void Start()
    {
        StartCoroutine(AStar());
    }

    IEnumerator AStar()
    {
        ne = pathMap.ne; size = pathMap.size;
        dist = new Dictionary<Vector2Int, int>();
        prev = new Dictionary<Vector2Int, Vector2Int>();
        hCost = new Dictionary<Vector2Int, int>();

        IntervalHeap<node> q = new IntervalHeap<node>(MemoryType.Normal);

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++) 
            {
                Vector2Int pos = new Vector2Int(i, j);
                hCost.Add(pos, (size.x-1-i) + (size.y-1-j));
            }
        }

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

        //real djikstra shit
        while(q.Count > 0)
        {
            node u = q.FindMin(); q.DeleteMin();
            if (!u.Valid()) {continue;}
            if (u.v == stop) {break;}

            yield return new WaitForSeconds(delay);
            var col = board.board.GetPixelDirect(u.v);
            if (col == Color.white || col == neighbor)
            {
                board.board.SetPixelDirect(u.v, root, true);
            }

            foreach (var v in ne[u.v])
            {
                if (board.board.GetPixelDirect(v) == Color.white)
                {
                    board.board.SetPixelDirect(v, neighbor, true);
                }

                if (dist[u.v] + 1 < dist[v]) //1 is cost
                {
                    dist[v] = dist[u.v] + 1;
                    prev[v] = u.v;
                    q.Add(new node(v, dist[v]));
                }
            }
        }

        //traceback
        if (dist[stop] == inf) { yield return null; } //no path
        Vector2Int cur = stop;
        while(cur != start)
        {
            if (board.board.GetPixelDirect(cur) == Color.black) {break;}
            
            board.board.SetPixelDirect(cur, Color.blue, true);
            cur = prev[cur];
        }
    }
}
