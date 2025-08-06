using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class WorldText : MonoBehaviour
{
    public bool autoFadeOut;
    public float visibleTime;
    public float fadeInTime;
    public float fadeOutTime;
    private ushort state; // 0: hidden, 1: visible, 2: fading in, 3: fading out
    private float visibleTimer;
    private float fadeTimer;
    private bool queueFadeOut;

    TextMeshPro textMesh;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        state = 0;
        queueFadeOut = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (textMesh != null)
            textMesh.alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (textMesh == null)
            return;

        if (fadeTimer > 0)
        {
            AdjustVisibility();
            if (fadeTimer == 0)
            {
                if (state == 2)
                    ShowText();
                else if (state == 3)
                    HideText();
            }
        }
        else if (!autoFadeOut)
            return;
        else if (visibleTimer > 0)
        {
            RunVisibleTimer();
        }
        else if (queueFadeOut)
        {
            queueFadeOut = false;
            FadeOut();
        }
    }

    public void ShowText()
    {
        state = 1;
        textMesh.alpha = 1;
        visibleTimer = visibleTime;
    }

    public void HideText()
    {
        state = 0;
        textMesh.alpha = 0;
    }

    public void FadeIn()
    {
        state = 2;
        // 1 - progress. The more visible the text is, the less time it needs to fade in
        fadeTimer = fadeInTime * (1 - textMesh.alpha / 1);
    }

    public void FadeOut()
    {
        state = 3;
        // The more visible the text is, the more time it needs to fade out
        fadeTimer = fadeOutTime * textMesh.alpha / 1;
    }

    private void AdjustVisibility()
    {
        float progress;

        fadeTimer -= Time.deltaTime;

        if (fadeTimer < 0)
        {
            fadeTimer = 0;
            return;
        }

        if (state == 2)
        {
            progress = Mathf.Clamp01(1 - (fadeTimer / fadeInTime));

            textMesh.alpha = Mathf.Lerp(0, 1, progress);
        }
        else if (state == 3)
        {
            progress = Mathf.Clamp01(1 - (fadeTimer / fadeOutTime));

            textMesh.alpha = Mathf.Lerp(1, 0, progress);
        }
    }

    private void RunVisibleTimer()
    {
        visibleTimer -= Time.deltaTime;
        if (visibleTimer < 0)
        {
            visibleTimer = 0;
            queueFadeOut = true;
        }
    }
}
