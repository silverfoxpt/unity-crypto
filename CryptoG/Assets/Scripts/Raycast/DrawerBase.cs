using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DrawerBase : MonoBehaviour
{
    public abstract List<LineRenderer> GetAllLineRend();
}
