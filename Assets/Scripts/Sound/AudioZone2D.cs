using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioZone2D : MonoBehaviour
{
    [Header("Fade")]
    [SerializeField] private float fadeInTime = 0.25f;
    [SerializeField] private float fadeOutTime = 0.35f;

    private AudioSource audioSource;
    private Coroutine fadeRoutine;
    private float baseVolume;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        baseVolume = audioSource.volume;   // guarda tu volumen “real”
        audioSource.volume = 0f;           // arranca en 0 para que no “cliquee”
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        FadeTo(baseVolume, fadeInTime, startIfNeeded: true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        FadeTo(0f, fadeOutTime, stopWhenZero: true);
    }

    private void FadeTo(float target, float time, bool startIfNeeded = false, bool stopWhenZero = false)
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeRoutine(target, time, startIfNeeded, stopWhenZero));
    }

    private IEnumerator FadeRoutine(float target, float time, bool startIfNeeded, bool stopWhenZero)
    {
        if (startIfNeeded && !audioSource.isPlaying)
            audioSource.Play();

        float start = audioSource.volume;
        float t = 0f;

        if (time <= 0f)
        {
            audioSource.volume = target;
        }
        else
        {
            while (t < time)
            {
                t += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(start, target, t / time);
                yield return null;
            }
            audioSource.volume = target;
        }

        if (stopWhenZero && Mathf.Approximately(target, 0f))
            audioSource.Stop();

        fadeRoutine = null;
    }
}
