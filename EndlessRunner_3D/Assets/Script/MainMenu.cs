using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI HiScoreText;
    public AudioMixer AudioMixer;
    public Slider MusicSlider;
    public Slider SfxSlider;

    private void Start()
    {
        LoadVolume();
        Time.timeScale = 1;

        //Memainkan music
        MusicManager.Instance.PlayMusic("MainMenu");

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
        MusicManager.Instance.PlayMusic("GamePlay");
    }
    public void UpdateMusicVolume(float volume)
    {
        AudioMixer.SetFloat("MusicVol", volume);
    }

    public void UpdateSoundVolume(float volume)
    {
        AudioMixer.SetFloat("SFXVol", volume);
    }

    public void SaveVolume()
    {
        AudioMixer.GetFloat("MusicVol", out float musicVolume);
        PlayerPrefs.SetFloat("MusicVol", musicVolume);

        AudioMixer.GetFloat("SFXVol", out float sfxVolume);
        PlayerPrefs.SetFloat("SFXVol", sfxVolume);
    }

    public void LoadVolume()
    {
        MusicSlider.value = PlayerPrefs.GetFloat("MusicVol");
        SfxSlider.value = PlayerPrefs.GetFloat("SFXVol");
    }

}
