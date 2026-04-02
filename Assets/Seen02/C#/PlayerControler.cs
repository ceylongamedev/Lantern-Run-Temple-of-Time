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

    private float _lockz;
    private float _originalHeight;
    private Vector3 _originalCenter;
    private float _targetHeight;

    [Header("Magnet Settings")]
    [SerializeField] private float _magnetRadius = 5f;
    [SerializeField] private float _magnetSpeed = 10f;
    [SerializeField] private LayerMask _coinLayer;
    public bool magnetActive = false;
    public bool isPowerUpOn = false;
    [SerializeField] private float _magnetDuration = 10f;
    [SerializeField] private float _speedUpDuration = 10f;

    [Header("Lantern")]
    [SerializeField] private Transform _lantern;
    [SerializeField] private float _lanternSlideYOffset = -0.35f;
    [SerializeField] private float _lanternLerpSpeed = 10f;
    private Vector3 _lanternOriginalLocalPos;
    [SerializeField] private GameObject _LnaternObject;

    [Header("Renderer")]
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private Color speedColer = Color.blue;
    [SerializeField] private GameObject magnet;

    [SerializeField] private UI_Manager uiScript;

    [Header("Lantern To Enable")]
    public ParticleSystem hitParticle;
    public float disableAfter = 2f;

    [Header("Foot Dust")]
    [SerializeField] private ParticleSystem footDust;


    private void Awake()
    {
        animator.applyRootMotion = false;
        animator.SetBool("isRunning", true);

        _lockz = transform.position.z;

        _originalHeight = controller.height;
        _originalCenter = controller.center;


        if (_lantern != null)
            _lanternOriginalLocalPos = _lantern.localPosition;
    }

    private void Start()
    {
        magnetActive = false;
        isPowerUpOn = false;
    }

    private void Update()
    {
        //SpeedUp
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
            //isPowerUpOn = !isPowerUpOn;
        //}
        if (isPowerUpOn)
        {
            targetRenderer.material.SetFloat("_ON", 1f);
            targetRenderer.material.SetColor("BottomColor", speedColer);
        }
        else if (!isPowerUpOn)
        {
            targetRenderer.material.SetFloat("_ON", 0f);
        }
        //Magnet

        //if (Input.GetKeyDown(KeyCode.E))
        //{
            //magnetActive = !magnetActive;
        //}
            

        if (_isDead) return;

        HandleInput();
        HandleMovement();
        ApplyGravity();
        HandleFootDust();

        if (!magnetActive)
        {
            magnet.SetActive(false);
            return;
        }

        magnet.SetActive(true);
        Collider[] coins = Physics.OverlapSphere(transform.position, _magnetRadius, _coinLayer);

        foreach (Collider coin in coins)
        {
            if (coin == null) continue;
            Vector3 direction = (transform.position - coin.transform.position).normalized;
            coin.transform.position += direction * _magnetSpeed * Time.deltaTime;
        }
    }

    private void HandleFootDust()
    {
        if (footDust == null) return;

        if (_isDead)
        {
            if (footDust.isPlaying)
                footDust.Stop();
            return;
        }

        if (controller.isGrounded && !_isSliding)
        {
            if (!footDust.isPlaying)
                footDust.Play();
        }
        else
        {
            if (footDust.isPlaying)
                footDust.Stop();
        }
    }

    private IEnumerator MoveLanternY(float targetYOffset)
    {
        if (_lantern == null) yield break;

        Vector3 start = _lantern.localPosition;
        Vector3 target = _lanternOriginalLocalPos + Vector3.up * targetYOffset;

        while (Vector3.Distance(_lantern.localPosition, target) > 0.01f)
        {
            _lantern.localPosition = Vector3.Lerp(
                _lantern.localPosition,
                target,
                Time.deltaTime * _lanternLerpSpeed
            );
            yield return null;
        }

        _lantern.localPosition = target;
    }

    private void ResetLantern()
    {
        if (_lantern == null) return;
        StartCoroutine(MoveLanternY(0f));
    }//Lantern

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
        ResetLantern();

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

        StartCoroutine(MoveLanternY(_lanternSlideYOffset));

        _targetHeight = _originalHeight * 0.5f;

        // crouch
        while (Mathf.Abs(controller.height - _targetHeight) > 0.01f)
        {
            controller.height = Mathf.Lerp(
                controller.height,
                _targetHeight,
                Time.deltaTime * _heightLerpSpeed
            );

            controller.center = new Vector3(
                _originalCenter.x,
                controller.height / 2f,
                _originalCenter.z
            );

            yield return null;
        }

        yield return new WaitForSeconds(_slideDuration);

        // stand up
        while (Mathf.Abs(controller.height - _originalHeight) > 0.01f)
        {
            controller.height = Mathf.Lerp(
                controller.height,
                _originalHeight,
                Time.deltaTime * _heightLerpSpeed
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

        ResetLantern();

        animator.SetBool("isSliding", false);
        animator.SetBool("isRunning", true);

        _isSliding = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            CoinFlyEffect.Instance.PlayFlyEffect(transform.position);// Coing flay
            UI_Manager.Instance.UpdateCoins(1);
        }
        if (other.CompareTag("Obstacle") && !_isDead && !isPowerUpOn)
            Die();

        if (other.CompareTag("Lantern")) 
        {
            ActivateObjects();
            UI_Manager.Instance.UpdateScore(20);
        }

        if (other.CompareTag("Mgnet"))
        {
            magnetActive = true;
            Invoke(nameof(DisableMagnet), _magnetDuration);
            Destroy(other.gameObject);
        }
        if (other.CompareTag("SpeedUP"))
        {
            isPowerUpOn = true;
            Invoke(nameof(DisableSpeed), _speedUpDuration);
            Destroy(other.gameObject);
        }
    }

    void DisableMagnet()
    {
        magnetActive = false;
    }

    void DisableSpeed()
    {
        isPowerUpOn = false;
    }

    private void ActivateObjects()
    {
        if (hitParticle != null)
            hitParticle.Play();

        Invoke(nameof(DeactivateObjects), disableAfter);
    }
    private void DeactivateObjects()
    {
        hitParticle.Stop();
    }

    private void Die()
    {
        _isDead = true;
        animator.SetBool("isRunning", false);
        animator.SetBool("isDead", true);
        EndlessEnvironment.isPlayerDead = true;
        _LnaternObject.SetActive(false);
        StartCoroutine(DestroyPlayer());
    }

    private IEnumerator DestroyPlayer()
    {
        yield return new WaitForSeconds(_destroyDelay);
        
        if (uiScript)
        {
            uiScript.EndGame();
            Debug.Log("Is Player Dead");
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _magnetRadius);
    }

}//Class
