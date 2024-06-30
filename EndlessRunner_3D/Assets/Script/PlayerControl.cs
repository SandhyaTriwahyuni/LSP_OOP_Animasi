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

    private Vector2 startTouchPosition, endTouchPosition;
    private float swipeRange = 50.0f;
    private bool isSwiping = false;

    public float JumpForce;
    public float Gravity = -20;
    private bool isJumping = false;

    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>(); // Mendapatkan komponen Animator
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
        _animator.SetTrigger("Slide"); // Memanggil trigger "Slide" pada Animator
    }

    private void Jump()
    {
        _direction.y = JumpForce;
        isJumping = true;
        _animator.SetTrigger("Jump");
    }
}
