using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static bool GameOver;
    public GameObject GameOverPanel;
    public GameObject HUD;
    public ScoreSystem ScoreSystem;

    // Variabel untuk StartGame
    public static bool IsGameStarted;
    public GameObject StartingText;

    public PlayerControl playerControl;
    public Animator PlayerAnimator;

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
            StartCoroutine(GameOverSequence());
        }

        // Detect tap to start the game
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !IsGameStarted)
        {
            playerControl.OnTap();
            IsGameStarted = true;
            Destroy(StartingText);
        }
    }

    IEnumerator GameOverSequence()
    {
        PlayerAnimator.SetTrigger("Die");
        // Menunggu hingga animasi "Die" selesai
        yield return new WaitForSecondsRealtime(2f);
       

        Time.timeScale = 0;
        HUD.SetActive(false);
        GameOverPanel.SetActive(true);
        if (ScoreSystem != null)
        {
            ScoreSystem.DisplayGameOverScore();
        }
    }
}
