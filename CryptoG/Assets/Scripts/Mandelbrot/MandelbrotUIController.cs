using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class MandelbrotUIController : MonoBehaviour
{
    [SerializeField] private ScreenController mandelScreen;
    [SerializeField] private ScreenController juliaScreen;
    [SerializeField] private MandelbrotDrawer mandelbrot;
    [SerializeField] private JuliaDrawer julia;

    [SerializeField] private SliderController sideSlider;
    [SerializeField] private SliderController iteSlider;
    [SerializeField] private SliderController infSlider;

    public void Rebuild()
    {
        int side = (int) sideSlider.GetValue();
        int ite = (int) iteSlider.GetValue();
        float inf = infSlider.GetValue();

        //screen1 
        mandelScreen.SetWidth(side);
        mandelScreen.SetHeight(side);
        mandelScreen.RefreshScreen();

        //screen1 
        juliaScreen.SetWidth(side);
        juliaScreen.SetHeight(side);
        juliaScreen.RefreshScreen();

        //mandelControl
        mandelbrot.SetMaxIteration(ite);
        mandelbrot.SetInf(inf);
        mandelbrot.DrawMandelbrot();
        
        mandelbrot.DeleteOverlay();
        mandelbrot.AddOverlay();

        //juliaControl
        julia.GetNewVars();
        julia.UpdateJulia(Vector2.negativeInfinity, false);
    }
}
