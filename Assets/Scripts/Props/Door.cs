using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private Collider2D col;
    private Animator animator;
    public bool open;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
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

    public void Open()
    {
        open = true;
    }

    public void Close()
    {
        open = false;
    }

    public void Toggle()
    {
        open = !open;
    }

    public void DisableCollider()
    {
        col.enabled = false;
    }

    public void EnableCollider()
    {
        col.enabled = true;
    }

    private void UpdateAnimator()
    {
        animator.SetBool("IsOpen", open);
    }
}
