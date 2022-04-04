using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CirclePackUIController : MonoBehaviour
{
    [SerializeField] private CirclePacker circlePacker;

    [Header("UI")]
    [SerializeField] private SliderController circleLimit;
    [SerializeField] private SliderController circleGrowthRate;
    [SerializeField] private SliderController circleRefreshRate;
    [SerializeField] private SliderController maxCircleSize;

    [Space(15)]
    [SerializeField] private Toggle useImage;
    [SerializeField] private Image image;

    [Space(15)]
    [SerializeField] private Toggle fastFill;
    [SerializeField] private Toggle allowOverlap;

    public void RestartCirclePack()
    {
        circlePacker.SetCircleLimit((int) circleLimit.GetValue());
        circlePacker.SetCircleGrowth(circleGrowthRate.GetValue());
        circlePacker.SetCircleRefresh(circleRefreshRate.GetValue());
        circlePacker.SetMaxCircle(maxCircleSize.GetValue());

        circlePacker.SetUseImg(useImage.isOn);
        if (image.sprite) {circlePacker.SetImg(image.sprite);}

        circlePacker.SetFastFill(fastFill.isOn);
        circlePacker.SetOverlap(allowOverlap.isOn);

        circlePacker.CirclePack();
    }
}


