using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallsGen : MonoBehaviour
{
    [SerializeField] private GameObject ballPref;
    [SerializeField] private int numBalls = 100;
    [SerializeField] private float delay = 0.05f;
    [SerializeField] private float heightDrop = 3f;
    [SerializeField] private float ballSize = 0.05f;
    [SerializeField] private GameObject stopper;

    private void Start()
    {
        StartCoroutine(GenBalls());
    }

    IEnumerator GenBalls()
    {
        stopper.SetActive(false);
        for (int i = 0; i < numBalls; i++)
        {
            CreateNewBall(); 
            yield return new WaitForSeconds(delay);
        }

        yield return new WaitForSeconds(2f);
        //falls!
    }

    private void CreateNewBall()
    {
        var ball = Instantiate(ballPref, new Vector3(0f, heightDrop, 0f), Quaternion.identity, transform);
        ball.transform.localScale = new Vector3(ballSize, ballSize, 1f);

        ball.GetComponent<Rigidbody2D>().velocity += new Vector2(UnityEngine.Random.Range(-0.1f, 0.1f), UnityEngine.Random.Range(-0.1f, 0.1f));
        ball.GetComponent<SpriteRenderer>().color = UtilityFunc.GetRandColor();
    }
}
