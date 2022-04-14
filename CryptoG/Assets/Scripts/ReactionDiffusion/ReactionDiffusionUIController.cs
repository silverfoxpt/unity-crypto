using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactionDiffusionUIController : MonoBehaviour
{
    [SerializeField] private GameObject box;
    [SerializeField] private ScreenController screenController;
    [SerializeField] private ReactionDiffusion reactionDiffusion;

    public void BoxScaleChange(float newScale)
    {
        box.transform.localScale = new Vector3(newScale, newScale, 0f);
    }

    public void StartReaction()
    {
        screenController.RefreshScreen();

        reactionDiffusion.StopAllCoroutines();
        reactionDiffusion.InitializeConditions();        
        reactionDiffusion.shouldRun = true;
    }

    public void StopReaction()
    {
        reactionDiffusion.shouldRun = false;
    }
}
