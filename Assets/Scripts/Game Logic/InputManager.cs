using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public InputSystem_Actions inputActions;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
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
        
    }

    /// <summary>
    /// Enables and disables the different input maps according to the <paramref name="mode"/> argument.
    /// </summary>
    /// <param name="mode">0: Player enable. 1: UI enabled. -1: None enabled.</param>
    public void ChangeMap(short mode)
    {
        switch (mode)
        {
            case 0:
                inputActions.Player.Enable();
                inputActions.UI.Disable();
                break;
            case 1:
                inputActions.Player.Disable();
                inputActions.UI.Enable();
                break;
            default:
                inputActions.Player.Disable();
                inputActions.UI.Disable();
                break;
        }
    }
}
