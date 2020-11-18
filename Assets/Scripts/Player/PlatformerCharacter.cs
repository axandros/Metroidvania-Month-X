﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
public class PlatformerCharacter : MonoBehaviour
{
    // --- Move Parameters ---
    [Header("Movement")]
    [SerializeField, Tooltip("Lateral movement speed.")]
    private float _MoveSpeed = 3.0f;

    // --- Jump Parameters ---
    [Header("Jump")]
    [SerializeField, Tooltip("How high in Unity Units the character should jump.")]
    private float _JumpHeight = 2.0f;
    [SerializeField, Tooltip("The time for the character to reach the peak of their jump.")]
    private float _TimeToPeak = 0.5f;
    [SerializeField, Tooltip("The layer that the character should be able to land on.")]
    private LayerMask _GroundLayer;
    [SerializeField]
    private float _GravityModifierJumpReleased = 1.25f;
    [SerializeField, Tooltip("How much more quickly should the character fall from their jump?")]
    private float _GravityModifierFalling = 2.0f;

    // --- Knockback Parameters ---
    [Header("Knockback")]
    [SerializeField, Tooltip("How high should the knockback send the player?")]
    private float _KnockbackHeight = 1.0f;
    [SerializeField, Tooltip("How far back should the character be knocked back on level ground?")]
    private float _KnockbackDistance = 2.0f;
    [SerializeField, Tooltip("How long should it take before the character comes back on after the knockback.   ")]
    private float _KnockbackTimetoPeak = 0.5f;

    // --- Climbing Parameters ---
    [Header("Climbing")]
    [SerializeField]
    private float _ClimbSpeed = 2;
    [SerializeField]
    private LayerMask _LadderLayer;

    // --- Debug Options ---
    [Header("Debugging")]
    [SerializeField, Tooltip("Draw the raytraces used to determine if the character is grounded.")]
    private bool _DebugGroundCheck = false;

    // --- Private Components ---
    private Rigidbody2D _rb;
    private BoxCollider2D _bc;
    private Animator _anim;

    // --- Derived cache ---
    private float _jumpGravityScale;
    private float _jumpUpVelocity;
    private Vector2 _knockbackInitialVelocity;
    private float _knockbackInitialGravity;

    // --- State Variables ---

    private float _lastJumpTime = 0;
    private float _lastKnockbackTime = 0;
    private float _lateralKnockbackVelocity = 0;
    private bool _grounded = false;
    private bool _onLadder = false;

    // --- Input Variables ---
    private float _movementInput = 0;
    private float _verticalInput = 0;   
    private bool _facingRight = true;
    private bool _hasJumped = false;
    private bool _jumpPressed = false;
    private bool _jumpHeld = false;
    


    // ===== FUNCTIONS ====
    // --- Queries ---
    bool IsGrounded()
    {
        bool ret = false;
        float footOffset = _bc.bounds.extents.x * 0.95f;
        float feetHeight = _bc.bounds.extents.y * 1.1f;
        //Debug.Log("Feet Height: " + feetHeight);
        float rayDistance = 0.2f;
        Vector3 pos = transform.position;
        Vector2 leftPosition = new Vector2(pos.x - footOffset, pos.y - feetHeight);
        Vector2 rightPosition = new Vector2(pos.x + footOffset, pos.y - feetHeight);
        RaycastHit2D leftCheck = Physics2D.Raycast(leftPosition, -Vector2.up, rayDistance, _GroundLayer);
        RaycastHit2D rightCheck = Physics2D.Raycast(rightPosition, -Vector2.up, rayDistance, _GroundLayer);

        if (_DebugGroundCheck)
        {
            Color leftColor = leftCheck ? Color.red : Color.green;
            Color rightColor = rightCheck ? Color.red : Color.green;
            //...and draw the ray in the scene view
            Debug.DrawRay(leftPosition, -Vector2.up * rayDistance, leftColor);
            Debug.DrawRay(rightPosition, -Vector2.up * rayDistance, rightColor);
        }

        if (leftCheck || rightCheck) { ret = true; }

        return ret;
    }
    private void Flip()
    {
        _facingRight = !_facingRight;
        transform.Rotate(new Vector3(0, 180, 9));
    }
    private void Jump()
    {
        _rb.velocity = new Vector2(_rb.velocity.x, _jumpUpVelocity);

        //_anim.SetFloat("IsInAir", Mathf.Abs(_movementInput));
    }
    public void Knockback(Transform damageSource)
    {
        Knockback(this.transform.position.x - damageSource.position.x > 0);
    }
    public void Knockback(bool KnockbackDirectionLeft = true)
    {
        _lastKnockbackTime = Time.time;
        int direction = 1;
        if (!KnockbackDirectionLeft) { direction = -1; }
        _rb.gravityScale = _knockbackInitialGravity;
        _lateralKnockbackVelocity = _knockbackInitialVelocity.x * direction;
        _rb.velocity = new Vector2(_lateralKnockbackVelocity, _knockbackInitialVelocity.y);
    }
    private void Climb()
    {
        // Look for Ladder
        Vector2 rayStartPosition = _bc.bounds.center;
        float rayDistance = 0.2f;
        RaycastHit2D LadderCheck = Physics2D.Raycast(rayStartPosition, Vector2.up, rayDistance, _LadderLayer);
        Color col = LadderCheck?Color.red:Color.blue;
        Debug.DrawRay(rayStartPosition, Vector2.up *rayDistance, Color.blue); 
        //Debug.Log("Looking for Ladder: ");
        if (LadderCheck)
        {
            Debug.Log("Ladder Found");
            // We found a ladder
            _onLadder = true;
            _rb.gravityScale = 0;
        } else
        {
            Debug.Log("Ladder Lost");
            _onLadder = false;
            _rb.gravityScale = _GravityModifierFalling;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // -- Get Components --
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _bc = GetComponent<BoxCollider2D>();

        // --- Cache Values ---
        _jumpGravityScale = CalculateGravity(_JumpHeight, _TimeToPeak);
        _jumpUpVelocity = CalculateVerticalVelocity(_JumpHeight, _TimeToPeak);
        float vx = CalculateLateralVelocity(_KnockbackDistance / 2, _KnockbackTimetoPeak);
        float vy = CalculateVerticalVelocity(_KnockbackHeight, vx, _KnockbackDistance / 2);
        _knockbackInitialVelocity = new Vector2(vx, vy);
        _knockbackInitialGravity = CalculateGravity(_KnockbackHeight, vx, _KnockbackDistance / 2);
    }


    // Update is called once per frame
    void Update()
    {
        // Normal code
        ClearAndUpdateInputs();

        if (Time.time - _lastKnockbackTime < _KnockbackTimetoPeak * 2)
        {
            UpdateKnockback();
        }else
        if (_hasJumped)
        {
            UpdateJump();
        }
        else if (_verticalInput != 0 || _onLadder)
        {
            Climb();
        }
        else if (_jumpPressed && _grounded)
        {
            Jump();
            _jumpHeld = true;
            _hasJumped = true;
            _lastJumpTime = 0;
        }
    }

    void ClearAndUpdateInputs()
    {
        _jumpPressed = false;
        _movementInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
        _jumpPressed = Input.GetButtonDown("Jump");
        _grounded = IsGrounded();
    }

    void UpdateJump()
    {
        _lastJumpTime += Time.deltaTime;
        if (_lastJumpTime >= 0.25)
        {
            if (_rb.velocity.y < 0) // Falling
            {
                _rb.gravityScale = _jumpGravityScale * _GravityModifierFalling;
            }
            else if (_grounded) // Landed
            {
                _rb.gravityScale = _jumpGravityScale;
                _hasJumped = false;
            }
            else if (_jumpHeld && !Input.GetButton("Jump")) // Let go of jump button.
            {
                _rb.gravityScale = _jumpGravityScale * _GravityModifierJumpReleased;
            }

        }
    }

    void UpdateKnockback()
    {
        _rb.velocity = new Vector2(_lateralKnockbackVelocity, _rb.velocity.y);
    }

    // Physics update interactions
    private void FixedUpdate()
    {
        if(_onLadder)
        {
            _rb.velocity = new Vector2(_MoveSpeed/2*_movementInput,_verticalInput * _ClimbSpeed);
            _anim.SetFloat("HorizontalSpeed", Mathf.Abs(_movementInput));
            //_anim.SetFloat("ClimbingSpeed", Mathf.Abs(_verticalInput));
        }
        else if (_grounded)
        {
            _rb.velocity = new Vector2(_movementInput * _MoveSpeed, _rb.velocity.y);
            _anim.SetFloat("HorizontalSpeed", Mathf.Abs(_movementInput));
            if (_movementInput > 0 && !_facingRight) { Flip(); }
            else if (_movementInput < 0 && _facingRight) { Flip(); }
        }

    }

    // Calculations
    /// <summary>
    /// Calculates the gravity scale needed to create an arc using the given parameters.  
    /// Assumes a Unity gravity of -1.
    /// </summary>
    /// <param name="height">The height of the peak of the arc.</param>
    /// <param name="initialLateralVelocity">The speed along the x axis the object has.</param>
    /// <param name="lateralHeightToPeak">The distance along the x axis from the start of the arc to its peak.</param>
    /// <returns></returns>
    private float CalculateGravity(float height, float initialLateralVelocity, float lateralHeightToPeak)
    {
        return (2 * height * initialLateralVelocity * initialLateralVelocity) / (lateralHeightToPeak * lateralHeightToPeak);
    }
    /// <summary>
    /// Calculates the gravity scale needed to create an arc using the given parameters.  
    /// Assumes a Unity gravity of -1.
    /// </summary>
    /// <param name="height">The height of the peak of the arc.</param>
    /// <param name="timeToPeak">The time that passes from the start of the arc until the object reaches the peak.</param>
    private float CalculateGravity(float height, float timeToPeak)
    {
        return 2 * height / (timeToPeak * timeToPeak);
    }
    /// <summary>
    /// Calculates the vertical velocity for an object's projectile motion given the input parameters.
    /// </summary>
    /// <param name="height">The height of the peak of the arc.</param>
    /// <param name="timeToPeak">The time that passes from the start of the arc until the object reaches the peak.</param>
    private float CalculateVerticalVelocity(float height, float timeToPeak)
    {
        return 2 * height / timeToPeak;
    }
    /// <summary>
    /// Calculates the vertical celocity for an object's projectile motion given the input parameters.
    /// </summary>
    /// <param name="height">The height of the peak of the arc.</param>
    /// <param name="initialLateralVelocity">The speed along the x axis the object has.</param>
    /// <param name="lateralHeightToPeak">The distance along the x axis from the start of the arc to its peak.</param>
    private float CalculateVerticalVelocity(float height, float initialLateralVelocity, float lateralHeightToPeak)
    {
        return (2 * height * initialLateralVelocity) / lateralHeightToPeak;
    }
    /// <summary>
    /// Calculate the x velocity to cover a specified distance.
    /// </summary>
    /// <param name="lateralDistanceToPeak">The distance to the peak of the arc. Half the distacne of the full parabola.</param>
    /// <param name="timeToPeak">The amount of time, in seconds, that need to pass between the start of the arc and the peak.</param>
    /// <returns></returns>
    private float CalculateLateralVelocity(float lateralDistanceToPeak, float timeToPeak)
    {
        return lateralDistanceToPeak / timeToPeak;
    }
}