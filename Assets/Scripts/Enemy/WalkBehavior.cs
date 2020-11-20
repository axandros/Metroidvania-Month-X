using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class WalkBehavior : MonoBehaviour
{
    private Rigidbody2D rb;
    private CapsuleCollider2D bc;

    [SerializeField]
    private LayerMask _GroundLayer;
    [SerializeField]
    private bool _MovementDebug = false;
    
    [SerializeField]
    private float _MoveSpeed = 1.0f;
    [SerializeField]
    private bool _StartFacingLeft = true;

    RaycastHit2D LeftFootCheck;
    RaycastHit2D RightFootCheck;

    private bool _facingRight = false;

    private float _lastFlipTime;
    private float _flipCooldown = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<CapsuleCollider2D>();
        _facingRight = _StartFacingLeft;
    }

    private void Update()
    {
        IsGrounded();
        if (!LeftFootCheck ^ !RightFootCheck || CheckforWalls()) // ^ is exclusive OR, or XOR
        {
            Flip();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        Vector2 movement = new Vector2(_MoveSpeed , rb.velocity.y);
        if (!_facingRight) { movement.x = -movement.x; }
        rb.velocity = movement;
    }

    bool IsGrounded()
    {
        bool ret = false;
        float footOffset = bc.bounds.extents.x * 0.95f;
        float feetHeight = bc.bounds.extents.y * 1.1f;
        //Debug.Log("Feet Height: " + feetHeight);
        float rayDistance = 0.2f;
        Vector3 pos = transform.position;
        Vector2 leftPosition = new Vector2(pos.x - footOffset, pos.y - feetHeight);
        Vector2 rightPosition = new Vector2(pos.x + footOffset, pos.y - feetHeight);
        LeftFootCheck = Physics2D.Raycast(leftPosition, -Vector2.up, rayDistance, _GroundLayer);
        RightFootCheck = Physics2D.Raycast(rightPosition, -Vector2.up, rayDistance, _GroundLayer);

        if (_MovementDebug)
        {
            //Debug.Log("Running Ground Debug");  
            Color leftColor = LeftFootCheck ? Color.red : Color.green;
            Color rightColor = RightFootCheck ? Color.red : Color.green;
            //...and draw the ray in the scene view
            Debug.DrawRay(leftPosition, -Vector2.up * rayDistance, leftColor);
            Debug.DrawRay(rightPosition, -Vector2.up * rayDistance, rightColor);
        }

        if (LeftFootCheck || RightFootCheck) { ret = true; }

        return ret;
    }
    
    bool CheckforWalls()
    {
        float rayDistance = 0.2f;
        float height = bc.bounds.size.y;
        float side = bc.bounds.extents.x;
        Vector3 pos = bc.bounds.center;
        Vector2 HeadRight = new Vector2(pos.x + side, pos.y +height*0.3f);
        Vector2 HeadLeft = new Vector2(pos.x-side, pos.y + height * 0.3f);
        Vector2 BodyRight = new Vector2(pos.x + side, pos.y);
        Vector2 BodyLeft = new Vector2(pos.x-side, pos.y);
        Vector2 FeetLeft = new Vector2(pos.x - side, pos.y - height * 0.3f);
        Vector2 FeetRight = new Vector2(pos.x + side, pos.y - height * 0.3f);
        RaycastHit2D LeftHeadCheck = Physics2D.Raycast(HeadLeft, -Vector2.right, rayDistance, _GroundLayer);
        RaycastHit2D RightHeadCheck = Physics2D.Raycast(HeadRight, Vector2.right, rayDistance, _GroundLayer);
        RaycastHit2D LeftBodyCheck = Physics2D.Raycast(BodyLeft, -Vector2.right, rayDistance, _GroundLayer);
        RaycastHit2D RightBodyCheck = Physics2D.Raycast(BodyRight, -Vector2.right, rayDistance, _GroundLayer);
        RaycastHit2D LeftFootCheck = Physics2D.Raycast(FeetLeft, -Vector2.right, rayDistance, _GroundLayer);
        RaycastHit2D RightFootCheck = Physics2D.Raycast(FeetRight, -Vector2.right, rayDistance, _GroundLayer);

        if (_MovementDebug)
        {
            Color leftHeadColor = LeftHeadCheck ? Color.red : Color.green;
            Color rightHeadColor = RightHeadCheck ? Color.red : Color.green;
            Color leftBodyColor = LeftBodyCheck ? Color.red : Color.green;
            Color rightBodyColor = RightBodyCheck ? Color.red : Color.green;
            Color leftColor = LeftFootCheck ? Color.red : Color.green;
            Color rightColor = RightFootCheck ? Color.red : Color.green;
            Debug.DrawRay(HeadLeft, -Vector2.right * rayDistance, leftHeadColor);
            Debug.DrawRay(HeadRight, Vector2.right * rayDistance, rightHeadColor);
            Debug.DrawRay(BodyLeft, -Vector2.right * rayDistance, leftBodyColor);
            Debug.DrawRay(BodyRight, Vector2.right * rayDistance, rightBodyColor);
            Debug.DrawRay(FeetLeft, -Vector2.right * rayDistance, leftColor);
            Debug.DrawRay(FeetRight, Vector2.right * rayDistance, rightColor);
        }

        return LeftHeadCheck || RightHeadCheck || LeftBodyCheck || RightBodyCheck || LeftFootCheck || RightFootCheck;
    }
    private void Flip()
    {
        if ((Time.time - _lastFlipTime) > _flipCooldown)
        {
            _lastFlipTime = Time.time;
            _facingRight = !_facingRight;
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }
}
