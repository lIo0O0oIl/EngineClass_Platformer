using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControler : MonoBehaviour
{
    //Player properties
    [Header("Player Propertise")]   //�ν����� â�� ������ ���� ����
    public float walkSpeed = 10f;
    public float gravity = 20f;
    public float jumpSpeed = 15f;
    public float doubleJumpSpeed = 10f;
    public float tripleJumpSpeed = 10f;
    public float xWallJumpSpeed = 15f;
    public float yWallJumpSpeed = 15f;

    //Player Abilities
    [Header("Player Abilities")]
    public bool canDoubleJump;
    public bool canTripleJump;
    public bool canWallJump;

    //Player states
    [Header("Player States")]
    public bool isJumping;
    public bool isDoubleJumping;
    public bool isTripleJumping;
    public bool isWallJumping;

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
        if (!isWallJumping)
        {
            _moveDirection.x = _input.x * walkSpeed;

            if (_moveDirection.x < 0f)  //���� �̵�
            {
                transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }
            else if (_moveDirection.x > 0f) // ������ �̵�
            {
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }

        if (_characterController.below) // on the ground
        {
            _moveDirection.y = 0;

            isJumping = false;
            isDoubleJumping = false;
            isTripleJumping = false;
            isWallJumping = false;

            if (_startJump == true)
            {
                _startJump = false;
                isJumping = true;
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

            //pressed jump button in the air
            if (_startJump )//&& !isDoubleJumping)  //�̰� ���� �� �ڵ�
            {
                //triple jump
                if (canTripleJump && !_characterController.right && !_characterController.left)
                {
                    if (isDoubleJumping && !isTripleJumping)
                    {
                        Debug.Log("3������");
                        _moveDirection.y = tripleJumpSpeed;
                        isTripleJumping = true;
                    }
                }

                //double jump
                if (canDoubleJump && !_characterController.right && !_characterController.left)
                {
                    if (!isDoubleJumping)
                    {
                        Debug.Log("2������");
                        _moveDirection.y = doubleJumpSpeed;
                        isDoubleJumping = true;
                    }
                }

                //Wall jump
                if (canWallJump && (_characterController.right || _characterController.left))   //���� ������ ���� ������ ���˺��� ����.
                {
                    if (_characterController.right)
                    {
                        Debug.Log("������ �� ����");
                        _moveDirection.x = -xWallJumpSpeed;
                        _moveDirection.y = xWallJumpSpeed;
                        transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                    }
                    else if(_characterController.left)
                    {
                        Debug.Log("���� �� ����");
                        _moveDirection.x = xWallJumpSpeed;
                        _moveDirection.y = xWallJumpSpeed;
                        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                    }

                    //isWallJumping = true;
                    StartCoroutine("WallJumpWaiter");
                }


                _startJump = false;
            }

            /*if (_startJump && isDoubleJumping)    //���� �� �ڵ�
            {
                //triple jump
                if (canDoubleJump && canTripleJump && isDoubleJumping && !_characterController.right && !_characterController.left)
                {
                    if (!isTripleJumping)
                    {
                        Debug.Log("3������");
                        _moveDirection.y = tripleJumpSpeed;
                        isTripleJumping = true;
                    }
                }
                _startJump = false;
            }*/

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

            Debug.Log("��������");
        }
        else if (context.canceled)
        {
            _startJump = false;
            _releaseJump = true;

            Debug.Log("������");
        }
    }

    IEnumerator WallJumpWaiter()
    {
        isWallJumping = true;
        yield return new WaitForSeconds(0.4f);
        isWallJumping = false;
    }



}
