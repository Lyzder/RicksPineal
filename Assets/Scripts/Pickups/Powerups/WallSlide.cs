using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSlide : pickup_base
{
    protected override void PickUpEffect(GameObject player)
    {
        player.GetComponent<PlayerController>().GetWallSlide();
        Destroy(this.gameObject);
    }
}
