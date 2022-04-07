using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChaosUIController : MonoBehaviour
{
    [SerializeField] private ChaosGame chaos;

    [Header("UI")]
    [SerializeField] private SliderController seedNum;
    [SerializeField] private Toggle distributeEven;
    [SerializeField] private SliderController centerDist;

    [Space(15)]
    [SerializeField] private SliderController pointSize;

    [Space(15)]
    [SerializeField] private SliderController delay;
    [SerializeField] private SliderController pointLimit;
    [SerializeField] private SliderController pointMul;
    [SerializeField] private SliderController jumper;

    [Space(15)]
    [SerializeField] private Toggle avoidPrev;
    [SerializeField] private Toggle allowMid;
    [SerializeField] private Toggle allowCenter;

    public void RestartAnimation()
    {
        chaos.SetNumSeeds((int) seedNum.GetValue());
        chaos.SetDistributeEven(distributeEven.isOn);
        chaos.SetCenterDist(centerDist.GetValue());

        chaos.SetPointSize(pointSize.GetValue());

        chaos.SetDelay(delay.GetValue());
        chaos.SetPointLimit((int) pointLimit.GetValue());
        chaos.SetPointMul((int) pointMul.GetValue());
        chaos.SetJump(jumper.GetValue());

        chaos.SetAvoidPrev(avoidPrev.isOn);
        chaos.SetAllowMid(allowMid.isOn);
        chaos.SetAllowCenter(allowCenter.isOn);

        chaos.StopAllCoroutines();
        chaos.StartGame();
    }
}
