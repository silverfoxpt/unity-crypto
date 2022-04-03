using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HilbertUIController : MonoBehaviour
{
    [SerializeField] private HilbertDrawer hilbertDrawer;

    [Header("UI")]
    [SerializeField] private SliderController hilbertLength;
    [SerializeField] private SliderController lineWidth;
    [SerializeField] private SliderController originalLength;

    [Space(15)]
    [SerializeField] private Toggle drawRepeat;
    [SerializeField] private SliderController goTo;
    [SerializeField] private SliderController repeatDelay;

    [Space(15)]
    [SerializeField] private Toggle slowLine;
    [SerializeField] private SliderController slowLineDelay;

    [Space(15)]
    [SerializeField] private Toggle drawPoint;
    [SerializeField] private Toggle paintColor;

    public void RestartHilbert()
    {
        hilbertDrawer.SetHilbertSideLength((int) Mathf.Pow(2, hilbertLength.GetValue()));
        hilbertDrawer.SetLineWidth(lineWidth.GetValue());
        hilbertDrawer.SetOriginalSideLength(originalLength.GetValue());

        hilbertDrawer.SetRepeatDraw(drawRepeat.isOn);
        hilbertDrawer.SetGoTo((int) Mathf.Pow(2, goTo.GetValue()));
        hilbertDrawer.SetDelay(repeatDelay.GetValue());

        hilbertDrawer.SetSlowLine(slowLine.isOn);
        hilbertDrawer.SetLineDelay(slowLineDelay.GetValue());

        hilbertDrawer.SetPoint(drawPoint.isOn);
        hilbertDrawer.SetPaint(paintColor.isOn);

        hilbertDrawer.DrawMain();
    }
}
