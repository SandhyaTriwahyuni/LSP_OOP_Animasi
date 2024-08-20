using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 2.0f;
    public float leftBoundary = -2.8f;
    public float rightBoundary = 2.8f;
    public HealthManager HealthManager;
    private Vector3 _targetPosition;
    private Animator _animator;
    private int _hitCount = 0;
    private bool _isDead = false;

    void Start()
    {
        _animator = GetComponent<Animator>();
        HealthManager = FindObjectOfType<HealthManager>();
        _targetPosition = new Vector3(leftBoundary, transform.position.y, transform.position.z);
    }

    void Update()
    {
        if (_isDead) return;

        Move();

        if (transform.position.x <= leftBoundary)
        {
            _targetPosition = new Vector3(rightBoundary, transform.position.y, transform.position.z);
            _animator.SetTrigger("MoveRight");
        }
        else if (transform.position.x >= rightBoundary)
        {
            _targetPosition = new Vector3(leftBoundary, transform.position.y, transform.position.z);
            _animator.SetTrigger("MoveLeft");
        }
    }

    void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, speed * Time.deltaTime);
    }

    public void TakeDamage()
    {
        if (_isDead) return;

        _hitCount++;
        _animator.SetTrigger("Hit");

        if (_hitCount >= 3)
        {
            Die();
        }
    }

    void Die()
    {
        _isDead = true;
        _animator.SetTrigger("Die");
        SoundManager.Instance.PlaySound3D("Hit", transform.position);
        Destroy(gameObject, 2f); // Menghapus game object setelah animasi mati
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_isDead)
        {
            _animator.SetTrigger("Attack");
            HealthManager.EnemyHit();
        }

        else if (other.CompareTag("Bullet"))
        {
            TakeDamage();
            Destroy(other.gameObject); // Menghapus peluru setelah terkena enemy
        }
    }
}
