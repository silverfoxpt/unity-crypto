using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierUIController : MonoBehaviour
{
    [SerializeField] private QuadraticBezierCurveDrawer quadraticBezierCurveDrawer;
    [SerializeField] private CubicBezierCurveDrawer cubicBezierCurveDrawer;

    [Header("UI")]
    [SerializeField] private SliderController animDelay;
    [SerializeField] private SliderController incre;
    [SerializeField] private SliderController lineWidth;

    public void Refresh()
    {
        quadraticBezierCurveDrawer.SetAnimationDelay(animDelay.GetValue());
        quadraticBezierCurveDrawer.SetIncrements(incre.GetValue());
        quadraticBezierCurveDrawer.SetLineWidth(lineWidth.GetValue());

        cubicBezierCurveDrawer.SetAnimationDelay(animDelay.GetValue());
        cubicBezierCurveDrawer.SetIncrements(incre.GetValue());
        cubicBezierCurveDrawer.SetLineWidth(lineWidth.GetValue());

        quadraticBezierCurveDrawer.RefreshCurve();
        cubicBezierCurveDrawer.RefreshCurve();
    }

    public void RestartAnimations()
    {
        quadraticBezierCurveDrawer.StopAllCoroutines();
        cubicBezierCurveDrawer.StopAllCoroutines();

        quadraticBezierCurveDrawer.StartCoroutine(quadraticBezierCurveDrawer.DrawBezierCurveVisualGuide());
        cubicBezierCurveDrawer.StartCoroutine(cubicBezierCurveDrawer.DrawBezierCurveVisualGuide());
    }
}
