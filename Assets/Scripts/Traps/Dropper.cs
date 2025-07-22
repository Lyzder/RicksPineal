using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dropper : MonoBehaviour
{
    [SerializeField] private GameObject drop;
    [SerializeField] private Transform spawnPoint;
    public float idleTime;
    public float spawnTime;
    public float startTime;
    private float timer;
    private bool state;

    // Components
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        state = false;
        timer = startTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (state)
        {
            SpawnCycle();
        }
        else
        {
            IdleCycle();
        }
    }

    private void SpawnDrop()
    {
        if (drop != null)
            Instantiate(drop, spawnPoint.position, Quaternion.identity);
    }

    private void IdleCycle()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            state = true;
            timer = spawnTime;
        }
    }
    
    private void SpawnCycle()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            state = false;
            timer = idleTime;
            SpawnDrop();
        }
    }
}
