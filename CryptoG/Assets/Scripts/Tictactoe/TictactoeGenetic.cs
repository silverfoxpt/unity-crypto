using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public class TictactoeGenetic : MonoBehaviour
{
    [SerializeField] private int size = 3;
    [SerializeField] private int winSize;

    [Header("Other")]
    [SerializeField] private string configSaveFile = "ticConfig.dat";
    
    private List<List<List<int>>> ticConfigs = new List<List<List<int>>>();
    private Dictionary<List<List<int>>, List<List<List<int>>> > prev = new Dictionary<List<List<int>>, List<List<List<int>>>>();
    private HashSet<List<List<int>>> unique = new HashSet<List<List<int>>>();

    private string dataPath;
    void Start()
    {
        GetConfigs();
        StripUnfairConfigs();
        FindUniqueConfigs();
    }

    private void FindUniqueConfigs()
    {
        throw new NotImplementedException();
    }

    private void StripUnfairConfigs()
    {
        List<List<List<int>>> allRightConf = new List<List<List<int>>>();
        foreach(var conf in ticConfigs)
        {
            if (CheckConfig(conf)) {allRightConf.Add(conf);}
        }
        ticConfigs = new List<List<List<int>>>(allRightConf);
    }

    private bool CheckConfig(List<List<int>> con)
    {
        int xWin = 0, yWin = 0;

        //horizontal
        if (con[0][0] == con[0][1] && con[0][1] == con[0][2]) {xWin += (con[0][0] == 1) ? 1 : 0; yWin += (con[0][0] == 2) ? 1 : 0;}
        if (con[1][0] == con[1][1] && con[1][1] == con[1][2]) {xWin += (con[1][0] == 1) ? 1 : 0; yWin += (con[1][0] == 2) ? 1 : 0;}
        if (con[2][0] == con[2][1] && con[2][1] == con[2][2]) {xWin += (con[2][0] == 1) ? 1 : 0; yWin += (con[2][0] == 2) ? 1 : 0;}

        //vertical
        if (con[0][0] == con[1][0] && con[1][0] == con[2][2]) {xWin += (con[0][0] == 1) ? 1 : 0; yWin += (con[0][0] == 2) ? 1 : 0;}
        if (con[0][1] == con[1][1] && con[1][1] == con[2][1]) {xWin += (con[0][1] == 1) ? 1 : 0; yWin += (con[0][1] == 2) ? 1 : 0;}
        if (con[0][2] == con[1][2] && con[1][2] == con[2][2]) {xWin += (con[0][2] == 1) ? 1 : 0; yWin += (con[0][2] == 2) ? 1 : 0;}

        //diagonal
        if (con[0][0] == con[1][1] && con[1][1] == con[2][2]) {xWin += (con[0][0] == 1) ? 1 : 0; yWin += (con[0][0] == 2) ? 1 : 0;}
        if (con[0][2] == con[1][1] && con[1][1] == con[2][0]) {xWin += (con[0][2] == 1) ? 1 : 0; yWin += (con[0][2] == 2) ? 1 : 0;}

        //check win state
        bool win = false;
        if ((xWin == 1 && yWin == 0) || (xWin == 0 && yWin == 1)) {win = true;}
        else if (xWin == 0 && yWin == 0) {win = false;}
        else {return false;}

        //count x y
        int xCount = 0, yCount = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (con[i][j] == 1) {xCount++;} else if (con[i][j] == 2) {yCount++;}
            }
        }

        if ((xCount == yCount || xCount == yCount+1))
        {
            if (win)
            {
                if (xWin == 1 && xCount == yCount+1) {return true;}
                else if (yWin == 1 && xCount == yCount) {return true;}
                return false;
            }
            return false;
        } 
        return false;
    }

    #region createConfigs
    private void GetConfigs()
    {
        dataPath = Application.dataPath + "/Prefabs/Tictactoe" + "/" + configSaveFile;

        if (!File.Exists(dataPath)) { GetAllConfigs(); SaveAllConfigsToFile(); }
        else { OpenConfigsFromFile(); }
    }

    private void OpenConfigsFromFile()
    {
        FileStream file;
        file = File.OpenRead(dataPath);

        BinaryFormatter bf = new BinaryFormatter();
        ticConfigs = (List<List<List<int>>>) bf.Deserialize(file);
        file.Close();

        Debug.Log(ticConfigs.Count);
    }

    private void SaveAllConfigsToFile()
    {
        FileStream file;
        file = File.Create(dataPath);

        BinaryFormatter form = new BinaryFormatter();
        form.Serialize(file, ticConfigs);

        file.Close();
    }

    private void GetAllConfigs()
    {
        ticConfigs = new List<List<List<int>>>();
        int avail = 1;
        for (int i = 0; i < size*size; i++)
        {
            avail *= 3;
        }        

        for (int i = 0; i < avail; i++)
        {
            int cur = i;
            List<int> conf = new List<int>();

            while(cur > 0)
            {
                conf.Add(cur%3); cur /= 3;
            }
            while(conf.Count < 9) {conf.Add(0);}

            List<List<int>> conf2d = new List<List<int>>();

            for (int j = 0; j < 9; j += 3)
            {
                List<int> nL = new List<int>();
                nL.Add(conf[j]);
                nL.Add(conf[j+1]);
                nL.Add(conf[j+2]);
                conf2d.Add(nL);
            }
            ticConfigs.Add(conf2d);
        }
    }
    #endregion
}
