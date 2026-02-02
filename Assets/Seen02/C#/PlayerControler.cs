using UnityEngine;
using System.Collections;

public class PlayerControler : MonoBehaviour
{
    [Header("Left and Right Movement")]
    [SerializeField] private float _stepDistanse = 2.5f;
    [SerializeField] private float _sideMOveSpeed = 8f;
    private int _currentStep = 1;

    [Header("Jump")]
    [SerializeField] private float _jumpForce = 7f;
    [SerializeField] private float _gravity = -20f;
    private float _verticalVelocity;
    private bool _isGrounded = true;

    [Header("Slide / Crouch")]
    [SerializeField] private float _slideDuration = 1f;
    [SerializeField] private float _heightLerpSpeed = 8f;
    [SerializeField] private float _standUpJumpLerpSpeed = 14f;
    private bool _isSliding;
    private Coroutine _slideRoutine;

    [Header("Death")]
    [SerializeField] private float _destroyDelay = 2f;
    private bool _isDead;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController controller;
    [SerializeField] private CapsuleCollider capsule;

    private float _lockz;
    private float _originalHeight;
    private Vector3 _originalCenter;
    private float _targetHeight;

    public bool isPowerUpOn = false;

    private void Awake()
    {
        animator.applyRootMotion = false;
        animator.SetBool("isRunning", true);

        _lockz = transform.position.z;

        _originalHeight = controller.height;
        _originalCenter = controller.center;
    }

    private void Update()
    {
        if (_isDead) return;

        HandleInput();
        HandleMovement();
        ApplyGravity();
    }

    private void LateUpdate()
    {
        Vector3 pos = transform.position;
        pos.z = _lockz;
        transform.position = pos;
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            ChangeLane(-1);

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            ChangeLane(1);

        //JUMP ground OR mid crouch
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (_isSliding)
            {
                JumpFromCrouch();
            }
            else if (_isGrounded)
            {
                Jump();
            }
        }

        // CROUCH
        if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            && _isGrounded && !_isSliding)
        {
            _slideRoutine = StartCoroutine(Slide());
        }
    }

    private bool CanPlayLaneAnimation()
    {
        return _isGrounded && !_isSliding && !_isDead;
    }

    private void ChangeLane(int direction)
    {
        int targetLane = Mathf.Clamp(_currentStep + direction, 0, 2);
        if (targetLane == _currentStep) return;

        _currentStep = targetLane;

        if (!CanPlayLaneAnimation()) return;

        StartCoroutine(PlayLaneBool(direction < 0 ? "moveLeft" : "moveRight"));
    }

    private IEnumerator PlayLaneBool(string boolName)
    {
        animator.SetBool(boolName, true);
        yield return new WaitForSeconds(0.15f);
        animator.SetBool(boolName, false);
    }

    private void HandleMovement()
    {
        float targetX = (_currentStep - 1) * _stepDistanse;
        float diff = targetX - transform.position.x;

        Vector3 move;
        move.x = diff * _sideMOveSpeed;
        move.y = _verticalVelocity;
        move.z = 0f;

        controller.Move(move * Time.deltaTime);
    }

    private void Jump()
    {
        _verticalVelocity = _jumpForce;
        _isGrounded = false;

        animator.SetBool("isJumping", true);
        animator.SetBool("isSliding", false);
        animator.SetBool("isRunning", false);
        animator.speed = 1.4f;
    }

    // JUMP mid crouch
    private void JumpFromCrouch()
    {
        if (_slideRoutine != null)
            StopCoroutine(_slideRoutine);

        _isSliding = false;

        StartCoroutine(StandUpThenJump());
    }

    private IEnumerator StandUpThenJump()
    {
        while (Mathf.Abs(controller.height - _originalHeight) > 0.01f)
        {
            controller.height = Mathf.Lerp(
                controller.height,
                _originalHeight,
                Time.deltaTime * _standUpJumpLerpSpeed
            );

            controller.center = new Vector3(
                _originalCenter.x,
                controller.height / 2f,
                _originalCenter.z
            );

            yield return null;
        }

        controller.height = _originalHeight;
        controller.center = _originalCenter;

        Jump();
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded)
        {
            if (!_isGrounded)
            {
                _isGrounded = true;
                animator.SetBool("isJumping", false);
                animator.speed = 1f;
                animator.SetBool("isRunning", true);
            }

            _verticalVelocity = -2f;
        }
        else
        {
            _verticalVelocity += _gravity * Time.deltaTime;
        }
    }

    private IEnumerator Slide()
    {
        _isSliding = true;

        animator.SetBool("isSliding", true);
        animator.SetBool("isRunning", false);

        _targetHeight = _originalHeight * 0.5f; 

        float originalCapsuleHeight = capsule.height; 
        Vector3 originalCapsuleCenter = capsule.center; 

        while (Mathf.Abs(controller.height - _targetHeight) > 0.01f)
        {
            controller.height = Mathf.Lerp(controller.height, _targetHeight, Time.deltaTime * _heightLerpSpeed);
            controller.center = new Vector3(_originalCenter.x, controller.height / 2f, _originalCenter.z);

            capsule.height = Mathf.Lerp(capsule.height, _targetHeight, Time.deltaTime * _heightLerpSpeed);
            capsule.center = new Vector3(originalCapsuleCenter.x, capsule.height / 2f, originalCapsuleCenter.z);

            yield return null;
        }

        yield return new WaitForSeconds(_slideDuration); 

        while (Mathf.Abs(controller.height - _originalHeight) > 0.01f)
        {

            controller.height = Mathf.Lerp(controller.height, _originalHeight, Time.deltaTime * _heightLerpSpeed);
            controller.center = new Vector3(_originalCenter.x, controller.height / 2f, _originalCenter.z);

            capsule.height = Mathf.Lerp(capsule.height, _originalHeight, Time.deltaTime * _heightLerpSpeed);
            capsule.center = new Vector3(originalCapsuleCenter.x, capsule.height / 2f, originalCapsuleCenter.z);

            yield return null;
        }

        controller.height = _originalHeight;
        controller.center = _originalCenter;
        capsule.height = _originalHeight;
        capsule.center = originalCapsuleCenter;

        animator.SetBool("isSliding", false);
        animator.SetBool("isRunning", true);

        _isSliding = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle") && !_isDead && !isPowerUpOn)
            Die();
    }

    private void Die()
    {
        _isDead = true;
        animator.SetBool("isRunning", false);
        animator.SetBool("isDead", true);
        EndlessEnvironment.isPlayerDead = true;
        StartCoroutine(DestroyPlayer());
    }

    private IEnumerator DestroyPlayer()
    {
        yield return new WaitForSeconds(_destroyDelay);
        Debug.Log("Is Player Dead");
    }
}//Class
