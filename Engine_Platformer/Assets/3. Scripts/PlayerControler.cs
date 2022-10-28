using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControler : MonoBehaviour
{
    //Player properties
    [Header("Player Propertise")]   //인스펙터 창에 나오는 변수 정리
    public float walkSpeed = 10f;
    public float creepSpeed = 5f;
    public float gravity = 20f;
    public float jumpSpeed = 15f;
    public float doubleJumpSpeed = 10f;
    public float tripleJumpSpeed = 10f;
    public float xWallJumpSpeed = 15f;
    public float yWallJumpSpeed = 15f;
    public float wallRunSpeed = 8f;
    public float wallSlideAmout = 0.1f;
    public float dashSpeed = 40f;
    public float dashTime = 0.2f;

    //Player Abilities
    [Header("Player Abilities")]
    public bool canDoubleJump;
    public bool canTripleJump;
    public bool canWallJump;
    public bool canWallRun;
    public bool canWallSlide;
    public bool canAirDash;
    public bool canGroundDesh;

    //Player states
    [Header("Player States")]
    public bool isJumping;
    public bool isDoubleJumping;
    public bool isTripleJumping;
    public bool isWallJumping;
    public bool isWallRunning;
    public bool isWallSliding;
    public bool isDucking;
    public bool isCreeping;
    public bool isDashing;

    //input flags
    bool _startJump;
    bool _releaseJump;

    private Vector2 _input;
    private Vector2 _moveDirection;
    private characterController2D _characterController;

    private CapsuleCollider2D _capsuleCollider;
    private SpriteRenderer _spriteRenderer;
    private Vector2 _originalColliderSize;

    private bool _ableToWallRun;

    void Start()
    {
        _characterController = GetComponent<characterController2D>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        _originalColliderSize = _capsuleCollider.size;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!isWallJumping)
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
        }

        if (_characterController.below) // on the ground
        {
            _moveDirection.y = 0;

            isJumping = false;
            isDoubleJumping = false;
            isTripleJumping = false;
            isWallJumping = false;
            isWallRunning = false;

            if (_startJump == true)
            {
                _startJump = false;
                isJumping = true;
                _moveDirection.y = jumpSpeed;
                _ableToWallRun = true;
                _characterController.DisableGroundCheck();
            }

            if (_input.y < 0f)
            {
                if (!isDucking && !isCreeping)
                {
                    isDucking = true;
                    _capsuleCollider.size = new Vector2(_capsuleCollider.size.x, _capsuleCollider.size.y / 2);
                    _spriteRenderer.sprite = Resources.Load<Sprite>("directionSpriteUp_crouching");
                    transform.position = new Vector2(transform.position.x, transform.position.y - (_originalColliderSize.y / 4));
                }
            }
            else
            {
                if (isDucking || isCreeping)
                {
                    RaycastHit2D hitCeiling = Physics2D.CapsuleCast(_capsuleCollider.bounds.center, transform.localScale, 
                        CapsuleDirection2D.Vertical, 0f, Vector2.up, _originalColliderSize.y / 2, _characterController.layerMask);

                    if (!hitCeiling.collider)
                    {
                        _capsuleCollider.size = _originalColliderSize;
                        _spriteRenderer.sprite = Resources.Load<Sprite>("directionSpriteUp");
                        transform.position = new Vector2(transform.position.x, transform.position.y + (_originalColliderSize.y / 4));
                        isCreeping = false;
                        isDucking = false;
                    }
                }
            }

            if (isDucking && _input.x != 0)
            {
                isCreeping = true;
            }
            else
            {
                isCreeping = false;
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

                //isWallRunning = false;
            }

            //pressed jump button in the air
            if (_startJump )
            {
                //triple jump
                if (canTripleJump && !_characterController.right && !_characterController.left)
                {
                    if (isDoubleJumping && !isTripleJumping)
                    {
                        Debug.Log("3번점프");
                        _moveDirection.y = tripleJumpSpeed;
                        isTripleJumping = true;
                    }
                }

                //double jump
                if (canDoubleJump && !_characterController.right && !_characterController.left)
                {
                    if (!isDoubleJumping)
                    {
                        Debug.Log("2번점프");
                        _moveDirection.y = doubleJumpSpeed;
                        isDoubleJumping = true;
                    }
                }

                //Wall jump
                if (canWallJump && (_characterController.right || _characterController.left))   //엔드 오얼이 같이 있으면 오알부터 계산됨.
                {
                    if (_characterController.right)
                    {
                        Debug.Log("오른쪽 벽 점프");
                        _moveDirection.x = -xWallJumpSpeed;
                        _moveDirection.y = xWallJumpSpeed;
                        transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                    }
                    else if(_characterController.left)
                    {
                        Debug.Log("왼쪽 벽 점프");
                        _moveDirection.x = xWallJumpSpeed;
                        _moveDirection.y = xWallJumpSpeed;
                        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                    }

                    //isWallJumping = true;
                    StartCoroutine("WallJumpWaiter");
                }
                _startJump = false;
            }

            //Running Wall
            if (canWallRun && (_characterController.left || _characterController.right))
            {
                if (_input.y > 0f && _ableToWallRun)
                {
                    Debug.Log("점프했담");  //벽 점프 안되는데...
                    _moveDirection.y = wallRunSpeed;
                }

                StartCoroutine("WallRunWaiter");
            }

            GravityCalculation();
        }

        _characterController.Move(_moveDirection * Time.deltaTime);
    }

    private void GravityCalculation()
    {
        if (_moveDirection.y > 0f && _characterController.above)
        {
            _moveDirection.y = 0f;
        }

        if (canWallSlide && (_characterController.left || _characterController.right))
        {
            if (_characterController.hitWallThisFrame)
            {
                _moveDirection.y = 0f;
            }

            if (_moveDirection.y <= 0)
            {
                _moveDirection.y -= gravity * wallSlideAmout * Time.deltaTime;
            }
            else
            {
                _moveDirection.y -= gravity * Time.deltaTime;
            }
        }
        else
        {
            _moveDirection.y -= gravity * Time.deltaTime;
        }
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

            Debug.Log("점프시작");
        }
        else if (context.canceled)
        {
            _startJump = false;
            _releaseJump = true;

            Debug.Log("점프끝");
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {

    }

    IEnumerator WallJumpWaiter()
    {
        isWallJumping = true;
        yield return new WaitForSeconds(0.4f);
        isWallJumping = false;
    }

    IEnumerator WallRunWaiter()
    {
        isWallRunning = true;
        yield return new WaitForSeconds(0.5f);
        isWallRunning = false;
        _ableToWallRun = false; //이거 안되는데 이게 뭐지
    }



}
