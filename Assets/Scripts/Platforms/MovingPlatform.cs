using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : DynamicPlatform
{
    [Header("Movement")]
    public float moveSpeed;
    public float waitTime;
    public Vector2[] waypoints;
    private float timer;
    private short currentPoint;
    private bool flow;
    private Vector2 lastPosition, targetPosition;

    // Components
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        timer = 0;
        currentPoint = 0;
        flow = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void FixedUpdate()
    {
        if (timer > 0)
        {
            Wait();
        }
        else
        {
            MovePlatform();
        }
        base.FixedUpdate();
    }

    private void MovePlatform()
    {
        Vector2 target = waypoints[currentPoint];
        targetPosition = Vector2.MoveTowards(rb.position, target, moveSpeed * Time.fixedDeltaTime);
        lastPosition = rb.position;
        rb.MovePosition(targetPosition);

        if (targetPosition == target)
        {
            timer = waitTime;
        }
    }

    private void Wait()
    {
        timer -= Time.deltaTime;
        lastPosition = rb.position;
        if (timer <= 0)
            SetDestination();
    }

    private void SetDestination()
    {
        if (waypoints.Length <= 1)
            return;

        if (currentPoint == 0)
        {
            flow = true;
        }
        else if (currentPoint == waypoints.Length - 1)
        {
            flow = false;
        }

        if (flow)
        {
            currentPoint += 1;
        }
        else
        {
            currentPoint -= 1;
        }
    }

    protected override void UpdateSpeed()
    {
        PlatformVelocity = (targetPosition - lastPosition) / Time.fixedDeltaTime;
    }
}
