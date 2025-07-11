using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[DefaultExecutionOrder(-90)]
public class ScreenTransitionManager : MonoBehaviour
{
    public static ScreenTransitionManager Instance { get; private set; }

    private GameObject cameraFocus;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraFocus == null)
        {
            FindCameraFocus();
        }
    }

    private void FindCameraFocus()
    {
        cameraFocus = GameObject.FindGameObjectWithTag("CameraFocus");
    }

    public void MoveToScreen(ScreenData screen)
    {
        if (screen == null) return;

        cameraFocus.transform.position = screen.cameraPosition;
    }
}
