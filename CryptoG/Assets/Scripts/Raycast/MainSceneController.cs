using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneController : MonoBehaviour
{
    
    [SerializeField] public Vector2 offset;
    [SerializeField] private GameObject bounder;

    private void Awake()
    {
        transform.position += (Vector3) offset;
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
                Debug.Log(rend.gameObject.name);
            }
        }
        return res;
    }

    public GameObject GetBounder() {return bounder;}
}
