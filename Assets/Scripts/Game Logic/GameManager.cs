using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-99)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public string startingLevel;

    public event Action OnWinning;

    private Vector2 respawnPoint;

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
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene(startingLevel);
        InputManager.Instance.ChangeMap(0);
    }

    public void WinGame()
    {
        OnWinning?.Invoke();
        InputManager.Instance.ChangeMap(1);
    }

    public void SetRespawnPoint(Vector2 point)
    {
        respawnPoint = point;
    }

    public void RespawnPlayer()
    {
        //TODO
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = respawnPoint;
            player.GetComponent<PlayerController>().ResetPlayer();
        }
    }
}
