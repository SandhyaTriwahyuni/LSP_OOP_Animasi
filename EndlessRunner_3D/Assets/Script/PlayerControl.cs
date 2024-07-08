using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private CharacterController _characterController;
    private Vector3 _direction;
    public float ForwardSpeed;

    private int _desiredLane = 1; // 0: Kiri, 1: Tengah, 2: Kanan
    public float LaneDistance = 2.8f;
    public float LaneChangeSpeed = 10f;

    private Vector3 _targetPosition;
    private Vector3 _initialPosition; // Deklarasi variabel _initialPosition

    private Vector2 startTouchPosition, endTouchPosition;
    private float swipeRange = 50.0f;
    private bool isSwiping = false;

    public float JumpForce;
    public float Gravity = -20;
    private bool isJumping = false;

    private Animator _animator;

    // Height awal dan height saat slide
    public float NormalHeight = 1.5f;
    public float SlideHeight = 1.0f;
    private float _originalHeight;
    private float _originalCenterY;

    private bool isSliding = false; // Flag untuk mengecek apakah sedang slide

    // Start is called before the first frame update
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>(); // Mendapatkan komponen Animator
        _initialPosition = transform.position; // Inisialisasi _initialPosition dengan posisi awal
        _originalHeight = _characterController.height; // Menyimpan tinggi awal CharacterController
        _originalCenterY = _characterController.center.y; // Menyimpan nilai awal center.y CharacterController
    }

    // Update is called once per frame
    void Update()
    {
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

    private void FixedUpdate()
    {
        Vector3 currentPosition = transform.position;
        Vector3 nextPosition = Vector3.Lerp(currentPosition, _targetPosition, LaneChangeSpeed * Time.deltaTime);
        Vector3 moveDirection = nextPosition - currentPosition;

        _characterController.Move(moveDirection + _direction * Time.deltaTime);
    }

    // Memeriksa input gesekan (swipe)
    void SwipeCheck()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
                isSwiping = true;
            }
            else if (touch.phase == TouchPhase.Moved && isSwiping)
            {
                endTouchPosition = touch.position;
                DetectSwipe();
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                ResetSwipe();
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

    void ResetSwipe()
    {
        startTouchPosition = endTouchPosition = Vector2.zero;
        isSwiping = false;
    }

    // Tindakan ketika gesekan ke kiri
    void OnSwipeLeft()
    {
        _desiredLane--;
        if (_desiredLane < 0)
            _desiredLane = 0;
    }

    // Tindakan ketika gesekan ke kanan
    void OnSwipeRight()
    {
        _desiredLane++;
        if (_desiredLane > 2)
            _desiredLane = 2;
    }

    // Tindakan ketika gesekan ke atas (untuk melompat)
    void OnSwipeUp()
    {
        Jump();
    }

    // Tindakan ketika gesekan ke bawah (untuk slide)
    void OnSwipeDown()
    {
        if (!isSliding)
        {
            _animator.SetTrigger("Slide"); // Memanggil trigger "Slide" pada Animator
            StartCoroutine(SlideCoroutine());
        }
    }

    private void Jump()
    {
        if (_characterController.isGrounded)
        {
            _direction.y = JumpForce;
            isJumping = true;
            _animator.SetTrigger("Jump");
        }
    }

    private void ReturnToRunningAnimation()
    {
        // Setelah animasi selesai, pastikan kembali ke posisi yang benar
        _targetPosition.z += transform.position.z - _initialPosition.z;
    }

    private IEnumerator SlideCoroutine()
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
