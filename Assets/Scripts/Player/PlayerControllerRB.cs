using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerRB : MonoBehaviour
{
    // TODO: add different gravity when player is falling

    [SerializeField] Camera _playerCamera;
    Rigidbody _rb;
    public Transform _groundChecker;

    // public Vector3 _velocity = Vector3.zero;

    LayerMask _groundMask;
    Vector3 _inputs = Vector3.zero;
    [SerializeField] bool _isGrounded = true;

    Vector3 _moveDirection;

    public float _walkSpeed = 120f;
    public float _sprintSpeed = 150f;
    // const float _airSpeedLimit = 20f;

    public float _groundDrag = 0.8f; // only applied when player is not moving
    public float _airDragHorizontal = 0.9f;
    public float _airDragVertical = 1f; 
    
    // const float _groundAcceleration = 150f;
    // const float _airAcceleration = 25f;

    public float jumpForce = 5f;
    public float gravityNormal = 30f;
    public float gravityHold = 20f;

    float defaultFOV;
    float sprintFOV;

    bool _isFalling = false;
    bool _isSprinting = false;
    bool _canWallJump = true;
    bool _isPressingMovement = false; // if player is holding/pressing key

    bool _isAgainstWall = false;
    Vector3 _wallNormal;


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

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.drag = 0f;

        defaultFOV = _playerCamera.fieldOfView;
        sprintFOV = _playerCamera.fieldOfView * 1.1f;
    }

    void Update()
    {
        Vector3 forward = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z);
        Vector3 right = new Vector3(_playerCamera.transform.right.x, 0f, _playerCamera.transform.right.z);

        _inputs = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;
        _moveDirection = (_inputs.x * right) + (_inputs.z * forward);

        _groundMask = LayerMask.GetMask("Ground"); // move this in Start
        _isGrounded = Physics.CheckSphere(_groundChecker.position, 0.4f, _groundMask);

        updateStates();

        if (_isSprinting) {
            _moveDirection *= _sprintSpeed;
        } else {
            _moveDirection *= _walkSpeed;
        }

        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _rb.AddForce(Vector3.up * Mathf.Sqrt(jumpForce * -2f * -gravityNormal), ForceMode.VelocityChange);
        }

        if (_isSprinting && _isPressingMovement) {
            _playerCamera.fieldOfView = Mathf.Lerp(_playerCamera.fieldOfView, sprintFOV, Easing.easeOutCubic(5f * Time.deltaTime)); // easing actually useless
        } else {
            _playerCamera.fieldOfView = Mathf.Lerp(_playerCamera.fieldOfView, defaultFOV, Easing.easeOutCubic(5f * Time.deltaTime));
        }
    }

    void updateStates()
    {
        _isPressingMovement = _inputs.magnitude > 0.1f;
        
        _isFalling = _rb.velocity.y < 0f && !_isGrounded;
        
        if (_isGrounded) {
            _canWallJump = true;
        }

        _isSprinting = Input.GetKey(KeyCode.LeftShift);
    }


    void FixedUpdate()
    {
        _rb.AddForce(_moveDirection * Time.fixedDeltaTime, ForceMode.VelocityChange);

        Vector3 drag;
        if (_isGrounded) {
            drag = new Vector3(_groundDrag, _groundDrag, _groundDrag);
        } else {
            drag = new Vector3(_airDragHorizontal, _airDragVertical, _airDragHorizontal);
        }
        // float drag = _isGrounded ? _groundDragMultiplier : _airDragMultiplier;
        _rb.velocity = new Vector3(_rb.velocity.x * drag.x, _rb.velocity.y * drag.y, _rb.velocity.z * drag.z);

        // reduce impact of gravity if pressing space
        _rb.AddForce(Vector3.down * gravityNormal * Time.fixedDeltaTime, ForceMode.VelocityChange);
        // if (!_isFalling && Input.GetKey(KeyCode.Space)) {
        //     _rb.AddForce(Vector3.down * gravityHold * Time.fixedDeltaTime, ForceMode.VelocityChange);
        // } else {
        //     _rb.AddForce(Vector3.down * gravityHold * Time.fixedDeltaTime, ForceMode.VelocityChange);
        // }

    }

}
