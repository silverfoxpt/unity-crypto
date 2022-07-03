using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateFractalTree : MonoBehaviour
{
    [System.Serializable]
    struct lRule
    {
        public string root;
        public string apply;
    
        public lRule(string r, string a) {root = r; apply = a;}
    }

    class turtle
    {
        public Vector2 dir, pos;

        public turtle(Vector2 di, Vector2 p) {dir = di; pos = p;}
    }

    [Header("References")]
    [SerializeField] private GameObject linePref;

    [Header("Line options")]
    [SerializeField] private float lineWidth = 0.025f;
    [SerializeField] private Color lineColor = Color.white;
    [SerializeField] private float branchLength = 0.5f;
    [SerializeField] private Vector2 offset = new Vector2(0f, -5f); // start at ground

    [Header("Tree options")]
    [SerializeField] private string axiom = "F";
    [SerializeField] private List<lRule> rules;
    [SerializeField] private int depth = 5;
    [SerializeField] private float turnAngle = 30f;

    private List<turtle> turtles;
    private string curFracString;

    void Start()
    {
        GenerateTree();
    }

    private Vector2 RotateVector(Vector2 inp, int sign)
    {
        float angle = turnAngle * Mathf.Deg2Rad * sign;

        return new Vector2(Mathf.Cos(angle) * inp.x - Mathf.Sin(angle) * inp.y,
                           Mathf.Sin(angle) * inp.x + Mathf.Cos(angle) * inp.y);
    }

    private void GenerateTree()
    {
        turtles = new List<turtle>();

        turtles.Add(new turtle(Vector2.up, offset));
        curFracString = axiom;

        for (int i = 0; i < depth; i++)
        {
            //process current string
            for (int j = 0; j < curFracString.Length; j++)
            {
                var cur = turtles[turtles.Count - 1]; //get current turtle

                char c = curFracString[j];
                if (c == 'F')
                {
                    Vector2 newPos = cur.pos + cur.dir * branchLength;
                    CreateNewLine(cur.pos, newPos);
                    cur.pos = newPos;
                }
                else if (c == '+')
                {
                    cur.dir = RotateVector(cur.dir, -1);
                }
                else if (c == '-' || c == '−')
                {
                    cur.dir = RotateVector(cur.dir, 1);
                }
                else if (c == '[')
                {
                    turtle newTurtle = new turtle(cur.dir, cur.pos);
                    turtles.Add(newTurtle);
                }
                else if (c == ']')
                {
                    turtles.RemoveAt(turtles.Count - 1);                    
                }
            }

            //update
            foreach (var rule in rules)
            {
                curFracString = curFracString.Replace(rule.root, rule.apply);
            }
        }
    }

    private void CreateNewLine(Vector2 start, Vector2 end)
    {
        var newLine = Instantiate(linePref, Vector3.zero, Quaternion.identity, transform);
        var rend = newLine.GetComponent<LineRenderer>();

        rend.positionCount = 2;
        rend.SetPosition(0, start);
        rend.SetPosition(1, end);

        rend.startColor = rend.endColor = lineColor;
        rend.startWidth = rend.endWidth = lineWidth;
    }
}
