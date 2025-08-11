using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IrisController : MonoBehaviour
{

    private Animator animator;
    private Image image;
    private readonly int circleSizeId = Shader.PropertyToID("_CircleSize");
    private readonly int offsetId = Shader.PropertyToID("_Offset");
    private Transform tracker;
    private Camera mainCamera;

    public float circleSize = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        image = GetComponent<Image>();
        image.materialForRendering.SetVector(offsetId, Vector2.zero);
        tracker = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        image.materialForRendering.SetFloat(circleSizeId, circleSize);
        if (tracker != null)
            image.materialForRendering.SetVector(offsetId, TrackTransform());
    }

    public void MakeVisible(bool visible)
    {
        image.enabled = visible;
    }

    /// <summary>
    /// Plays the iris in transition at the center of the screen.
    /// </summary>
    public void TransitionIn()
    {
        image.materialForRendering.SetVector(offsetId, Vector2.zero);
        animator.ResetTrigger("In");
        animator.SetTrigger("In");
    }

    /// <summary>
    /// Plays the iris in transition at the specified screen coordinates.
    /// </summary>
    /// <param name="coordinates"></param>
    public void TransitionIn(Vector2 coordinates)
    {
        image.materialForRendering.SetVector(offsetId, CoordToOffset(coordinates));
        animator.ResetTrigger("In");
        animator.SetTrigger("In");
    }

    /// <summary>
    /// Plays the iris in transition at the position of the transform argument.
    /// </summary>
    /// <param name="tracker"></param>
    public void TransitionIn(Transform tracker)
    {
        this.tracker = tracker;
        animator.ResetTrigger("In");
        animator.SetTrigger("In");
    }

    /// <summary>
    /// Plays the iris out transition at the center of the screen.
    /// </summary>
    public void TransitionOut()
    {
        image.materialForRendering.SetVector(offsetId, Vector2.zero);
        animator.ResetTrigger("Out");
        animator.SetTrigger("Out");
    }

    /// <summary>
    /// Plays the iris out transition at the specified screen coordinates.
    /// </summary>
    /// <param name="coordinates"></param>
    public void TransitionOut(Vector2 coordinates)
    {
        image.materialForRendering.SetVector(offsetId, CoordToOffset(coordinates));
        animator.ResetTrigger("Out");
        animator.SetTrigger("Out");
    }

    /// <summary>
    /// Plays the iris out transition at the position of the transform argument.
    /// </summary>
    /// <param name="tracker"></param>
    public void TransitionOut(Transform tracker)
    {
        this.tracker = tracker;
        animator.ResetTrigger("Out");
        animator.SetTrigger("Out");
    }

    private Vector2 CoordToOffset(Vector2 coordinates)
    {
        Vector2 offset = new();

        offset.x = Mathf.Clamp((coordinates.x / Screen.width) - 0.5f, -0.5f, 0.5f);
        offset.y = Mathf.Clamp((coordinates.y / Screen.height) - 0.5f, -0.5f, 0.5f);

        return offset;
    }

    public void ForceHoldIn()
    {
        animator.Play("IrisHoldIn");
    }

    public void ForceHoldOut()
    {
        animator.Play("IrisHoldOut");
    }

    public void ResetTracker()
    {
        tracker = null;
    }

    public void ResetOffset()
    {
        image.materialForRendering.SetVector(offsetId, Vector2.zero);
    }

    private Vector2 TrackTransform()
    {
        Vector2 coord;

        coord = mainCamera.WorldToScreenPoint(tracker.position);

        return CoordToOffset(coord);
    }
}
