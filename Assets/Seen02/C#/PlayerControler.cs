using System.Collections;
using UnityEngine;

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

    [Header("Slide")]
    [SerializeField] private float _slideDuration = 1f;
    private bool _isSliding;

    [Header("Death")]
    [SerializeField] private float _destroyDelay = 2f;
    private bool _isDead;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController controller;


    private void Awake()
    {
        animator.applyRootMotion = false;
    }

    private void Update()
    {
        if (_isDead) return;

        HandleInput();
        HandleMovement();
        ApplyGravity();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.A)||Input.GetKeyDown(KeyCode.LeftArrow))
            ChangeLane(-1);

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            ChangeLane(1);

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && _isGrounded && !_isSliding)
            Jump();

        if ((Input.GetKeyDown(KeyCode.S)|| Input.GetKeyDown(KeyCode.DownArrow)) && _isGrounded && !_isSliding)
            StartCoroutine(Slide());
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

        if (direction < 0)
            animator.SetTrigger("moveLeft");
        else
            animator.SetTrigger("moveRight");
    }

    private void HandleMovement()
    {
        float targetX = (_currentStep - 1) * _stepDistanse;
        float diff = targetX - transform.position.x;

        Vector3 move = Vector3.right * diff * _sideMOveSpeed;
        move.y = _verticalVelocity;

        controller.Move(move * Time.deltaTime);
    }

    private void Jump()
    {
        _verticalVelocity = _jumpForce;
        _isGrounded = false;

        animator.SetBool("isJumping", true);
        animator.speed = 1.4f; 
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

                animator.ResetTrigger("moveLeft");
                animator.ResetTrigger("moveRight");
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

        float originalHeight = controller.height;
        controller.height = originalHeight / 2;

        yield return new WaitForSeconds(_slideDuration);

        controller.height = originalHeight;
        animator.SetBool("isSliding", false);
        _isSliding = false;

        animator.ResetTrigger("moveLeft");
        animator.ResetTrigger("moveRight");
        animator.SetBool("isRunning", true);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit == null) return;
        if (hit.collider == null) return;

        if (hit.collider.CompareTag("Obstacle") && !_isDead)
        {
            Die();
        }
    }

    private void Die()
    {
        _isDead = true;

        animator.SetBool("isRunning", false);
        animator.SetBool("isDead", true);

        StartCoroutine(DestroyPlayer());
    }

    private IEnumerator DestroyPlayer()
    {
        yield return new WaitForSeconds(_destroyDelay);
        Destroy(gameObject);
    }

}//Class
