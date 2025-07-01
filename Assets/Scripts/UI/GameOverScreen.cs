using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    private bool isShown;
    private InputSystem_Actions inputActions;

    private void Awake()
    {
        inputActions = InputManager.Instance.inputActions;
    }

    // Start is called before the first frame update
    void Start()
    {
        isShown = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        GameManager.Instance.OnWinning += ShowScreen;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnWinning -= ShowScreen;
        if (isShown)
        {
            inputActions.UI.Submit.performed -= OnSubmit;
        }
    }

    private void ShowScreen()
    {
        canvas.SetActive(true);
        isShown = true;

        inputActions.UI.Submit.performed += OnSubmit;
    }

    private void OnSubmit(InputAction.CallbackContext ctx)
    {
        GameManager.Instance.StartGame();
    }
}
