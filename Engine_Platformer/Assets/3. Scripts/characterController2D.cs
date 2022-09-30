using GlobalType;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterController2D : MonoBehaviour
{
    public float raycastDistance = 0.2f;
    public LayerMask layerMask;
    public float slopeAngleLimit = 45f;

    public bool below;
    public bool above;
    public bool left;
    public bool right;
    public GroundType groundType;

    private Vector2 _slopeNormal;
    private float _slopArgle;

    private Vector2 _moveAmount;
    private Vector2 _currentPosition;
    private Vector2 _lastPosition;

    private Rigidbody2D _rigidbody;
    private CapsuleCollider2D _capsuleCollider;

    Vector2[] _raycastPosition = new Vector2[3];
    RaycastHit2D[] _raycastHits = new RaycastHit2D[3];

    private bool _disableGroundCheck;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _lastPosition = _rigidbody.position;
        if (_slopArgle != 0 && below)
        {
            if ((_slopArgle > 0f && _moveAmount.x > 0f) || (_slopArgle < 0f && _moveAmount.x < 0f)) // 슬로프 내려갈 때
            {
                _moveAmount.y = -Mathf.Abs(Mathf.Tan(_slopArgle * Mathf.Deg2Rad) * _moveAmount.x);
            }

        }
        _currentPosition = _lastPosition + _moveAmount;

        _rigidbody.MovePosition(_currentPosition);

        _moveAmount = Vector2.zero;

        if (!_disableGroundCheck)
        {
            CheckGrounded();
        }

        CheckOtherCollision();
    }

    public void Move(Vector2 movement)
    {
        _moveAmount += movement;
    }

    private void CheckOtherCollision()
    {
        //left check
        RaycastHit2D leftHit = Physics2D.BoxCast(_capsuleCollider.bounds.center, _capsuleCollider.size * 0.7f,
            0f, Vector2.left, raycastDistance, layerMask);

        if (leftHit.collider)
        {
            left = true;
        }
        else
        {
            left = false;
        }

        //right check
        RaycastHit2D rightHit = Physics2D.BoxCast(_capsuleCollider.bounds.center, _capsuleCollider.size * 0.7f,
            0f, Vector2.right, raycastDistance, layerMask);

        if (rightHit.collider)
        {
            right = true;
        }
        else
        {
            right = false;
        }

        //above check 위쪽
        RaycastHit2D aboveHit = Physics2D.CapsuleCast(_capsuleCollider.bounds.center, _capsuleCollider.size, CapsuleDirection2D.Vertical,
            0f, Vector2.up, raycastDistance, layerMask);

        if (aboveHit.collider)
        {
            above = true;
        }
        else
        {
            above = false;
        }
    }

    public void CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.CapsuleCast(_capsuleCollider.bounds.center, _capsuleCollider.size, CapsuleDirection2D.Vertical,
            0f, Vector2.down, raycastDistance, layerMask);

        if (hit.collider)
        {
            _slopeNormal = hit.normal;
            _slopArgle = Vector2.SignedAngle(_slopeNormal, Vector2.up);

            groundType = DetermineGroundType(hit.collider);

            if (_slopArgle > slopeAngleLimit || _slopArgle < -slopeAngleLimit)
            {
                below = false;
            }
            else
            {
                below = true;
            }
        }
        else
        {
            groundType = GroundType.none;
            below = false;
        }

        #region 전 코드
        /*Vector2 raycastOrgin = _rigidbody.position - new Vector2(0, _capsuleCollider.size.y * .5f);

        _raycastPosition[0] = raycastOrgin + (Vector2.left * _capsuleCollider.size.x * .25f + Vector2.up * .1f);
        _raycastPosition[1] = raycastOrgin;
        _raycastPosition[2] = raycastOrgin + (Vector2.right * _capsuleCollider.size.x * .25f + Vector2.up * .1f);

        DrawDebugRays(Vector2.down, Color.green);

        int numberofGroundHits = 0;

        for (int i = 0; i < _raycastPosition.Length; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(_raycastPosition[i], Vector2.down, raycastDistance, layerMask);

            if (hit.collider)
            {
                _raycastHits[i] = hit;
                numberofGroundHits++;
                _slopeNormal = hit.normal;
                _slopArgle = Vector2.SignedAngle(_slopeNormal, Vector2.up);
                // groundType = DetermineGroundType(hit.collider);
            }
        }

        if (numberofGroundHits > 0)
        {
            
            if (_raycastHits[1].collider)
            {
                groundType = DetermineGroundType(_raycastHits[1].collider);
            }

            if (_slopArgle > slopeAngleLimit || _slopArgle < -slopeAngleLimit)
            {
                below = false;
            }
            else
            {
                below = true;
            }
        }
        else
        {
            below = false;
            groundType = GroundType.none;
        }*/
        #endregion
    }

    private GroundType DetermineGroundType(Collider2D collider)
    {
        GroundEffector groundEffector = collider.GetComponent<GroundEffector>();

        if (groundEffector != null) return groundEffector.groundType;
        else return GroundType.LevelGeometry;
    }

    private void DrawDebugRays(Vector3 direction, Color color)
    {
        for (int i = 0; i < _raycastPosition.Length; i++)
        {
            Debug.DrawRay(_raycastPosition[i], direction * raycastDistance, color);
        }
    }

    public void DisableGroundCheck()
    {
        below = false;
        _disableGroundCheck = true;
        StartCoroutine("EnableGroundCheck");
    }

    IEnumerator EnableGroundCheck()
    {
        yield return new WaitForSeconds(.1f);
        _disableGroundCheck = false;
    }



}
