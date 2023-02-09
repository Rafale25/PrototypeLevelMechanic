using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerCC : MonoBehaviour
{

    // TODO: add different gravity when player is falling
    // TODO: use rigibody instead of characterController for improved wall velocity stopping and collision detection

    [SerializeField] Camera _playerCamera;
    CharacterController _controller;
    public Vector3 _velocity = Vector3.zero;

    float walkSpeed = 10f;
    float sprintSpeed = 15f;
    float airSpeedLimit = 20f;

    float groundDrag = 0.6f; // only applied when player is not moving
    float groundAcceleration = 150f;
    float airAcceleration = 25f;

    const float dashCoolDown = 3.0f;
    const float dashDuration = 0.175f;

    bool _isFalling = false;
    bool _isSprinting = false;
    bool _canWallJump = true;
    bool _isPressingMovement = false; // if player is holding/pressing key

    bool _isAgainstWall = false;
    Vector3 _wallNormal;

    float jumpForce = 20f;
    float antiBump = 4.5f;
    float gravityNormal = 40f;
    float gravityHold = 30f;

    float defaultFOV;
    float sprintFOV;

    // grapplin
    public bool _isGrapplin = false;
    public Vector3 _grapplinPoint;
    public float _grapplinLength;

    void OnDrawGizmos()
    {
        if (_isGrapplin) {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(_playerCamera.transform.position + new Vector3(0f, -1f, 0f), _grapplinPoint);
        }
    }


    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        defaultFOV = _playerCamera.fieldOfView;
        sprintFOV = _playerCamera.fieldOfView * 1.1f;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_isGrapplin)
            {
                _isGrapplin = false;
            }
            else 
            {
                RaycastHit hit;
                if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out hit, 500f))
                {
                    _isGrapplin = true;
                    _grapplinPoint = hit.point;
                    _grapplinLength = hit.distance;
                }
            }

        }

        if (_isGrapplin) {
            _grapplinLength += Input.mouseScrollDelta.y;
        }


        if (_isSprinting && _isPressingMovement) {
            _playerCamera.fieldOfView = Mathf.Lerp(_playerCamera.fieldOfView, sprintFOV, Easing.easeOutCubic(5f * Time.deltaTime)); // easing actually useless
        } else {
            _playerCamera.fieldOfView = Mathf.Lerp(_playerCamera.fieldOfView, defaultFOV, Easing.easeOutCubic(5f * Time.deltaTime));
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.DrawRay(hit.point, hit.normal, Color.red, 10f);
        
        _wallNormal = hit.normal;

        if (!_controller.isGrounded) {
            _isAgainstWall = true;
        } else {
            _isAgainstWall = false;
        }
    }

    void FixedUpdate()
    {
        DefaultMovement();

        Vector3 worldVelocity = _velocity;

        // if not pressing space, increase velocity when falling
        if (_velocity.y < 0f && !Input.GetKey(KeyCode.Space))
            worldVelocity.y *= 1.4f;

        _controller.Move(worldVelocity * Time.fixedDeltaTime);
    }

    // IEnumerator TriggerDash()
    // {
    //     _isDashing = true;
    //     _canDash = false;

    //     float defaultFOV = _playerCamera.fieldOfView;
    //     _playerCamera.fieldOfView *= 1.05f;

    //     yield return new WaitForSeconds(dashDuration);

    //     _playerCamera.fieldOfView = defaultFOV;
    //     _isDashing = false;

    //     yield return new WaitForSeconds(dashCoolDown);

    //     _canDash = true;
    // }

    void DefaultMovement()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        Vector3 forward = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z);
        Vector3 right = new Vector3(_playerCamera.transform.right.x, 0f, _playerCamera.transform.right.z);

        _isPressingMovement = input.magnitude > 0.1f;
        _isFalling = _velocity.y < 0f && !_controller.isGrounded;
        if (_controller.isGrounded) {
            _canWallJump = true;
        }
        if (Input.GetKey(KeyCode.LeftShift)) {
            _isSprinting = true;
        } else {
            _isSprinting = false;
        }

        // wall jump
        if (!_controller.isGrounded && Input.GetKeyDown(KeyCode.Space)) {
            if (_isAgainstWall && _canWallJump) {

                // check if wallNormal is horizontal enough to jump on
                float slopeAngle = Vector3.Angle(_wallNormal, Vector3.up);
                if (slopeAngle <= 90.001f && slopeAngle > 60f) {

                    Vector3 wallDirection = new Vector3(_wallNormal.x, 0f, _wallNormal.z);
                    Vector3 jumpDirection = (wallDirection + Vector3.up * 1.5f).normalized; // to get a 45 degree angle jump regardless of the surface normal
                    
                    _velocity -= _wallNormal * Vector3.Dot(_velocity, _wallNormal); // clamp velocity to normal's plane 
                    _velocity += jumpDirection * 30f;

                    _canWallJump = false;
                    _isAgainstWall = false;

                    Debug.DrawRay(transform.position, jumpDirection*40f, Color.green, 10f);
                }
            }
        }

        if (_controller.isGrounded) {

            if (!_isPressingMovement) {
                // _velocity *= groundDrag * Time.deltaTime;
                _velocity *= groundDrag;
            }

            _velocity += input.y * forward * groundAcceleration * Time.fixedDeltaTime;
            _velocity += input.x * right * groundAcceleration * Time.fixedDeltaTime;
            _velocity.y = -antiBump;

            // jump
            if (Input.GetKey(KeyCode.Space))
            {
                _velocity.y += jumpForce;
            }

        } else {
            _velocity += input.y * forward * airAcceleration * Time.fixedDeltaTime;
            _velocity += input.x * right * airAcceleration * Time.fixedDeltaTime;

            // reduce impact of gravity if pressing space
            if (!_isFalling && Input.GetKey(KeyCode.Space)) {
                _velocity.y -= gravityHold * Time.fixedDeltaTime;
            } else {
                _velocity.y -= gravityNormal * Time.fixedDeltaTime;
            }
        }

        // grapplin
        // if (_isGrapplin) {
        //     float distance_grapplinPoint = Vector3.Distance(transform.position, _grapplinPoint);
        //     Vector3 dirGrapplin = (transform.position - _grapplinPoint).normalized; 

        //     if (distance_grapplinPoint > _grapplinLength) {
        //         // charController.transform.position = new Vector3(some location);
        //         this.GetComponent<CharacterController>().enabled = false;
        //         transform.position = _grapplinPoint + dirGrapplin * _grapplinLength;
        //         this.GetComponent<CharacterController>().enabled = true;

        //         Vector3 badDir = (-dirGrapplin) * Vector3.Dot(_velocity, -dirGrapplin);
        //         _velocity -= badDir; // clamp velocity to normal's plane 
        //         _velocity += _velocity.normalized * badDir.magnitude;
        //     }
        // }


        // clamp speed
        float speedLimit;
        if (!_controller.isGrounded) {
            speedLimit = airSpeedLimit;
        } else if (_isSprinting) {
            speedLimit = sprintSpeed;
        } else {
            speedLimit = walkSpeed;
        }
        Vector2 xzVelocity = new Vector2(_velocity.x, _velocity.z);
        if (xzVelocity.magnitude > speedLimit) {
            xzVelocity = xzVelocity.normalized * speedLimit;

            _velocity.x = xzVelocity.x;
            _velocity.z = xzVelocity.y;
        }

        _velocity.y = Mathf.Clamp(_velocity.y, -50f, 30f);
    }
}
