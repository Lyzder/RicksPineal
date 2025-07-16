using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class pickup_base : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PickUpEffect(collision.gameObject);
        }
    }

    protected abstract void PickUpEffect(GameObject player);
}
