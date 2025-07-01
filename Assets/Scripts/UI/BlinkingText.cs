using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlinkingText : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    public float interval;
    private float timer;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (textMesh == null)
            return;

        timer += Time.deltaTime;
        if (timer >= interval)
        {
            textMesh.enabled = !textMesh.enabled;
            timer = 0;
        }
    }
}
