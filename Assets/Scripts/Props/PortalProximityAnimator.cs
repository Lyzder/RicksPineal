using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PortalProximityAnimator : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private Animator portalAnimator;
    [SerializeField] private string activeBoolParam = "IsActive";
    [SerializeField] private string playerTag = "Player";

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float targetVolume = 1f;
    [SerializeField] private float fadeInTime = 0.25f;
    [SerializeField] private float fadeOutTime = 0.35f;

    private Coroutine fadeRoutine;

    private void Awake()
    {
        // Animator fallback
        if (portalAnimator == null) portalAnimator = GetComponent<Animator>();
        if (portalAnimator == null) portalAnimator = GetComponentInChildren<Animator>();

        if (portalAnimator != null)
            portalAnimator.SetBool(activeBoolParam, false);

        // AudioSource fallback
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false;          // IMPORTANT: one-shot per enter
            audioSource.volume = 0f;           // start silent, we fade in
            audioSource.Stop();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        // Activate animation
        if (portalAnimator != null)
            portalAnimator.SetBool(activeBoolParam, true);

        // Play audio (once per enter) + fade in
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Stop();       // restart clean if re-enter quickly
            audioSource.volume = 0f;
            audioSource.Play();

            StartFade(targetVolume, fadeInTime);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        // Deactivate animation
        if (portalAnimator != null)
            portalAnimator.SetBool(activeBoolParam, false);

        // Fade out + stop
        if (audioSource != null)
            StartFade(0f, fadeOutTime, stopAtEnd: true);
    }

    private void StartFade(float to, float duration, bool stopAtEnd = false)
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeRoutine(to, duration, stopAtEnd));
    }

    private IEnumerator FadeRoutine(float to, float duration, bool stopAtEnd)
    {
        if (audioSource == null) yield break;

        float from = audioSource.volume;

        if (duration <= 0f)
        {
            audioSource.volume = to;
            if (stopAtEnd && Mathf.Approximately(to, 0f)) audioSource.Stop();
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);
            audioSource.volume = Mathf.Lerp(from, to, k);
            yield return null;
        }

        audioSource.volume = to;

        if (stopAtEnd && Mathf.Approximately(to, 0f))
            audioSource.Stop();
    }
}
