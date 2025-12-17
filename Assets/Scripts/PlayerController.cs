using System;
using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform headIKTarget;

    [Header("Movement")]
    [SerializeField] private float maxMoveSpeed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravityScale = 2f;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private float jumpCooldown = 0.5f;
    [SerializeField] private float inputSmoothing = 0.1f;

    [Header("Head IK Settings")]
    [SerializeField] private float ikTargetDistance = 3f;
    [SerializeField] private float ikTargetHeight = 1f;
    [SerializeField] private float ikTargetSmoothSpeed = 5f;
    [SerializeField] private float cameraInfluence = 0.7f; // Kameranın etki oranı (0-1)
    [SerializeField] private float movementInfluence = 0.3f; // Hareketin etki oranı (0-1)

    private Vector3 moveDirection = Vector3.zero;
    private Vector3 smoothMoveDirection = Vector3.zero;
    private Vector3 currentVelocity = Vector3.zero;
    private float currentSpeed = 0f;
    private bool isMoving = false;
    private bool isSprinting = false;
    private bool isGrounded = false;
    private float jumpCooldownTimer = 0f;
    private bool isBoneInteracted = false;
    private float boneInteractionTimer = 0f;
    
    private Vector3 ikTargetPosition;

    public static PlayerController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void Start()
    {
        playerRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        playerRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        
        if (headIKTarget != null)
        {
            ikTargetPosition = transform.position + transform.forward * ikTargetDistance + Vector3.up * ikTargetHeight;
            headIKTarget.position = ikTargetPosition;
        }
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
        {
            Vector3 cameraForward = cameraTransform.forward;
            Vector3 cameraRight = cameraTransform.right;
            
            cameraForward.y = 0f;
            cameraRight.y = 0f;

            moveDirection = (cameraForward.normalized * vertical + cameraRight.normalized * horizontal).normalized;
            smoothMoveDirection = Vector3.SmoothDamp(smoothMoveDirection, moveDirection, ref currentVelocity, inputSmoothing);
            
            isMoving = true;
        }
        else
        {
            smoothMoveDirection = Vector3.SmoothDamp(smoothMoveDirection, Vector3.zero, ref currentVelocity, inputSmoothing);
            isMoving = false;
        }

        isSprinting = Input.GetKey(KeyCode.LeftShift);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && jumpCooldownTimer <= 0f)
        {
            Jump();
            jumpCooldownTimer = jumpCooldown;
        }
        
        if (jumpCooldownTimer > 0f)
        {
            jumpCooldownTimer -= Time.deltaTime;
        }

        playerAnimator.SetFloat("Speed", currentSpeed);
        playerAnimator.SetBool("isGrounded", isGrounded);
    }
    
    private void FixedUpdate()
    {
        CheckGroundStatus();
        
        if (isMoving)
        {
            RotateTowardsMovement();
            MoveCharacter();
        }
        else
        {
            if (isGrounded)
            {
                SlowDown();
            }
        }

        if (!isGrounded)
        {
            playerRigidbody.AddForce(Vector3.down * gravityScale, ForceMode.Acceleration);
        }
        else
        {
            Vector3 velocity = playerRigidbody.linearVelocity;
            velocity.y = Mathf.Max(velocity.y, -5f);
            playerRigidbody.linearVelocity = velocity;
        }
    }

    private void LateUpdate()
    {
        UpdateHeadIKTarget();
    }

    private void RotateTowardsMovement()
    {
        if (smoothMoveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(smoothMoveDirection);
            playerRigidbody.transform.rotation = Quaternion.Slerp(
                playerRigidbody.transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.fixedDeltaTime
            );
        }
    }

    private void MoveCharacter()
    {
        float targetSpeed = maxMoveSpeed * (isSprinting ? sprintMultiplier : 1f);
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);

        Vector3 movement = smoothMoveDirection * currentSpeed * Time.fixedDeltaTime;
        playerRigidbody.MovePosition(playerRigidbody.position + movement);
    }

    private void SlowDown()
    {
        if (currentSpeed > 0.01f)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.fixedDeltaTime);
            Vector3 movement = playerRigidbody.transform.forward * currentSpeed * Time.fixedDeltaTime;
            playerRigidbody.MovePosition(playerRigidbody.position + movement);
        }
        else
        {
            currentSpeed = 0f;
            Vector3 velocity = playerRigidbody.linearVelocity;
            velocity.x = 0f;
            velocity.z = 0f;
            playerRigidbody.linearVelocity = velocity;
        }
    }

    private void Jump()
    {
        playerAnimator.SetTrigger("Jump");
        Vector3 forwardMovement = playerRigidbody.transform.forward * 2f;
        playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        if (isGrounded)
        {
            playerRigidbody.AddForce(forwardMovement, ForceMode.Impulse);
        }
    }

    private void CheckGroundStatus()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            playerAnimator.ResetTrigger("Jump");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bone"))
        {
            sprintMultiplier = 4.4f;
            boneInteractionTimer = 5f;
            isBoneInteracted = true;
            Destroy(other.gameObject);
        }
    }
    private void UpdateHeadIKTarget()
    {
        if (headIKTarget == null) return;

        // Kameranın baktığı yön (yatay)
        Vector3 cameraLookDirection = cameraTransform.forward;
        cameraLookDirection.y = 0f;
        cameraLookDirection.Normalize();

        Vector3 targetPosition;

        if (isMoving && smoothMoveDirection.magnitude > 0.1f)
        {
            // Hareket ederken: hem hareket yönü hem kamera yönünü karıştır
            Vector3 movementDirection = smoothMoveDirection.normalized;
            Vector3 blendedDirection = (movementDirection * movementInfluence + cameraLookDirection * cameraInfluence).normalized;
            
            targetPosition = transform.position + blendedDirection * ikTargetDistance + Vector3.up * ikTargetHeight;
        }
        else
        {
            // Duruyorken: sadece kamera yönüne bak
            targetPosition = transform.position + cameraLookDirection * ikTargetDistance + Vector3.up * ikTargetHeight;
        }

        // Smooth geçiş
        ikTargetPosition = Vector3.Lerp(ikTargetPosition, targetPosition, ikTargetSmoothSpeed * Time.deltaTime);
        headIKTarget.position = ikTargetPosition;
    }
}