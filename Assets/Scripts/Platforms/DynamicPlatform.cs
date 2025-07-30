using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DynamicPlatform : MonoBehaviour
{
    public Vector2 PlatformVelocity { get; protected set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected virtual void FixedUpdate()
    {
        UpdateSpeed();
    }

    protected abstract void UpdateSpeed();
}
