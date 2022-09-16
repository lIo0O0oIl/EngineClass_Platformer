using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControler : MonoBehaviour
{
    public float walkSpeed = 10f;
    public float gravity = 20f;

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

        if (_characterController.below)
        {

        }
        else
        {
            _moveDirection.y -= gravity * Time.deltaTime;
        }

        _characterController.Move(_moveDirection * Time.deltaTime);
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
    }



}
