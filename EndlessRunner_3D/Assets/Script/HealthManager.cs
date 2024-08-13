using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public Slider HealthBar;
    public float MaxHealth = 50f;
    private float _currentHealth;
    public float HealthDecreaseRate = 2f;
    public float HealthIncreaseAmount = 3f;
    public Gradient Gradient;
    public Image Fill;

    public PlayerManager PlayerManager;

    void Start()
    {
        _currentHealth = MaxHealth;
        HealthBar.maxValue = MaxHealth;
        HealthBar.value = _currentHealth;
        PlayerManager = FindObjectOfType<PlayerManager>();
        UpdateHealthBarColor();
    }

    void Update()
    {
        if (PlayerManager.IsGameStarted && !PlayerManager.GameOver)
        {
            DecreaseHealth(); // Kurangi health saat game sudah dimulai dan belum game over
        }
    }

    void DecreaseHealth()
    {
        _currentHealth -= HealthDecreaseRate * Time.deltaTime; // Kurangi health saat ini
        if (_currentHealth < 0)
        {
            _currentHealth = 0; // Pastikan health tidak negatif
            PlayerManager.GameOver = true; // Aktifkan game over jika health mencapai 0
        }
        HealthBar.value = _currentHealth; // Perbarui nilai slider
        UpdateHealthBarColor(); // Perbarui warna slider
    }

    public void IncreaseHealth()
    {
        // Hanya izinkan peningkatan kesehatan jika game belum berakhir
        if (!PlayerManager.GameOver)
        {
            _currentHealth += HealthIncreaseAmount; // Tambahkan health saat ini
            if (_currentHealth > MaxHealth)
            {
                _currentHealth = MaxHealth; // Pastikan health tidak melebihi maksimum
            }
            HealthBar.value = _currentHealth; // Perbarui nilai slider
            UpdateHealthBarColor(); // Perbarui warna slider
        }
    }

    public void EnemyHit()
    {
        _currentHealth -= 50f; // Mengurangi health sebesar 50
        if (_currentHealth < 0)
        {
            _currentHealth = 0; // Pastikan health tidak negatif
            PlayerManager.GameOver = true; // Aktifkan game over jika health mencapai 0
        }
        HealthBar.value = _currentHealth; // Perbarui nilai slider
        UpdateHealthBarColor(); // Perbarui warna slider
    }

    public void ObstacleHit()
    {
        _currentHealth -= 10f; // Mengurangi health sebesar 10
        if (_currentHealth < 0)
        {
            _currentHealth = 0; // Pastikan health tidak negatif
            PlayerManager.GameOver = true; // Aktifkan game over jika health mencapai 0
        }
        HealthBar.value = _currentHealth; // Perbarui nilai slider
        UpdateHealthBarColor(); // Perbarui warna slider
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Medicine"))
        {
            IncreaseHealth();
            Destroy(other.gameObject); // Hapus obat setelah dikumpulkan
        }
    }

    void UpdateHealthBarColor()
    {
        // Gunakan nilai normalisasi untuk mengevaluasi gradient
        Fill.color = Gradient.Evaluate(HealthBar.normalizedValue);
    }
}
