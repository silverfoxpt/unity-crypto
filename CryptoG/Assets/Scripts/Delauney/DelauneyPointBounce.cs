using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelauneyPointBounce : MonoBehaviour
{
    public float accelerationTime = 2f;
    public float maxSpeed = 5f;
    public float maxVelo = 1f;

    private Vector2 movement;
    private float timeLeft;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        timeLeft -= Time.deltaTime;
        if(timeLeft <= 0)
        {
            movement = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            timeLeft = accelerationTime;

            rb.AddForce(movement * maxSpeed * Time.deltaTime);
            rb.velocity = rb.velocity.normalized * maxVelo;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag != "BariDelauney") { return; }
        rb.AddForce((new Vector2(0, 0) - (Vector2) transform.position).normalized * 2 * maxSpeed * Time.deltaTime);
        rb.velocity = rb.velocity.normalized * maxVelo;
    }
}
