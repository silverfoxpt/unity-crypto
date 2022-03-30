using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MazeUIController : MonoBehaviour
{
    [SerializeField] TMP_Dropdown dropdown;
    [SerializeField] MazeCreator mazeCreator;

    private string status = "idle";

    public void StartMaze()
    {
        if (status != "idle") {return;}

        mazeCreator.RefreshMaze();
        int val = dropdown.value;

        switch(val)
        {
            case 0: FindObjectOfType<MazeBacktrackAlgorithm>().CreateNewMaze(); break;
            case 1: FindObjectOfType<MazeKruskalAlgorithm>().CreateNewMaze(); break;
            case 2: FindObjectOfType<MazePrimAlgorithm>().CreateNewMaze(); break;
            case 3: FindObjectOfType<MazeAldousBroderAlgorithm>().CreateNewMaze(); break;
            case 4: FindObjectOfType<MazeHuntKillAlgorithm>().CreateNewMaze(); break;
        }

        status = "running";
    }

    public void StopMaze()
    {
        if (status == "running")
        {
            FindObjectOfType<MazeBacktrackAlgorithm>().StopAllCoroutines();
            FindObjectOfType<MazeKruskalAlgorithm>().StopAllCoroutines();
            FindObjectOfType<MazePrimAlgorithm>().StopAllCoroutines();
            FindObjectOfType<MazeAldousBroderAlgorithm>().StopAllCoroutines();
            FindObjectOfType<MazeHuntKillAlgorithm>().StopAllCoroutines();

            status = "idle";
        }
    }
}
