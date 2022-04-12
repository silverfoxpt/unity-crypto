using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KochUIController : MonoBehaviour
{
    [SerializeField] private KochSnowflakeGen kochSnowflakeGen;
    [Header("UI")]
    [SerializeField] private SliderController depth;
    [SerializeField] private SliderController centerDist;

    [SerializeField] private Toggle useAnim;
    [SerializeField] private SliderController delay;

    [SerializeField] private SliderController lineWidth;

    public void Restart()
    {
        kochSnowflakeGen.SetDepth((int) depth.GetValue());
        kochSnowflakeGen.SetCenterDist(centerDist.GetValue());

        kochSnowflakeGen.SetUseAnim(useAnim.isOn);
        kochSnowflakeGen.SetDelay(delay.GetValue());

        kochSnowflakeGen.SetLineWidth(lineWidth.GetValue());

        kochSnowflakeGen.StopAllCoroutines();
        kochSnowflakeGen.GenerateKochSnowflake();
    }
}
