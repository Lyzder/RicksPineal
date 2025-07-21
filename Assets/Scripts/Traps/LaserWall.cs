using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


public class LaserWall : MonoBehaviour
{
    [Header("Segments")]
    public int segmentCount;
    [SerializeField] GameObject segmentPrefab;
    [SerializeField] GameObject segmentStartPrefab;
    [SerializeField] GameObject segmentEndPrefab;
    [SerializeField] Vector2 segmentSize;
    [SerializeField] Vector2 segmentStartSize;
    [SerializeField] Vector2 segmentEndSize;
    [SerializeField] float2 startCoordinates;
    private List<GameObject> segments;
    public enum Orientation
    {
        Left,
        Right,
        Top,
        Bottom
    }

    public Orientation orientation;
    [Header("Timing")]
    public float intervalTime;
    public float startTime;
    public bool isActive;
    private float timer;
    [Header("Debug")]
    public bool showSize;
    public Color lineColor;
    // Components
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        segments = new List<GameObject>();
        PopulateSegments();
    }

    // Update is called once per frame
    void Update()
    {
        if (segmentCount > 0)
            RunTimer();
    }

    private void PopulateSegments()
    {
        if (segmentCount < 1)
        {
            boxCollider.enabled = false;
            return;
        }
        Vector3 currentPosition = transform.TransformPoint(new Vector3(startCoordinates.x, startCoordinates.y, 0));
        GameObject segment;
        Vector2 colliderSize = segmentStartSize;
        int i;

        for (i = 0; i < segmentCount; i++)
        {
            if (i == 0)
            {
                segment = Instantiate(segmentStartPrefab, currentPosition, Quaternion.identity, gameObject.transform);
            }
            else if (i == segmentCount - 1)
            {
                segment = Instantiate(segmentEndPrefab, currentPosition, Quaternion.identity, gameObject.transform);
            }
            else
            {
                segment = Instantiate(segmentPrefab, currentPosition, Quaternion.identity, gameObject.transform);
            }
            segments.Add(segment);
            switch (orientation)
            {
                case Orientation.Left:
                    currentPosition.x -= segmentSize.x;
                    if (i == 0)
                        break;
                    else if (i == segmentCount - 1)
                        colliderSize.x += segmentEndSize.x;
                    else
                        colliderSize.x += segmentSize.x;
                    break;
                case Orientation.Right:
                    currentPosition.x += segmentSize.x;
                    if (i == 0)
                        break;
                    else if (i == segmentCount - 1)
                        colliderSize.x += segmentEndSize.x;
                    else
                        colliderSize.x += segmentSize.x;
                    break;
                case Orientation.Top:
                    currentPosition.y += segmentSize.y;
                    if (i == 0)
                        break;
                    else if (i == segmentCount - 1)
                        colliderSize.y += segmentEndSize.y;
                    else
                        colliderSize.y += segmentSize.y;
                    break;
                case Orientation.Bottom:
                    currentPosition.y -= segmentSize.y;
                    if (i == 0)
                        break;
                    else if (i == segmentCount - 1)
                        colliderSize.y += segmentEndSize.y;
                    else
                        colliderSize.y += segmentSize.y;
                    break;
            }
        }
        switch (orientation)
        {
            case Orientation.Left:
                boxCollider.size = new Vector2(colliderSize.x, 0.5f);
                boxCollider.offset = new Vector2(boxCollider.offset.x - (colliderSize.x / 2) + (segmentStartSize.x / 2), boxCollider.offset.y);
                break;
            case Orientation.Right:
                boxCollider.size = new Vector2(colliderSize.x, 0.5f);
                boxCollider.offset = new Vector2(boxCollider.offset.x + (colliderSize.x / 2) - (segmentStartSize.x / 2), boxCollider.offset.y);
                break;
            case Orientation.Top:
                boxCollider.size = new Vector2(0.5f, colliderSize.y);
                boxCollider.offset = new Vector2(boxCollider.offset.x, boxCollider.offset.y + (colliderSize.y / 2) - (segmentStartSize.y / 2));
                break;
            case Orientation.Bottom:
                boxCollider.size = new Vector2(0.5f, colliderSize.y);
                boxCollider.offset = new Vector2(boxCollider.offset.x, boxCollider.offset.y - (colliderSize.y / 2) + (segmentStartSize. y / 2));
                break;
        }
    }

    private void RunTimer()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            isActive = !isActive;
            ActivateSegments(isActive);
        }
    }

    private void ActivateSegments(bool active)
    {
        Collider2D segmentCollider;
        SpriteRenderer spriteRenderer;
        Animator animator;

        boxCollider.enabled = active;
        timer = intervalTime;

        foreach (GameObject segment in segments)
        {
            segmentCollider = segment.GetComponent<Collider2D>();
            spriteRenderer = segment.GetComponent<SpriteRenderer>();
            animator = spriteRenderer.GetComponent<Animator>();

            segmentCollider.enabled = active;
            spriteRenderer.enabled = active;
            animator.SetBool("IsActive", active);
        }
    }

    private void OnDrawGizmos()
    {
        if (!showSize || segmentCount < 1) return;

        int i;

        Gizmos.color = lineColor;
        Vector3 currentPosition = transform.TransformPoint(new Vector3(startCoordinates.x, startCoordinates.y, 0));
        Vector3 center, size;

        for (i = 0; i < segmentCount; i++)
        {
            center = new(currentPosition.x, currentPosition.y);
            switch (orientation)
            {
                case Orientation.Left:
                    if (i == 0)
                    {
                        size = new(segmentStartSize.y, segmentStartSize.x, 0);
                        currentPosition.x -= segmentStartSize.x;
                    }
                    else if (i == segmentCount - 1)
                    {
                        size = new(segmentEndSize.y, segmentEndSize.x, 0);
                        currentPosition.x -= segmentEndSize.x;
                    }
                    else
                    {
                        size = new(segmentSize.y, segmentSize.x, 0);
                        currentPosition.x -= segmentSize.x;
                    }
                    Gizmos.DrawWireCube(center, size);
                    break;
                case Orientation.Right:
                    if (i == 0)
                    {
                        size = new(segmentStartSize.y, segmentStartSize.x, 0);
                        currentPosition.x += segmentStartSize.x;
                    }
                    else if (i == segmentCount - 1)
                    {
                        size = new(segmentEndSize.y, segmentEndSize.x, 0);
                        currentPosition.x += segmentEndSize.x;
                    }
                    else
                    {
                        size = new(segmentSize.y, segmentSize.x, 0);
                        currentPosition.x += segmentSize.x;
                    }
                    Gizmos.DrawWireCube(center, size);
                    break;
                case Orientation.Top:
                    if (i == 0)
                    {
                        size = new(segmentStartSize.x, segmentStartSize.y, 0);
                        currentPosition.y += segmentStartSize.y;
                    }
                    else if (i == segmentCount - 1)
                    {
                        size = new(segmentEndSize.x, segmentEndSize.y, 0);
                        currentPosition.y += segmentEndSize.y;
                    }
                    else
                    {
                        size = new(segmentSize.x, segmentSize.y, 0);
                        currentPosition.y += segmentSize.y;
                    }
                    Gizmos.DrawWireCube(center, size);
                    break;
                case Orientation.Bottom:
                    if (i == 0)
                    {
                        size = new(segmentStartSize.x, segmentStartSize.y, 0);
                        currentPosition.y -= segmentStartSize.y;
                    }
                    else if (i == segmentCount - 1)
                    {
                        size = new(segmentEndSize.x, segmentEndSize.y, 0);
                        currentPosition.y -= segmentEndSize.y;
                    }
                    else
                    {
                        size = new(segmentSize.x, segmentSize.y, 0);
                        currentPosition.y -= segmentSize.y;
                    }
                    Gizmos.DrawWireCube(center, size);
                    break;
            }
        }
    }
}
