using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST6 : MonoBehaviour
{
    [SerializeField] private RenderTexture rend;
    [SerializeField] private ComputeShader comp;
    [SerializeField] private ComputeShader modComp;
    [SerializeField] private Vector2Int size;

    IEnumerator Start()
    {
        rend = new RenderTexture(size.x, size.y, 24);
        rend.enableRandomWrite = true;
        rend.Create();

        int idx = comp.FindKernel("CSMain");
        comp.SetTexture(idx, "Result", rend);

        comp.Dispatch(idx, size.x / 8, size.y / 8, 1);

        //Camera.main.targetTexture = rend;

        yield return new WaitForSeconds(5f);

        int idx2 = modComp.FindKernel("CSMain");
        modComp.SetTexture(idx2, "Result", rend);

        modComp.Dispatch(idx, size.x / 8, size.y / 8, 1);
        Debug.Log("Yarr");
    }

    void Update()
    {
        
    }
}
