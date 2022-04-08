using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DLAggUI : MonoBehaviour
{
    [SerializeField] private DiffLimitedAgg diffLimitedAgg;

    [Header("UI")]
    [SerializeField] private SliderController pointSize;
    [SerializeField] private SliderController numWalker;
    [SerializeField] private SliderController moveMag;

    [Space(10)]
    [SerializeField] private Toggle leftSide;
    [SerializeField] private Toggle rightSide;
    [SerializeField] private Toggle topSide;
    [SerializeField] private Toggle bottomSide;

    [Space(10)]
    [SerializeField] private SliderController delay;

    public void Restart()
    {
        diffLimitedAgg.StopAllCoroutines();

        diffLimitedAgg.SetPointSize(pointSize.GetValue());
        diffLimitedAgg.SetNumWalker((int) numWalker.GetValue());
        diffLimitedAgg.SetMoveMag(moveMag.GetValue());

        diffLimitedAgg.SetLeftSpawn(leftSide.isOn);
        diffLimitedAgg.SetRightSpawn(rightSide.isOn);
        diffLimitedAgg.SetTopSpawn(topSide.isOn);
        diffLimitedAgg.SetBottomSpawn(bottomSide.isOn);

        diffLimitedAgg.SetDelay(delay.GetValue());

        diffLimitedAgg.StartAggregation();
    }
}
