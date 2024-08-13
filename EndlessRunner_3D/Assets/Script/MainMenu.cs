using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI HiScoreText;

    private void Start()
    {
        Time.timeScale = 1;

        // Ambil high score dari PlayerPrefs
        float highScore = PlayerPrefs.GetFloat("HighScore", 0f);

        // Tampilkan high score di UI
        if (HiScoreText != null)
        {
            HiScoreText.text = Mathf.Round(highScore).ToString();
        }
    }
    public void PlayGame()
    {
        SceneManager.LoadScene("GamePlay");
    }


}
