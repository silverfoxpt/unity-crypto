using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

public class EquationResolve : MonoBehaviour
{
    private const string alpha = "abcdefghijklmnopqrstuvwxyz";
    private const string alphanum = "abcdefghijklmnopqrstuvwxyz0123456789.";
    private const string allowed = "abcdefghijklmnopqrstuvwxyz0123456789/*-+^().";
    void Start()
    {
        /*Dictionary<char, float> val = new Dictionary<char, float>{
            {'a', 1f},
            {'b', 2f},
            {'c', 15.5f},
        };
        Debug.Log(CalculateRaw(RPN("a*b+c"), val));*/
    }

    private int preced(char c)
    {
        if (c == '+' || c == '-') {return 1;}
        if (c == '*' || c == '/') {return 2;}
        if (c == '^') {return 3;}
        return 0;
    }

    public string RPN(string inp)
    {
        //checker
        foreach(char c in inp)
        {
            if (!(allowed.Contains(c))) {Debug.LogError("Wrong format!"); return "---------";}
        }

        string output = "";
        int idx = 0;
        Stack<char> st = new Stack<char>(); st.Push('.'); //antioverflow

        while(idx < inp.Length)
        {
            bool found = false;
            //alphanumeric
            while(idx < inp.Length && alphanum.Contains(inp[idx]))
            {
                output += inp[idx]; idx++; found = true;
            }
            if (found) { output += ' '; continue;}
            if (idx >= inp.Length) {break;}            

            // check for (
            if (inp[idx] == '(') { st.Push('('); idx++; found = true;}
            if (found) { continue;}

            //check for ^
            if (inp[idx] == '^') { st.Push('^'); idx++; found = true;}
            if (found) {continue;}

            //check for )
            if (inp[idx] == ')')
            {
                idx++;
                while(!(st.Count == 0) && (st.Peek() != '('))
                {
                    output += st.Peek(); output += ' '; st.Pop();
                }
                st.Pop(); //pop the )
                found = true;
            }
            if (found) {continue;}

            //else
            while (!(st.Count == 0) && preced(inp[idx]) <= preced(st.Peek()))
            {
                output += st.Peek(); output += ' '; st.Pop();
            }             
            st.Push(inp[idx]);
            idx++;
        }

        while(!(st.Count == 0))
        {
            output += st.Peek(); output += ' '; st.Pop();
        }

        //return
        return output;
    }

    private float GetFinalvalue(float a, float b, string exp)
    {
        if (exp == "+") {return a+b;}
        if (exp == "-") {return a-b;}
        if (exp == "*") {return a*b;}
        if (exp == "/") {return a/b;}
        if (exp == "^") {return Mathf.Pow(a, b);}
        return -1f;
    }

    public float CalculateRaw(string rpn, Dictionary<char, float> sub = null)
    {
        string[] sp = rpn.Split(' ');
        List<string> raw = new List<string>(sp);
        while (raw.Count > 0 && (raw[raw.Count-1] == " " || raw[raw.Count-1] == "" || raw[raw.Count-1] == ".")) {raw.RemoveAt(raw.Count-1);}

        //process
        Stack<string> st = new Stack<string>();
        foreach(string curStr in raw)
        {
            //is a num/var
            if (alphanum.Contains(curStr[0])) { st.Push(curStr); }

            //not
            else
            {
                string x1 = st.Pop(); string x2 = st.Pop();

                if (alpha.Contains(x1[0])) { x1 = (sub[x1[0]]).ToString();}
                float first = float.Parse(x1, CultureInfo.InvariantCulture.NumberFormat);

                if (alpha.Contains(x2[0])) { x2 = (sub[x2[0]]).ToString();}
                float sec = float.Parse(x2, CultureInfo.InvariantCulture.NumberFormat);

                st.Push(GetFinalvalue(sec, first, curStr).ToString());
            }
        }

        //return
        return float.Parse(st.Peek(), CultureInfo.InvariantCulture.NumberFormat);
    }
}
