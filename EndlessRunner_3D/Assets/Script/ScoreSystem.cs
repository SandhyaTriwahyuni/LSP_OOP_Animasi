using UnityEngine;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
    public TextMeshProUGUI ScoreTextHUD; // Untuk UI HUD
    public TextMeshProUGUI ScoreTextGameOver; // Untuk UI Game Over
    public TextMeshProUGUI HiScoreText;

    private float _startTime;
    private float _score;
    private float _highScore;

    private const float PointsPerMillisecond = 10f;

    void Start()
    {
        _startTime = Time.time;
        _score = 0;
        _highScore = PlayerPrefs.GetFloat("HighScore", 0f);
        UpdateScoreHUD();
        UpdateHighScore();
    }

    void Update()
    {
        if (!PlayerManager.IsGameStarted || PlayerManager.GameOver)
            return;

        float currentTime = Time.time - _startTime;
        _score = currentTime * PointsPerMillisecond;
        UpdateScoreHUD();
    }

    void UpdateScoreHUD()
    {
        if (ScoreTextHUD != null)
        {
            ScoreTextHUD.text = "SCORE: " + Mathf.Round(_score).ToString();
        }
    }

    public void DisplayGameOverScore()
    {
        if (ScoreTextGameOver != null)
        {
            ScoreTextGameOver.text = "SCORE: " + Mathf.Round(_score).ToString();
        }

        // Periksa apakah skor saat ini lebih tinggi dari high score
        if (_score > _highScore)
        {
            _highScore = _score;
            PlayerPrefs.SetFloat("HighScore", _highScore);
            PlayerPrefs.Save();
            UpdateHighScore(); // Perbarui tampilan high score
        }
    }

    void UpdateHighScore()
    {
        if (HiScoreText != null)
        {
            HiScoreText.text = "HIGH SCORE: " + Mathf.Round(_highScore).ToString();
        }
    }
}
