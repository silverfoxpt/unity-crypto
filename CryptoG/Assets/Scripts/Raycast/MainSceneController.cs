using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneController : MonoBehaviour
{
    
    [SerializeField] public Vector2 offset;
    [SerializeField] private GameObject bounder;

    private void Awake()
    {
        List<LineRenderer> rends = GetAllSceneLineRenderers();
        foreach (var rend in rends)
        {
            for (int idx = 0; idx < rend.positionCount; idx++)
            {
                Vector2 pos = rend.GetPosition(idx);
                pos += offset;
                rend.SetPosition(idx, pos);
            }
        }
    }

    public List<LineRenderer> GetAllSceneLineRenderers()
    {
        List<LineRenderer> res = new List<LineRenderer>();
        foreach(Transform child in transform)
        {
            var lis = child.gameObject.GetComponent<DrawerBase>().GetAllLineRend();
            foreach(var rend in lis)
            {
                res.Add(rend);
            }
        }
        return res;
    }

    public List<singleLine> GetAllSingleLines()
    {
        List<singleLine> res = new List<singleLine>();
        foreach(Transform child in transform)
        {
            var lis = child.gameObject.GetComponent<DrawerBase>().GetAllLineCoordinates();
            foreach(var rend in lis) {res.Add(rend);}
        }
        return res;
    }

    public GameObject GetBounder() {return bounder;}
}
