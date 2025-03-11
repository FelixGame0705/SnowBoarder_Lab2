using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField]
    private AudioClip hitSound;

    [SerializeField]
    private AudioClip jumpSound;

    [SerializeField]
    private AudioClip landingSound;

    [SerializeField]
    private AudioClip speedGateSound;

    [SerializeField]
    private AudioClip pickItemSound;

    [Header("Movement Settings")]
    [SerializeField]
    private float torqueAmount = 5f;

    [SerializeField]
    private float maxForce = 80f;

    [SerializeField]
    private float baseSpeed = 20f;

    [SerializeField]
    private Vector2 boostDirection = Vector2.right;

    [Header("Jump Settings")]
    [SerializeField]
    private float jumpForce = 10f;

    [SerializeField]
    private float maxJumpTime = 0.3f;

    [Header("Component References")]
    [SerializeField]
    private Animator m_Animator;

    [SerializeField]
    private GroundChecker groundChecker;

    [SerializeField]
    private BoostController boostController;

    [SerializeField]
    private GameObject peguin;

    [SerializeField]
    private ParticleSystem boostParticles;

    [Header("Speed Gate Settings")]
    [SerializeField]
    private float defaultSpeedGateBoostDuration = 2f;

    [SerializeField]
    private int collisionCount = 0;

    private Rigidbody2D rb2d;
    private SurfaceEffector2D surfaceEffector2D;
    private CameraShakeManager cameraShake;
    private AudioSource audioSource;

    private float previousRotation;
    private float cumulativeRotation;
    private bool _isJumping;
    private float _jumpTimeCounter;
    private float forceToUse;

    private bool isSpeedGateBoosting;
    private float speedGateRemainingTime;
    private float speedGateBoostForce;

    void Start()
    {
        InitializeComponents();
        SetupEventSubscriptions();
    }

    private void InitializeComponents()
    {
        cameraShake = FindObjectOfType<CameraShakeManager>();
        rb2d = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        surfaceEffector2D = FindAnyObjectByType<SurfaceEffector2D>();
        m_Animator = GetComponent<Animator>();

        previousRotation = transform.eulerAngles.z;
        cumulativeRotation = 0f;

        if (surfaceEffector2D != null)
        {
            surfaceEffector2D.speed = baseSpeed;
        }

        EnsureRequiredComponents();
    }

    private void EnsureRequiredComponents()
    {
        if (groundChecker == null)
        {
            groundChecker = GetComponent<GroundChecker>();
            if (groundChecker == null)
            {
                groundChecker = gameObject.AddComponent<GroundChecker>();
                Debug.LogWarning(
                    "GroundChecker was not assigned, automatically added one to the player."
                );
            }
        }

        if (boostController == null)
        {
            boostController = GetComponent<BoostController>();
            if (boostController == null)
            {
                boostController = gameObject.AddComponent<BoostController>();
                Debug.LogWarning(
                    "BoostController was not assigned, automatically added one to the player."
                );
            }
        }
    }

    private void SetupEventSubscriptions()
    {
        groundChecker.OnLandingChanged += LandingExecute;
        groundChecker.OnFlyingChanged += FlyingExecute;
        boostController.OnBoostStateChanged += OnBoostStateChanged;
    }

    private void OnDestroy()
    {
        if (boostController != null)
        {
            boostController.OnBoostStateChanged -= OnBoostStateChanged;
        }

        if (groundChecker != null)
        {
            groundChecker.OnLandingChanged -= LandingExecute;
            groundChecker.OnFlyingChanged -= FlyingExecute;
        }
    }

    void Update()
    {
        RotatePlayer();
        CheckRotationTrick();
        HandleJumpInput();
        HandleDownInput();

        if (isSpeedGateBoosting)
        {
            speedGateRemainingTime -= Time.deltaTime;
            if (speedGateRemainingTime <= 0)
            {
                isSpeedGateBoosting = false;

                if (boostParticles != null && !boostController.IsBoosting)
                {
                    boostParticles.Stop();
                }
            }
        }
    }

    void FixedUpdate()
    {
        ApplyBoost();
    }

    private void OnBoostStateChanged(bool isBoosting)
    {
        if (boostParticles != null)
        {
            if (isBoosting || isSpeedGateBoosting)
                boostParticles.Play();
            else
                boostParticles.Stop();
        }
    }

    private void ApplyBoost()
    {
        Vector2 forwardDirection = transform.right;
        forceToUse = 0f;

        if (boostController.IsBoosting || isSpeedGateBoosting)
        {
            forceToUse = boostController.IsBoosting
                ? boostController.BoostForce
                : speedGateBoostForce;
            if (boostController.IsBoosting && isSpeedGateBoosting)
            {
                forceToUse =
                    (boostController.BoostForce + speedGateBoostForce) > maxForce
                        ? maxForce
                        : boostController.BoostForce + speedGateBoostForce;
            }

            rb2d.sharedMaterial.friction = 0f;

            Vector2 forceVector = forwardDirection.normalized * forceToUse;
            rb2d.AddForce(forceVector, ForceMode2D.Force);
        }
        else
        {
            rb2d.sharedMaterial.friction = 0.02f;
        }
    }

    public void ActivateSpeedGateBoost(float duration, float force)
    {
        speedGateRemainingTime = duration;
        speedGateBoostForce = force;
        isSpeedGateBoosting = true;

        if (speedGateSound != null)
        {
            audioSource.PlayOneShot(speedGateSound);
        }

        if (boostParticles != null && !boostParticles.isPlaying)
        {
            boostParticles.Play();
        }
    }

    public void LandingExecute()
    {
        audioSource.PlayOneShot(landingSound);
        StartCoroutine(Landing());
    }

    public void FlyingExecute()
    {
        StopAllCoroutines();
        m_Animator.Play("Flying");
    }

    IEnumerator Landing()
    {
        m_Animator.Play("Landing");
        yield return new WaitForSeconds(1f);
        m_Animator.Play("Idle");
    }

    private void RotatePlayer()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        rb2d.AddTorque(-horizontalInput * torqueAmount);
    }

    private void CheckRotationTrick()
    {
        float currentRotation = transform.eulerAngles.z;
        float deltaAngle = Mathf.DeltaAngle(previousRotation, currentRotation);

        if (cumulativeRotation != 0 && Mathf.Sign(deltaAngle) != Mathf.Sign(cumulativeRotation))
        {
            cumulativeRotation = 0;
        }
        cumulativeRotation += deltaAngle;
        previousRotation = currentRotation;

        if (Mathf.Abs(cumulativeRotation) >= 360f)
        {
            ScoreManager.Instance.AddTrickScore(50);
            audioSource.PlayOneShot(pickItemSound);
            cumulativeRotation = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            ScoreManager.Instance.AddCollectibleScore(10);
            audioSource.PlayOneShot(pickItemSound);
            collision.GetComponent<SpriteRenderer>().enabled = false;
            collision.GetComponent<Collider2D>().enabled = false;
        }

        if (collision.CompareTag("SpeedGate"))
        {
            SpeedGate speedGate = collision.GetComponent<SpeedGate>();
            if (speedGate != null)
            {
                ActivateSpeedGateBoost(speedGate.boostDuration, speedGate.boostForce);
                speedGate.Activate();
            }
            else
            {
                ActivateSpeedGateBoost(defaultSpeedGateBoostDuration, boostController.BoostForce);
            }

            ScoreManager.Instance.AddTrickScore(25);
        }

        if (collision.CompareTag("Peguin") && boostController != null)
        {
            peguin.SetActive(true);
            boostController.AddBoostEnergy(100f);
            audioSource.PlayOneShot(pickItemSound);
            collision.gameObject.SetActive(false);
        }

        if (collision.CompareTag("EnergyItem"))
        {
            boostController.AddBoostEnergy(10f);
            audioSource.PlayOneShot(pickItemSound);
            collision.gameObject.SetActive(false);
        }

        if (collision.CompareTag("ObstacleEdge"))
        {
            collisionCount++;
            AvalancheTracker.Instance.TurnOn(ref collisionCount, transform.position);
            audioSource.PlayOneShot(hitSound);
            rb2d.AddForce(Vector2.zero, ForceMode2D.Impulse);
            collision.enabled = false;
        }
    }

    void HandleDownInput()
    {
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            m_Animator.Play("Falling");
        }

        if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow))
        {
            m_Animator.Play("Idle");
        }
    }

    void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            StartJump();
            audioSource.PlayOneShot(jumpSound);
        }

        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && _isJumping)
        {
            ContinueJump();
        }

        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow))
        {
            StopJump();
        }
    }

    void StartJump()
    {
        if (groundChecker.IsGrounded)
        {
            _isJumping = true;
            _jumpTimeCounter = maxJumpTime;

            rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    void ContinueJump()
    {
        if (_jumpTimeCounter > 0)
        {
            rb2d.AddForce(Vector2.up * jumpForce * Time.deltaTime, ForceMode2D.Force);
            _jumpTimeCounter -= Time.deltaTime;
        }
        else
        {
            StopJump();
        }
    }

    void StopJump()
    {
        _isJumping = false;
        _jumpTimeCounter = 0;
    }
}
