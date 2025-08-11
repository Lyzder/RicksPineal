using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-90)]
public class OverlayManager : MonoBehaviour
{
    public static OverlayManager Instance {  get; private set; }

    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private IrisController irisTransition;

    private Camera mainCamera;

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
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    public void MakeIrisVisible(bool visible)
    {
        irisTransition.MakeVisible(visible);
    }

    public void PlayIrisIn()
    {
        irisTransition.TransitionIn();
    }

    public void PlayIrisIn(Vector2 pos)
    {
        irisTransition.TransitionIn(pos);
    }

    public void PlayIrisIn(Transform pos)
    {
        irisTransition.TransitionIn(pos);
    }

    public void PlayIrisOut()
    {
        irisTransition.TransitionOut();
    }

    public void PlayIrisOut(Vector2 pos)
    {
        irisTransition.TransitionOut(pos);
    }

    public void PlayIrisOut(Transform pos)
    {
        irisTransition.TransitionOut(pos);
    }

    public void ForceIrisHoldOut()
    {
        irisTransition.ForceHoldOut();
    }

    public void ForceIrisHoldIn()
    {
        irisTransition.ForceHoldIn();
    }

    public void ShowGameOverScreen()
    {
        gameOverScreen.SetActive(true);
    }

    public void HideGameOverScreen()
    { 
        gameOverScreen.SetActive(false); 
    }
}
