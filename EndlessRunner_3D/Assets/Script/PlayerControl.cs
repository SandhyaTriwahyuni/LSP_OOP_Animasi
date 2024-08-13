using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private CharacterController _characterController;
    private Vector3 _direction;
    public float ForwardSpeed;
    public float MaxSpeed;

    private int _desiredLane = 1; // 0: Kiri, 1: Tengah, 2: Kanan
    public float LaneDistance = 2.8f;
    public float LaneChangeSpeed = 10f;

    private Vector3 _targetPosition;
    private Vector3 _initialPosition; // Deklarasi variabel _initialPosition

    private Vector2 startTouchPosition, endTouchPosition;
    private float swipeRange = 30.0f;
    private float tapRange = 10.0f; // Range to detect a tap
    private float tapTimeMax = 0.2f; // Maximum time to detect a tap
    private float tapTime;

    private bool isSwiping = false;

    public float JumpForce;
    public float MaxJumpForce;
    public float Gravity = -10;
    private bool isJumping = false;

    private Animator _animator;

    // Height awal dan height saat slide
    public float NormalHeight = 1.5f;
    public float SlideHeight = 1.0f;
    private float _originalHeight;
    private float _originalCenterY;

    private bool isSliding = false; // Flag untuk mengecek apakah sedang slide

    //PowerUp
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float bulletSpeed = 20f;
    public int bulletsToShoot = 8;

    public HealthManager HealthManager;
    public PlayerManager PlayerManager; // Tambahkan referensi ke PlayerManager

    // Start is called before the first frame update
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>(); // Mendapatkan komponen Animator
        _initialPosition = transform.position; // Inisialisasi _initialPosition dengan posisi awal
        _originalHeight = _characterController.height; // Menyimpan tinggi awal CharacterController
        _originalCenterY = _characterController.center.y; // Menyimpan nilai awal center.y CharacterController

        HealthManager = FindObjectOfType<HealthManager>();
        PlayerManager = FindObjectOfType<PlayerManager>(); // Cari PlayerManager pada scene

        // Menjadikan bulletSpawnPoint anak dari Player
        bulletSpawnPoint.parent = transform;
    }

    // Update is called once per frame
    void Update()
    {
        // Periksa apakah game sudah dimulai dan belum game over
        if (PlayerManager.IsGameStarted && !PlayerManager.GameOver)
        {
            _animator.SetBool("isStarted", true);

            //IncreaseSpeed
            if (ForwardSpeed < MaxSpeed)
            {
                ForwardSpeed += 0.1f * Time.deltaTime;
            }

            if (JumpForce < MaxJumpForce)
            {
                JumpForce += 0.001f * Time.deltaTime;
            }

            _direction.z = ForwardSpeed;
            _direction.y += Gravity * Time.deltaTime;

            if (_characterController.isGrounded)
            {
                if (isJumping)
                {
                    isJumping = false;
                    ReturnToRunningAnimation();
                }
            }

            // Memeriksa input gesekan (swipe)
            SwipeCheck();

            // Menghitung posisi target berdasarkan jalur yang diinginkan
            Vector3 lanePosition = transform.position.z * transform.forward + transform.position.y * transform.up;

            if (_desiredLane == 0)
            {
                lanePosition += Vector3.left * LaneDistance;
            }
            else if (_desiredLane == 2)
            {
                lanePosition += Vector3.right * LaneDistance;
            }

            _targetPosition = new Vector3(lanePosition.x, transform.position.y, transform.position.z);
        }
        else
        {
            // Game over atau belum dimulai, hentikan pergerakan
            _direction.z = 0;
        }
    }

    private void FixedUpdate()
    {
        if (PlayerManager.IsGameStarted && !PlayerManager.GameOver)
        {
            Vector3 currentPosition = transform.position;
            Vector3 nextPosition = Vector3.Lerp(currentPosition, _targetPosition, LaneChangeSpeed * Time.deltaTime);
            Vector3 moveDirection = nextPosition - currentPosition;

            _characterController.Move(moveDirection + _direction * Time.deltaTime);
        }
    }

    // Memeriksa input gesekan (swipe) dan tap
    void SwipeCheck()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
                isSwiping = true;
                tapTime = 0; // Added for tap detection
            }
            else if (touch.phase == TouchPhase.Moved && isSwiping)
            {
                endTouchPosition = touch.position;
                DetectSwipe();
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                endTouchPosition = touch.position;
                if (isSwiping)
                {
                    DetectSwipe();
                }
                DetectTap(); // Added for tap detection
                ResetSwipe();
            }

            // Increment the tap timer
            if (isSwiping)
            {
                tapTime += Time.deltaTime; // Added for tap detection
            }
        }
    }

    // Mendeteksi gesekan (swipe)
    void DetectSwipe()
    {
        Vector2 distance = endTouchPosition - startTouchPosition;

        if (distance.magnitude > swipeRange)
        {
            if (Mathf.Abs(distance.x) > Mathf.Abs(distance.y))
            {
                // Gesekan ke kiri atau ke kanan
                if (distance.x > 0)
                {
                    OnSwipeRight();
                }
                else
                {
                    OnSwipeLeft();
                }
            }
            else
            {
                // Gesekan ke atas atau ke bawah
                if (distance.y > 0)
                {
                    OnSwipeUp();
                }
                else
                {
                    OnSwipeDown();
                }
            }
            ResetSwipe();
        }
    }

    void DetectTap() // Added for tap detection
    {
        Vector2 distance = endTouchPosition - startTouchPosition;

        if (distance.magnitude < tapRange && tapTime < tapTimeMax)
        {
            OnTap();
        }
    }

    void ResetSwipe()
    {
        startTouchPosition = endTouchPosition = Vector2.zero;
        isSwiping = false;
    }

    // Tindakan ketika gesekan ke kiri
    void OnSwipeLeft()
    {
        if (PlayerManager.IsGameStarted && !PlayerManager.GameOver)
        {
            _desiredLane--;
            if (_desiredLane < 0)
                _desiredLane = 0;
        }
    }

    // Tindakan ketika gesekan ke kanan
    void OnSwipeRight()
    {
        if (PlayerManager.IsGameStarted && !PlayerManager.GameOver)
        {
            _desiredLane++;
            if (_desiredLane > 2)
                _desiredLane = 2;
        }
    }

    // Tindakan ketika gesekan ke atas (untuk melompat)
    void OnSwipeUp()
    {
        if (PlayerManager.IsGameStarted && !PlayerManager.GameOver)
        {
            Jump();
        }
    }

    // Tindakan ketika gesekan ke bawah (untuk slide)
    void OnSwipeDown()
    {
        if (PlayerManager.IsGameStarted && !PlayerManager.GameOver)
        {
            if (!isSliding)
            {
                _animator.SetTrigger("Slide"); // Memanggil trigger "Slide" pada Animator
                StartCoroutine(SlideCoroutine());
            }
        }
    }

    // Tindakan ketika tap
    public void OnTap() // Added for tap detection
    {
        //if (PlayerManager.IsGameStarted && !PlayerManager.GameOver)
        {
         
        }
    }

    private void Jump()
    {
        if (PlayerManager.IsGameStarted && !PlayerManager.GameOver)
        {
            if (_characterController.isGrounded)
            {
                _direction.y = JumpForce;
                isJumping = true;
                _animator.SetTrigger("Jump");
            }

            
        }
    }

    private void ReturnToRunningAnimation()
    {
        // Setelah animasi selesai, pastikan kembali ke posisi yang benar
        _targetPosition.z += transform.position.z - _initialPosition.z;
    }

    private IEnumerator SlideCoroutine()
    {
        if (PlayerManager.IsGameStarted && !PlayerManager.GameOver)
        {
            isSliding = true; // Mengatur flag isSliding menjadi true
            _characterController.height = SlideHeight; // Ubah tinggi CharacterController saat slide
            _characterController.center = new Vector3(_characterController.center.x, 0.4f, _characterController.center.z); // Ubah center.y saat slide
            yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length); // Tunggu hingga animasi slide selesai
            _characterController.height = _originalHeight; // Kembalikan tinggi CharacterController setelah animasi slide selesai
            _characterController.center = new Vector3(_characterController.center.x, _originalCenterY, _characterController.center.z); // Kembalikan center.y setelah animasi slide selesai
            isSliding = false; // Mengatur flag isSliding menjadi false setelah slide selesai
        }
    }

    //Ketika Player menabrak Obstacle
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.tag == "Obstacle")
        {
            if (PlayerManager.IsGameStarted && !PlayerManager.GameOver)
            {
                HealthManager.ObstacleHit();
                Destroy(hit.gameObject);
            }
        }
    }

    public void ActivatePowerUp()
    {
        if (PlayerManager.IsGameStarted && !PlayerManager.GameOver)
        {
            StartCoroutine(ShootBullets());
        }
    }

    private IEnumerator ShootBullets()
    {
        for (int i = 0; i < bulletsToShoot; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.velocity = bulletSpawnPoint.forward * bulletSpeed;

            yield return new WaitForSeconds(0.1f); // Jeda waktu antara setiap tembakan
        }
    }
}
