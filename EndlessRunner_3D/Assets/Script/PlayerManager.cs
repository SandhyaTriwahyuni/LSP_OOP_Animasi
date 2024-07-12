using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static bool GameOver;
    public GameObject GameOverPanel;

    //Variabel untuk StartGame
    public static bool IsGameStarted;
    public GameObject StartingText;

    public PlayerControl playerControl; // Make it public

    // Start is called before the first frame update
    void Start()
    {
        GameOver = false;
        Time.timeScale = 1; // untuk replay
        IsGameStarted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameOver)
        {
            Time.timeScale = 0;
            GameOverPanel.SetActive(true);
        }

        // Detect tap to start the game
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !IsGameStarted)
        {
            playerControl.OnTap();
            IsGameStarted = true;
            Destroy(StartingText);
        }

    }
}
