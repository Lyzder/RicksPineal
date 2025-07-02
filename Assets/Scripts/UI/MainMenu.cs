using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class MainMenu : MonoBehaviour
{
    private InputSystem_Actions inputActions;

    private void Awake()
    {
        inputActions = InputManager.Instance.inputActions;
    }

    private void OnEnable()
    {
        inputActions.UI.Submit.performed += OnSubmit;
    }

    private void OnDisable()
    {
        inputActions.UI.Submit.performed -= OnSubmit;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnSubmit(InputAction.CallbackContext ctx)
    {
        Debug.Log("Submit received");
        GameManager.Instance.StartGame();
    }
}
