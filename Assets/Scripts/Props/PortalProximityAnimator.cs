using UnityEngine;

public class PortalProximityAnimator : MonoBehaviour
{
    [SerializeField] private Animator portalAnimator;
    [SerializeField] private string activeBoolParam = "IsActive";
    [SerializeField] private string playerTag = "Player";

    private void Awake()
    {
        if (portalAnimator == null)
            portalAnimator = GetComponent<Animator>();

        if (portalAnimator == null)
            portalAnimator = GetComponentInChildren<Animator>();

        if (portalAnimator != null)
            portalAnimator.SetBool(activeBoolParam, false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (portalAnimator != null) portalAnimator.SetBool(activeBoolParam, true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (portalAnimator != null) portalAnimator.SetBool(activeBoolParam, false);
    }
}
