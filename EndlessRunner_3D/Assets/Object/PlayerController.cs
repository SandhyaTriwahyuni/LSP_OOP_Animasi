using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController _characterController;
    private Vector3 _direction;
    public float ForwardSpeed;

    private int _desiredLane = 1; 
    public float LaneDistance = 2.5f;
    public float LaneChangeSpeed = 10f; 

    public float JumpForce;
    public float Gravity = -20;


    private Vector3 _targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        _direction.z = ForwardSpeed;
        _direction.y += Gravity * Time.deltaTime;
        if (_characterController.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }



        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _desiredLane++;
            if (_desiredLane == 3)
                _desiredLane = 2;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _desiredLane--;
            if (_desiredLane == -1)
                _desiredLane = 0;
        }

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

    private void Jump()
    {
        _direction.y = JumpForce;
    }
}
