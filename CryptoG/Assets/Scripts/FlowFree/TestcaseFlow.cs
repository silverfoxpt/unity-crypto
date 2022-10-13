using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flowfree Testcase", fileName = "Flowtest (0)")]
public class TestcaseFlow : ScriptableObject
{
    [SerializeField] List<flowCellPair> initialCellList = new List<flowCellPair>();

    public List<flowCellPair> GetCellList() {return initialCellList; }
}
