using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HolderController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counter;
    [SerializeField] private Image fillCol;
    private int count = 0;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Ball")
        {
            Destroy(other.gameObject);
            count++;

            counter.text = count.ToString();
        }
    }

    public int GetCount()
    {
        return count;
    }

    public void SetPic(float percent)
    {
        fillCol.fillAmount = percent;
    }
}
