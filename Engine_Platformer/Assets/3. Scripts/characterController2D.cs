using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterController2D : MonoBehaviour
{
    public float raycastDistance = 0.2f;
    public LayerMask layerMask;

    public bool below;

    private Vector2 _moveAmount;
    private Vector2 _currentPosition;
    private Vector2 _lastPosition;

    private Rigidbody2D _rigidbody;
    private CapsuleCollider2D _capsuleCollider;

    Vector2[] _raycastPosition = new Vector2[3];
    RaycastHit2D[] _raycastHits = new RaycastHit2D[3];

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
        _currentPosition = _lastPosition + _moveAmount;

        _rigidbody.MovePosition(_currentPosition);

        _moveAmount = Vector2.zero;

        CheckGrounded();
    }

    public void Move(Vector2 movement)
    {
        _moveAmount += movement;
    }

    public void CheckGrounded()
    {
        Vector2 raycastOrgin = _rigidbody.position - new Vector2(0, _capsuleCollider.size.y * .5f);

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
            }
        }

        if (numberofGroundHits > 0)
        {
            below = true;
        }
        else
        {
            below = false;
        }
    }

    private void DrawDebugRays(Vector3 direction, Color color)
    {
        for (int i = 0; i < _raycastPosition.Length; i++)
        {
            Debug.DrawRay(_raycastPosition[i], direction * raycastDistance, color);
        }
    }



}
