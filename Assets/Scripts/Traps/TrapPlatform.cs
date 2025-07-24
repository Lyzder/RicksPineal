using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapPlatform : MonoBehaviour
{
    [Header("Timing")]
    public float intervalTime;
    public float startTime;
    public bool isActive;
    private float timer;
    [Header("Hitbox")]
    [SerializeField] private GameObject hitboxObject;

    // Components
    Collider2D col;
    Animator animator;

    private void Awake()
    {
        col = hitboxObject.GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        timer = startTime;
    }

    // Update is called once per frame
    void Update()
    {
        RunTimer();
        UpdateState();
    }

    private void RunTimer()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            isActive = !isActive;
            timer = intervalTime;
        }
    }

    private void UpdateState()
    {
        col.enabled = isActive;
        animator.SetBool("IsActive", isActive);
    }
}
