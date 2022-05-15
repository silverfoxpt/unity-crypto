using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotsController : MonoBehaviour
{
    [SerializeField] private GameObject holderPref;
    [SerializeField] private float offset = 2.5f;

    private BumpsController bumpsController;
    private List<HolderController> holders;

    private void Awake() 
    {
        bumpsController = FindObjectOfType<BumpsController>();    
    }

    private void Start()
    {
        holders = new List<HolderController>();
        float startX = -bumpsController.space * (bumpsController.startLevel/2) 
                        - (bumpsController.level - 1) * bumpsController.space /2
                        - bumpsController.space / 2f;

        float startY = -bumpsController.space * (bumpsController.level - 1) - offset;
        Vector2 startPos = new Vector2(startX, startY);

        //create holders
        int numEnd = bumpsController.startLevel + bumpsController.level - 1 ;
        for (int i = 0; i < numEnd + 1; i++)
        {
            var hold = Instantiate(holderPref, startPos, Quaternion.identity, transform);
            startPos += new Vector2(bumpsController.space, 0f);

            holders.Add(hold.GetComponent<HolderController>());
        }
    }

    private void Update()
    {
        int max = -1;
        foreach(var hold in holders)
        {
            max = Mathf.Max(hold.GetCount(), max);
        }

        foreach(var hold in holders)
        {
            if (max == 0) {hold.SetPic(1f); continue;}
            hold.SetPic(hold.GetCount() / (float) max);
        }
    }
}
