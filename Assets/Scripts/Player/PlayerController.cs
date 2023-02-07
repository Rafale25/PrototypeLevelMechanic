using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    // TODO: add different gravity when player is falling
    // TODO: velocity is local to player

    [SerializeField] Camera _playerCamera;
    CharacterController _controller;
    public Vector3 _velocity = Vector3.zero;

    float walkSpeed = 15f;//6f;
    float sprintSpeed = 30f;

    float groundAcceleration = 1f;
    float airAcceleration = 0.075f;
    float groundDrag = 0.95f;

    const float dashCoolDown = 3.0f;
    const float dashDuration = 0.175f;

    bool _isDashing = false;
    bool _isFalling = false;
    bool _isSprinting = false;
    bool _canDash = true;
    bool _canWallJump = true;
    bool _isPressingMovement = false; // if player is holding/pressing key

    bool _isAgainstWall = false;
    Vector3 _wallNormal;

    float jumpForce = 25f;
    float antiBump = 4.5f;
    float gravityNormal = 35f;
    float gravityHold = 25f;

    float defaultFOV;
    float sprintFOV;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        defaultFOV = _playerCamera.fieldOfView;
        sprintFOV = _playerCamera.fieldOfView * 1.1f;
    }

    void Update()
    {
        // if (_canDash && Input.GetKeyDown(KeyCode.LeftShift)) {
        //     StartCoroutine(TriggerDash());
        // }
        // if (_isDashing) return;
        DefaultMovement();

        if (_isSprinting && _isPressingMovement) {
            _playerCamera.fieldOfView = Mathf.Lerp(_playerCamera.fieldOfView, sprintFOV, 0.03f);
        } else {
            _playerCamera.fieldOfView = Mathf.Lerp(_playerCamera.fieldOfView, defaultFOV, 0.03f);
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!_controller.isGrounded) {
            _isAgainstWall = true;
            _wallNormal = hit.normal;
            Debug.DrawRay(hit.point, hit.normal, Color.red, 10f);
        } else {
            _isAgainstWall = false;
        }
    }



    void FixedUpdate()
    {
        // Vector3 worldVelocity = transform.TransformDirection(_velocity);
        Vector3 worldVelocity = _velocity;

        if (_isDashing) {

            worldVelocity = _playerCamera.transform.rotation * Vector3.forward;
            worldVelocity *= 15.0f * sprintSpeed;
            _controller.Move(worldVelocity * Time.fixedDeltaTime);

        } else {

            // if not pressing space, increase velocity when falling
            if (_velocity.y < 0f && !Input.GetKey(KeyCode.Space))
                worldVelocity.y *= 1.4f;

            _controller.Move(worldVelocity * Time.fixedDeltaTime);

        }
    }

    IEnumerator TriggerDash()
    {
        _isDashing = true;
        _canDash = false;

        float defaultFOV = _playerCamera.fieldOfView;
        _playerCamera.fieldOfView *= 1.05f;

        yield return new WaitForSeconds(dashDuration);

        _playerCamera.fieldOfView = defaultFOV;
        _isDashing = false;

        yield return new WaitForSeconds(dashCoolDown);

        _canDash = true;
    }


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

        // wall jump
        if (!_controller.isGrounded && Input.GetKeyDown(KeyCode.Space)) {
            if (_isAgainstWall && _canWallJump) {
                // check if wallNormal is horizontal enough to jump on
                float slopeAngle = Vector3.Angle(_wallNormal, Vector3.up);
                if (slopeAngle < 91f && slopeAngle > 60f) {

                    // Vector3 wallNormal90 = Vector3.Cross(_wallNormal, Vector3.up);
                    // _velocity = Vector3.Scale(_velocity , wallNormal90);

                    // _velocity += transform.InverseTransformDirection(_wallNormal) * 40f + Vector3.up * 15f; 
                    _velocity += _wallNormal * 40f + Vector3.up * 15f; 
                    _canWallJump = false;
                }
            }
        }

        if (_controller.isGrounded) {

            _velocity *= groundDrag;

            Debug.Log(forward);

            _velocity += input.y * forward * groundAcceleration;
            _velocity += input.x * right * groundAcceleration;
            // _velocity.x += input.x * groundAcceleration;
            // _velocity.z += input.y * groundAcceleration;
            _velocity.y = -antiBump;

            // jump
            if (Input.GetKey(KeyCode.Space))
            {
                _velocity.y += jumpForce;
            }

        } else {
            // _velocity.x += input.x * airAcceleration;
            // _velocity.z += input.y * airAcceleration;
            _velocity += input.y * forward * airAcceleration;
            _velocity += input.x * right * airAcceleration;

            // reduce impact of gravity if pressing space
            if (!_isFalling && Input.GetKey(KeyCode.Space)) {
                _velocity.y -= gravityHold * Time.deltaTime;
            } else {
                _velocity.y -= gravityNormal * Time.deltaTime;
            }

        }

        float speedLimit = walkSpeed;
        if (Input.GetKey(KeyCode.LeftShift)) {
            _isSprinting = true;
        } else {
            _isSprinting = false;
        }

        if (_isSprinting) {
            speedLimit = sprintSpeed;
        }

        // clamp speed
        Vector2 xzVelocity = new Vector2(_velocity.x, _velocity.z);
        if (xzVelocity.magnitude > speedLimit) {
            xzVelocity = xzVelocity.normalized * speedLimit;

            _velocity.x = xzVelocity.x;
            _velocity.z = xzVelocity.y;
        }
    }
}
