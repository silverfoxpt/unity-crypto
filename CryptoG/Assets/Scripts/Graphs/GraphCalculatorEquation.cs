using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GraphCalculatorEquation : MonoBehaviour
{
    [SerializeField] private GameObject userInp;

    private string userEquation = "x*x+2*x";

    private void Start()
    {
        userInp.GetComponent<TMP_InputField>().text = userEquation;
    }

    public void DrawGraphBaseOnUser()
    {
        string inp = userInp.GetComponent<TMP_InputField>().text;
        userEquation = inp;

        FindObjectOfType<GraphDrawer>().ResetGraph();
    }

    public float Function(float x)
    {
        Dictionary<char, float> refer = new Dictionary<char, float>();
        refer.Add('x', x);

        string rpnEquation = GetComponent<EquationResolve>().RPN(userEquation);
        return GetComponent<EquationResolve>().CalculateRaw(rpnEquation, refer);
    }
}
