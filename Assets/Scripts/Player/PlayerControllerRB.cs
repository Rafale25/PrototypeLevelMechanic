using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerRB : MonoBehaviour
{
    // TODO: add different gravity when player is falling

    [SerializeField] Camera _playerCamera;
    // [SerializeField] LineRenderer _lineRenderer;
    LineRenderer _lineRenderer;
    Rigidbody _rb;
    public Transform _groundChecker;

    // public Vector3 _velocity = Vector3.zero;

    LayerMask _groundMask;
    Vector3 _inputs = Vector3.zero;
    [SerializeField] bool _isGrounded = true;

    Vector3 _moveDirection;

    float _walkSpeed = 120f;
    float _sprintSpeed = 150f;

    float _groundDrag = 0.8f; // only applied when player is not moving
    float _airDragHorizontal = 0.993f;
    float _airDragVertical = 0.999f;

    const float _groundAcceleration = 1f;
    const float _airAcceleration = 0.18f;

    float jumpForce = 5f;
    float gravityNormal = 30f;
    float gravityHold = 20f;

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
    const float maxGrapplinLength = 175f;

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
        _rb.drag = 0f; // doing the drag manually

        defaultFOV = _playerCamera.fieldOfView;
        sprintFOV = _playerCamera.fieldOfView * 1.1f;

        _lineRenderer = GetComponent<LineRenderer>();
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

        float speed = _isSprinting ? _sprintSpeed : _walkSpeed;
        float acceleration = _isGrounded ? _groundAcceleration : _airAcceleration;
        _moveDirection *= speed * acceleration;

        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _rb.AddForce(Vector3.up * Mathf.Sqrt(jumpForce * -2f * -gravityNormal), ForceMode.VelocityChange);
        }

        if (_isSprinting && _isPressingMovement) {
            _playerCamera.fieldOfView = Mathf.Lerp(_playerCamera.fieldOfView, sprintFOV, Easing.easeOutCubic(5f * Time.deltaTime)); // easing actually useless
        } else {
            _playerCamera.fieldOfView = Mathf.Lerp(_playerCamera.fieldOfView, defaultFOV, Easing.easeOutCubic(5f * Time.deltaTime));
        }


        if (Input.GetMouseButtonDown(0))
        {
            if (_isGrapplin)
            {
                _isGrapplin = false;
                _lineRenderer.enabled = false;
            }
            else
            {
                RaycastHit hit;
                if (Physics.Raycast(_playerCamera.transform.position, _playerCamera.transform.forward, out hit, maxGrapplinLength))
                {
                    _isGrapplin = true;
                    _grapplinPoint = hit.point;
                    _grapplinLength = hit.distance;

                    StartCoroutine(GrapplingAcceleration());
                }
                _lineRenderer.enabled = true;
            }
        }

        if (_isGrapplin) {
            _grapplinLength += Input.mouseScrollDelta.y;
            // _lineRenderer.SetPosition(0, _playerCamera.transform.position + _playerCamera.transform.forward * 0.2f + new Vector3(.0f, -.3f, .0f));
            _lineRenderer.SetPosition(0, _playerCamera.transform.position + _playerCamera.transform.right*0.5f );
            _lineRenderer.SetPosition(1, _grapplinPoint);
        }


    }

    IEnumerator GrapplingAcceleration()
    {
        int frames = 12;

        while (frames > 0) {
            // Debug.Log(frames);
            Vector3 dirGrapplin = (transform.position - _grapplinPoint).normalized;
            _rb.AddForce(-dirGrapplin * 140f, ForceMode.Acceleration);

            yield return new WaitForFixedUpdate();
            frames -= 1;
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

        // Debug.Log(_grapplinLength);

        Vector3 drag;
        if (_isGrounded) {
            drag = new Vector3(_groundDrag, _groundDrag, _groundDrag);
        } else {
            drag = new Vector3(_airDragHorizontal, _airDragVertical, _airDragHorizontal);
            // drag = Vector3.one;
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

        if (_isGrapplin) {
            float distance_grapplinPoint = Vector3.Distance(transform.position, _grapplinPoint);
            Vector3 dirGrapplin = (transform.position - _grapplinPoint).normalized;

            if (distance_grapplinPoint < _grapplinLength) {
                _grapplinLength = distance_grapplinPoint;
            } else {
                transform.position = _grapplinPoint + dirGrapplin * _grapplinLength;


                float delta = Vector3.Dot(_rb.velocity, -dirGrapplin);
                if (delta < 0f) {
                    Vector3 badDir = (-dirGrapplin) * delta;
                    _rb.velocity -= badDir; // clamp velocity to normal's plane
                }
                // _rb.velocity += _rb.velocity.normalized * badDir.magnitude;
            }
        }

    }

}
