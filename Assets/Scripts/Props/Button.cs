using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Button : Interactable
{
    public UnityEvent onTriggerActivated;
    [SerializeField] private AudioClip clickSfx;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void PlayAction(PlayerController player)
    {
        AudioManager.Instance.PlaySFX(clickSfx);
        ActivateTrigger();
    }

    protected void ActivateTrigger()
    {
        onTriggerActivated?.Invoke();
    }
}
