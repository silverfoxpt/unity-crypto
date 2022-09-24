using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPoint
{
    public float[] dataInput;
    public float[] dataExpectedOutput;

    public DataPoint(float[] inp, float[] outp)
    {
        dataExpectedOutput = outp;
        dataInput = inp;
    }
}
