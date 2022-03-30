using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleController : MonoBehaviour
{
    private void Start() 
    {
        //ChangeSize(2);    
    }
    public void ChangeSize(float s)
    {
        transform.localScale = new Vector3(s, s, 0f);
    }
}
