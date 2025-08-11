using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-99)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public string startingLevel;
    public float respawnTransitionDuration;

    //public event Action OnWinning;

    private GameObject player;
    private Vector2 respawnPoint;
    private Camera mainCamera;
    private InputSystem_Actions inputActions;
    private bool win;

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
        mainCamera = Camera.main;
        inputActions = InputManager.Instance.inputActions;
        win = false;
        //Instantiate(eventSystem);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Main Menu")
            InputManager.Instance.ChangeMap(1);
        else
            InputManager.Instance.ChangeMap(0);
    }

    // Update is called once per frame
    void Update()
    {
       if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        SceneManager.activeSceneChanged += PrepareForScene;
        if (inputActions == null)
            return;
        inputActions.UI.Submit.performed += OnSubmit;
    }

    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= PrepareForScene;
        if (inputActions == null)
            return;
        inputActions.UI.Submit.performed -= OnSubmit;
    }

    private void PrepareForScene(Scene arg0, Scene arg1)
    {
        string sceneName = arg1.name;

        switch (sceneName)
        {
            case "Main Menu":
                InputManager.Instance.ChangeMap(1);
                OverlayManager.Instance.MakeIrisVisible(false);
                break;
            default:
                InputManager.Instance.ChangeMap(0);
                OverlayManager.Instance.MakeIrisVisible(true);
                OverlayManager.Instance.ForceIrisHoldOut();
                OverlayManager.Instance.PlayIrisIn();
                break;
        }

        OverlayManager.Instance.HideGameOverScreen();
    }

    public void StartGame()
    {
        OverlayManager.Instance.ForceIrisHoldOut();
        SceneManager.LoadScene(startingLevel);
        InputManager.Instance.ChangeMap(0);
        win = false;
    }

    public void WinGame()
    {
        OverlayManager.Instance.ShowGameOverScreen();
        InputManager.Instance.ChangeMap(1);
        win = true;
    }

    public void SetRespawnPoint(Vector2 point)
    {
        respawnPoint = point;
    }

    public void RespawnPlayer()
    {
        //TODO
        Vector2 coord;
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            coord = mainCamera.WorldToScreenPoint(player.transform.position);
            StartCoroutine(PlayIrisTransitionComplete(coord));
            StartCoroutine(TimeRespawn());
        }
    }

    private IEnumerator TimeRespawn()
    {
        yield return new WaitForSeconds(respawnTransitionDuration);
        MovePlayer(respawnPoint);
        player.GetComponent<PlayerController>().ResetPlayer();
    }

    private IEnumerator PlayIrisTransitionComplete(Vector2 coord)
    {
        OverlayManager.Instance.PlayIrisOut(player.transform);
        yield return new WaitForSeconds(respawnTransitionDuration);
        OverlayManager.Instance.PlayIrisIn(mainCamera.WorldToScreenPoint(respawnPoint));
    }

    private void MovePlayer(Vector2 coord)
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            return;

        player.transform.position = coord;
    }

    private void OnSubmit(InputAction.CallbackContext ctx)
    {
        if (win)
            StartGame();
    }
}
