using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControler : MonoBehaviour
{
    public float walkSpeed = 10f;
    public float gravity = 20f;
    public float jumpSpeed = 15f;

    //input flags
    bool _startJump;
    bool _releaseJump;

    private Vector2 _input;
    private Vector2 _moveDirection;
    private characterController2D _characterController;

    void Start()
    {
        _characterController = GetComponent<characterController2D>();
    }

    void Update()
    {
        _moveDirection.x = _input.x * walkSpeed;

        if (_moveDirection.x < 0f)  //왼쪽 이동
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else if (_moveDirection.x > 0f) // 오른쪽 이동
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

        if (_characterController.below) // on the ground
        {
            _moveDirection.y = 0;

            if (_startJump == true)
            {
                _startJump = false;
                _moveDirection.y = jumpSpeed;
                _characterController.DisableGroundCheck();
            }
        }
        else   // in the air
        {
            if (_releaseJump)
            {
                _releaseJump = false;

                if (_moveDirection.y > 0f)
                {
                    _moveDirection.y *= 0.5f;
                }
            }
            CravityCalculation();
        }

        _characterController.Move(_moveDirection * Time.deltaTime);
    }

    private void CravityCalculation()
    {
        if (_moveDirection.y > 0f && _characterController.above)
        {
            _moveDirection.y = 0f;
        }
        _moveDirection.y -= gravity * Time.deltaTime;
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _startJump = true;
            _releaseJump = false;
        }
        else if (context.canceled)
        {
            _startJump = false;
            _releaseJump = true;
        }
    }





}
