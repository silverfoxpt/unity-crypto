using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST2 : MonoBehaviour
{
    struct test1
    {
        public TEST3 x;
        public test1(TEST3 a) {this.x = a;}
    }

    private void Start()
    {
        TEST3 a = new TEST3();
        a.x = 1; a.y = 2;

        test1 b = new test1(a);
        b.x.x = 99;

        Debug.Log(a.x);
    }
}
