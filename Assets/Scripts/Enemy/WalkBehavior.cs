using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(EnemyHealth))]
public class WalkBehavior : MonoBehaviour
{
    private Rigidbody2D rb;
    private CapsuleCollider2D bc;
    [SerializeField]
    private LayerMask _RoomLayer;
    [SerializeField]
    private LayerMask _GroundLayer;
    [SerializeField]
    private LayerMask _EnemyLayer;
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

    EnemyHealth _eh;
    private Room RoomAssigned { get {
            if (_eh)
            {
                return _eh.RoomAssigned;
            }
            return null; 
        } }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<CapsuleCollider2D>();
        _eh = GetComponent<EnemyHealth>();
        _eh.OnActiveChange += OnActiveChange;
        _facingRight = _StartFacingLeft;
        rb.gravityScale = 9.81f;
    }

    private void OnActiveChange(bool setActive)
    {
        if (setActive)
        {

        }
        else
        {
            rb.velocity = Vector2.zero;   
        }
        bc.enabled = setActive;
    }

    private void Update()
    {
        if (_eh.Active)
        {
            IsGrounded();   // Sets foot checks
            bool FootCheck = !LeftFootCheck ^ !RightFootCheck;// ^ is exclusive OR, or XOR | Is only 1 foot off the platform?
            bool Wall = CheckForBlocking(_EnemyLayer | _GroundLayer); // Is there a wall in front of us?
            bool Room = !CheckIsInRoom(); // Are we in the room?
            if (_MovementDebug) { Debug.Log("Movement Check flags: " + FootCheck + ", " + Wall + ", " + Room); }
            if ( FootCheck || Wall || Room) 
            {
                Flip();
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_eh.Active)
        {
            Vector2 movement = new Vector2(_MoveSpeed, rb.velocity.y);
            if (!_facingRight) { movement.x = -movement.x; }
            rb.velocity = movement;
        }
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
    
    bool CheckForBlocking(LayerMask Layers)
    {
        float rayDistance = 0.2f;
        float height = bc.bounds.size.y;
        float side = bc.bounds.extents.x+0.1f;
        Vector3 pos = bc.bounds.center;
        Vector2 HeadRight = new Vector2(pos.x + side, pos.y +height*0.3f);
        Vector2 HeadLeft = new Vector2(pos.x-side, pos.y + height * 0.3f);
        Vector2 BodyRight = new Vector2(pos.x + side, pos.y);
        Vector2 BodyLeft = new Vector2(pos.x-side, pos.y);
        Vector2 FeetLeft = new Vector2(pos.x - side, pos.y - height * 0.3f);
        Vector2 FeetRight = new Vector2(pos.x + side, pos.y - height * 0.3f);
        RaycastHit2D LeftHeadCheck = Physics2D.Raycast(HeadLeft, -Vector2.right, rayDistance, Layers);
        RaycastHit2D RightHeadCheck = Physics2D.Raycast(HeadRight, Vector2.right, rayDistance, Layers);
        RaycastHit2D LeftBodyCheck = Physics2D.Raycast(BodyLeft, -Vector2.right, rayDistance, Layers);
        RaycastHit2D RightBodyCheck = Physics2D.Raycast(BodyRight, Vector2.right, rayDistance, Layers);
        RaycastHit2D LeftFootCheck = Physics2D.Raycast(FeetLeft, -Vector2.right, rayDistance, Layers);
        RaycastHit2D RightFootCheck = Physics2D.Raycast(FeetRight, Vector2.right, rayDistance, Layers);

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
            Debug.Log("Block" + (bool)LeftHeadCheck +", "+ (bool)RightHeadCheck + ", " + (bool)LeftBodyCheck + ", " + (bool)RightBodyCheck + ", " + (bool)LeftFootCheck + ", " + (bool)RightFootCheck);
        }
        
        return LeftHeadCheck || RightHeadCheck || LeftBodyCheck || RightBodyCheck || LeftFootCheck || RightFootCheck;
    }

    bool CheckIsInRoom()
    {
        //Debug.Log("Assigned Room: " + RoomAssigned);
        bool ret = false;
        float rayDistance = 0.2f;
        float height = bc.bounds.size.y;
        float side = bc.bounds.extents.x;
        Vector3 pos = bc.bounds.center;
        Vector2 BodyRight = new Vector2(pos.x + side, pos.y);
        Vector2 BodyLeft = new Vector2(pos.x - side, pos.y);
        RaycastHit2D LeftBodyCheck = Physics2D.Raycast(BodyLeft, -Vector2.right, rayDistance, _RoomLayer);
        RaycastHit2D RightBodyCheck = Physics2D.Raycast(BodyRight, Vector2.right, rayDistance, _RoomLayer);
        if (LeftBodyCheck && RightBodyCheck && RoomAssigned)
        {
            Room LeftRoom = LeftBodyCheck.collider.gameObject.GetComponent<Room>();
            Room RightRoom = RightBodyCheck.collider.gameObject.GetComponent<Room>();
            if (LeftRoom && RightRoom)
            {
                if (RoomAssigned)
                {
                    if (LeftRoom == RoomAssigned && RightRoom == RoomAssigned)
                    {
                        ret = true;
                    }
                }
                else
                {
                    ret = true;
                }
            }
        }

        if (_MovementDebug)
        {
            Color leftBodyColor = LeftBodyCheck ? Color.cyan : Color.white;
            Color rightBodyColor = RightBodyCheck ? Color.cyan : Color.white;
            Debug.DrawRay(BodyLeft+Vector2.up/4, -Vector2.right * rayDistance, leftBodyColor);
            Debug.DrawRay(BodyRight+ Vector2.up/4, Vector2.right * rayDistance, rightBodyColor);
        }

        return ret;
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
