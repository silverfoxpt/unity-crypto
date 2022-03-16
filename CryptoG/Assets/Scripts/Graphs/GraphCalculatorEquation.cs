using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GraphCalculatorEquation : MonoBehaviour
{
    private string userEquation = "-----";
    private string userEquationRPN = "------";

    public void SetNewEquation(string inp)
    {
        userEquation = inp;
        userEquationRPN = GetComponent<EquationResolve>().RPN(userEquation);
    }

    public float Function(float x)
    {
        Dictionary<char, float> refer = new Dictionary<char, float>();
        refer.Add('x', x);

        return GetComponent<EquationResolve>().CalculateRaw(userEquationRPN, refer);
    }
}
