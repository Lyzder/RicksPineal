using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IrisController : MonoBehaviour
{

    private Animator animator;
    private Image image;
    private readonly int circleSizeId = Shader.PropertyToID("_CircleSize");

    public float circleSize = 0;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        image.materialForRendering.SetFloat(circleSizeId, circleSize);
    }

    public void TransitionIn()
    {
        animator.ResetTrigger("In");
        animator.SetTrigger("In");
    }

    public void TransitionOut()
    {
        animator.ResetTrigger("Out");
        animator.SetTrigger("Out");
    }
}
