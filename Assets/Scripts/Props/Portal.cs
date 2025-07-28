using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : Interactable
{
    [SerializeField] private Vector3 coordinates;
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
        player.transform.position = coordinates;
    }
}
