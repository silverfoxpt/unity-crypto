using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

public class EquationResolve : MonoBehaviour
{
    private const string alpha = "abcdefghijklmnopqrstuvwxyzxyz";
    private const string alphanum = "abcdefghijklmnopqrstuvwxyzxyz0123456789.";
    private const string allowed = "abcdefghijklmnopqrstuvwxyz0123456789/*-+^().";
    private const string regularOperators = "+-*/^";
    private const string irregularOperators = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private Dictionary<string, string> replacementFunctions = new Dictionary<string, string>(){
        {"sqrt", "A"},
        {"cbrt", "B"},
        {"ln", "C"},
        {"log10", "D"},
        {"log2", "E"},
        {"sin", "F"},
        {"cos", "G"},
        {"tan", "H"}, {"tang", "H"},
        {"cot", "I"}, {"cotang", "I"},
        {"abs", "J"},
    };
    
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
        if (irregularOperators.Contains(c)) {return 4;}
        return 0;
    }

    public string RPN(string inp)
    {
        //checker
        foreach(char c in inp)
        {
            if (!(allowed.Contains(c))) {Debug.LogError("Wrong format!"); return "---------";}
        }

        //replace
        foreach(KeyValuePair<string, string> entry in replacementFunctions)
        {
            inp = inp.Replace(entry.Key, entry.Value);
        }

        //RPN
        string output = "";
        int idx = 0;
        Stack<char> st = new Stack<char>(); st.Push('#'); //antioverflow

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

            //check for ^,irregulars
            if (inp[idx] == '^' || irregularOperators.Contains(inp[idx])) { st.Push(inp[idx]); idx++; found = true;}
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

        //irregulars
        if (exp == "A") {return Mathf.Sqrt(b);}
        if (exp == "B") {return Mathf.Pow(b, 1/3f);}
        if (exp == "C") {return Mathf.Log(b);}
        if (exp == "D") {return Mathf.Log10(b);}
        if (exp == "E") {return Mathf.Log(b, 2);}
        if (exp == "F") {return Mathf.Sin(b);}
        if (exp == "G") {return Mathf.Cos(b);}
        if (exp == "H") {return Mathf.Tan(b);}
        if (exp == "I") {return 1/Mathf.Tan(b);}
        if (exp == "J") {return Mathf.Abs(b);}
        return -1f;
    }

    public float CalculateRaw(string rpn, Dictionary<char, float> sub = null)
    {
        string[] sp = rpn.Split(' ');
        List<string> raw = new List<string>(sp);
        while (raw.Count > 0 && (raw[raw.Count-1] == " " || raw[raw.Count-1] == "" || raw[raw.Count-1] == "#")) {raw.RemoveAt(raw.Count-1);}

        Debug.Log(rpn);

        //process
        Stack<string> st = new Stack<string>();
        foreach(string curStr in raw)
        {
            //is a num/var
            if (alphanum.Contains(curStr[0])) 
            { 
                string newCurStr = curStr;
                if (alpha.Contains(curStr[0])) { newCurStr = (sub[curStr[0]]).ToString(); }
                st.Push(newCurStr);                
            }

            //is one of +-*/^
            else if (regularOperators.Contains(curStr[0]))
            {
                string x1 = st.Pop(); string x2 = st.Pop();

                float first = float.Parse(x1, CultureInfo.InvariantCulture.NumberFormat);
                float sec = float.Parse(x2, CultureInfo.InvariantCulture.NumberFormat);

                st.Push(GetFinalvalue(sec, first, curStr).ToString());
            }
            
            //is a 1-argument math function
            else
            {
                string x1 = st.Pop(); 
                float first = float.Parse(x1, CultureInfo.InvariantCulture.NumberFormat);

                st.Push(GetFinalvalue(0, first, curStr).ToString());
            }
        }

        //return
        return float.Parse(st.Peek(), CultureInfo.InvariantCulture.NumberFormat);
    }
}
