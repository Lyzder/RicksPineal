using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float waitTime;
    public Vector2[] waypoints;
    private float timer;
    private short currentPoint;
    private bool direction;
    public Vector2 PlatformVelocity { get; private set; }

    // Components
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        timer = 0;
        currentPoint = 0;
        direction = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (timer > 0)
        {
            Wait();
        }
        else
        {
            MovePlatform();
        }
    }

    private void MovePlatform()
    {
        Vector2 target = waypoints[currentPoint];
        Vector2 newPosition = Vector2.MoveTowards(rb.position, target, moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);

        if (newPosition == target)
        {
            timer = waitTime;
        }
    }

    private void Wait()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
            SetDestination();
    }

    private void SetDestination()
    {
        if (waypoints.Length <= 1)
            return;

        if (currentPoint == 0)
        {
            direction = true;
        }
        else if (currentPoint == waypoints.Length - 1)
        {
            direction = false;
        }

        if (direction)
        {
            currentPoint += 1;
        }
        else
        {
            currentPoint -= 1;
        }
    }
}
