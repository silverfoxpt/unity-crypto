using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlugboardDisplayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI curText;
    [SerializeField] private GameObject plugboard; 
    
    public void UpdatePlugboardDisplay(string newSetup)
    {
        Dictionary<char, char> tmp = new Dictionary<char, char>();
        for (char c = 'A'; c <= 'Z'; c++)
        {
            tmp.Add(c, c);
        }
        
        for (int i = 0; i < EnigmaInfo.defaultLength; i++)
        {
            tmp[(char) ('A' + i)] = newSetup[i];    
        }
        
        string newText = "";
        HashSet<char> tmp2 = new HashSet<char>();
        int cnt = 0;
        for (char c = 'A'; c <= 'Z'; c++)
        {
            if (tmp[c] != c && !tmp2.Contains(c))
            {
                cnt++;
                newText += c; newText += tmp[c]; newText += ' ';
                tmp2.Add(c); tmp2.Add(tmp[c]);
                if (cnt == EnigmaInfo.maxPlugboard/2) { newText += '\n';}
            }
        }
        while (cnt <= EnigmaInfo.maxPlugboard)
        {
            cnt++;
            newText += "__ ";
            if (cnt == EnigmaInfo.maxPlugboard/2) {newText += '\n';}
        }
        curText.text = newText;
    }
}
