using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct weirdPair
{
    public char encodeChar; 
    public string decodeString;
}

[System.Serializable]
public struct multiPair
{
    public string encodeString, decodeString;
}

[CreateAssetMenu(menuName = "Spelling Alphabet", fileName = "Alphabet")]
public class SpellingAlphabetScriptableObject : ScriptableObject
{
    [SerializeField] private List<string> mainAlphabet;
    [SerializeField] private List<weirdPair> weirdAlphabet;
    [SerializeField] private List<multiPair> multiAlphabet;

    public List<string> GetMainAlphabet() {return new List<string>(mainAlphabet);}
    public List<weirdPair> GetWeirdAlphabet() {return new List<weirdPair>(weirdAlphabet);}
    public List<multiPair> GetMultiAlphabet() {return new List<multiPair>(multiAlphabet);}
}
