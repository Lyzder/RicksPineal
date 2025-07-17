using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicLooper : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clip;
    public int loopStartSample;
    public int loopEndSample;
    public bool loop;

    private void Awake()
    {
        
    }

    void Start()
    {
        
    }

    // Update is called once per frame

    void Update()
    {
        if (!audioSource.isPlaying || audioSource.clip == null) return;

        if (loop)
        {
            LoopWithPoints();
        }
    }

    public void PlayClip()
    {
        audioSource.Stop();
        audioSource.timeSamples = 0;
        StartPlayback();
    }

    public void StartPlayback()
    {
        if (audioSource == null || audioSource.clip == null) return;

        audioSource.Play();
    }

    public void PausePlayback()
    {
        if (audioSource == null ||  !audioSource.isPlaying) return;

        audioSource.Pause();
    }

    public void UnPausePlayback()
    {
        if (audioSource == null || audioSource.isPlaying) return;

        audioSource.UnPause();
    }

    public void StopPlayback()
    {
        if (audioSource == null || audioSource.clip == null) return;

        audioSource.Stop();
    }

    public void SetClip(AudioClip clip, bool loop)
    {
        this.clip = clip;
        loopStartSample = 0;
        loopEndSample = 0;
        audioSource.clip = clip;
        SetLoopType(loop);
    }

    public void SetClip(AudioClip clip, int loopStartSample, int loopEndSample, bool loop)
    {
        this.clip = clip;
        this.loopStartSample = loopStartSample;
        this.loopEndSample = loopEndSample;
        audioSource.clip = clip;
        SetLoopType(loop);
    }

    private void SetLoopType(bool loop)
    {
        if (loop)
        {
            if (loopStartSample == 0 && loopEndSample == 0)
            {
                audioSource.loop = true;
                this.loop = false;
            }
            else
            {
                audioSource.loop = false;
                this.loop = true;
            }
        }
        else
        {
            audioSource.loop = false;
            this.loop = false;
        }
    }

    private void LoopWithPoints()
    {
        if (audioSource.timeSamples >= loopEndSample)
        {
            audioSource.timeSamples = loopStartSample;
        }
    }
}
