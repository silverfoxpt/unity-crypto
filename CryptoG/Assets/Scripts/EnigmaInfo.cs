using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnigmaInfo : MonoBehaviour
{
    public static int defaultLength = 26;
    public static int numWheel = 8;
    public static int maxPlugboard = 10;
    public static string defaultAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public static string reflectorB = "YRUHQSLDPXNGOKMIEBFZCWVJAT";
    public static string reflectorC = "FVPJIAOYEDRZXWGCTKUQSBNMHL";
    public static List<string> wheels = new List<string>{
        "EKMFLGDQVZNTOWYHXUSPAIBRCJ",
        "AJDKSIRUXBLHWTMCQGZNPYFVOE",
        "BDFHJLCPRTXVZNYEIWGAKMUSQO",
        "ESOVPZJAYQUIRHXLNFTGKDCMWB",
        "VZBRGITYUPSDNHLXAWMJQOFECK",
        "JPGVOUMFYQBENHZRDKASXLICTW",
        "NZJHGRCXMYSWBOUFAIVLPEKQDT",
        "FKQHTLXOCBJSPDZRAMEWNIUYGV"
    };


    public static List<List<char> > turnovers = new List<List<char>>{
        new List<char>{'Q'},
        new List<char>{'E'},
        new List<char>{'V'},
        new List<char>{'J'},
        new List<char>{'Z'},
        new List<char>{'Z', 'M'},
        new List<char>{'Z', 'M'},
        new List<char>{'Z', 'M'},
    };

    public static Dictionary<string, int> convertText = new Dictionary<string, int>() {
        {"I", 0},
        {"II", 1},
        {"III", 2},
        {"IV", 3},
        {"V", 4},
        {"VI", 5},
        {"VII", 6},
        {"VIII", 7},
    };

    public static Dictionary<int, string> convertTextReverse = new Dictionary<int, string>() {
        {0, "I"},
        {1, "II"},
        {2, "III"},
        {3, "IV"},
        {4, "V"},
        {5, "VI"},
        {6, "VII"},
        {7, "VIII"},
    };
}
