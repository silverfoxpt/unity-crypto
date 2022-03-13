using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST : MonoBehaviour
{
    //string x = "ÅnD";
    void Start()
    {
        string x = "5 6 7";
        string[] a = x.Split(' ');
        foreach(string m in a)
        {
            //Debug.Log(m);
        }
    }
}
