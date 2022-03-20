using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingDotSceneOne : MonoBehaviour
{
    [SerializeField] private GameObject dotPref;
    [SerializeField] private GameObject bounder;
    private GameObject dot = null;

    private void Start()
    {
        dot = Instantiate(dotPref, bounder.GetComponent<RectangleDrawer>().GetCenterRect(), Quaternion.identity, transform);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        dot.transform.position = (Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
}
