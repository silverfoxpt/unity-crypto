using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoissonUIController : MonoBehaviour
{
    [SerializeField] private PoissonDiskSampler poissonDiskSampler;
    
    public void StartSim()
    {
        poissonDiskSampler.StartPoisson();
    }

    public void StopSim()
    {
        poissonDiskSampler.StopAllCoroutines();
    }
}
