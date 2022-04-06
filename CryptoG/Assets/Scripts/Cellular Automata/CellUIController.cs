using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellUIController : MonoBehaviour
{
    [SerializeField] private CellularAutomaton cellularAutomaton;

    [Header("UI")]
    [SerializeField] private SliderController width;
    [SerializeField] private SliderController height;
    [SerializeField] private SliderController level;
    [SerializeField] private SliderController cellSize;

    [SerializeField] private Toggle anim;
    [SerializeField] private SliderController delay;

    public void Restart()
    {
        cellularAutomaton.SetWidth((int) width.GetValue());
        cellularAutomaton.SetHeight((int) height.GetValue());
        cellularAutomaton.SetLevel((int) level.GetValue());
        cellularAutomaton.SetSize(cellSize.GetValue());

        cellularAutomaton.SetDelay(delay.GetValue());
        cellularAutomaton.ExecuteOrder66(anim.isOn);
    }
}
