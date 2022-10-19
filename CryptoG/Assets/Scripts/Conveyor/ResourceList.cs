using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct resourceCon
{
    public int id;
    public string name;
    public Sprite img;
}

public class ResourceList : MonoBehaviour
{
    public List<resourceCon> resourceReferences;
}