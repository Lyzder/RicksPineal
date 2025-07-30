using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treadmill : DynamicPlatform
{
    public enum Direction
    {
        Left,
        Right,
    }
    public Direction direction;
    public float speed;
    public bool active;

    // Components

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAnimator();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void UpdateSpeed()
    {
        if (active)
            PlatformVelocity = new Vector2(direction == Direction.Left ? speed * -1 : speed, 0);
        else
            PlatformVelocity = Vector2.zero;
    }

    private void UpdateAnimator()
    {
        animator.SetInteger("Direction", direction == Direction.Left ? -1 : 1);
        animator.SetBool("IsActive", active);
    }
}
