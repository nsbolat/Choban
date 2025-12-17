using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform headIKTarget;

    [Header("Movement")]
    [SerializeField] private float maxMoveSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 12f;
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private float gravityMultiplier = 2.5f;
    [SerializeField] private float inputSmoothTime = 0.1f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float jumpGroundDisableTime = 0.15f;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 0.45f;

    [Header("Slope")]
    [SerializeField] private float maxSlopeAngle = 55f;
    [SerializeField] private float slopeSpeedMultiplier = 0.6f;
    [SerializeField] private float stickToGroundForce = 15f;

    [Header("Head IK")]
    [SerializeField] private float ikTargetDistance = 3f;
    [SerializeField] private float ikTargetHeight = 1.2f;
    [SerializeField] private float ikSmoothSpeed = 6f;
    
    [Header("Head IK Limits")]
    [SerializeField] private float maxHeadTurnAngle = 70f;   // derece
    [SerializeField] private float headIKDisableAngle = 100f;

    private Vector3 moveInput;
    private Vector3 smoothMoveDir;
    private Vector3 smoothVelocity;

    private float currentSpeed;
    private bool isGrounded;
    private bool isSprinting;
    private bool isJumping;

    private float jumpGroundTimer;
    private RaycastHit groundHit;
    private Vector3 ikTargetPos;

    private void Awake()
    {
        Instance = this;
        if (!cameraTransform)
            cameraTransform = Camera.main.transform;
    }

    private void Start()
    {
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        if (headIKTarget)
            ikTargetPos = headIKTarget.position;
    }

    private void Update()
    {
        ReadInput();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        CheckGround();
        HandleMovement();
        StickToGround();
        ApplyExtraGravity();
        RotateCharacter();
    }

    private void LateUpdate()
    {
        UpdateHeadIK();
    }

    private void ReadInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;

        moveInput = (camForward.normalized * v + camRight.normalized * h).normalized;

        smoothMoveDir = Vector3.SmoothDamp(
            smoothMoveDir,
            moveInput,
            ref smoothVelocity,
            inputSmoothTime
        );

        isSprinting = Input.GetKey(KeyCode.LeftShift);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isJumping)
            Jump();
    }

    private void HandleMovement()
    {
        if (smoothMoveDir.magnitude < 0.05f)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.fixedDeltaTime);
            return;
        }

        float targetSpeed = maxMoveSpeed * (isSprinting ? sprintMultiplier : 1f);
        targetSpeed *= GetSlopeSpeedMultiplier();

        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);

        Vector3 moveDir = smoothMoveDir;

        if (isGrounded)
            moveDir = Vector3.ProjectOnPlane(moveDir, groundHit.normal).normalized;

        Vector3 movement = moveDir * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
    }

    private void RotateCharacter()
    {
        if (smoothMoveDir.magnitude < 0.1f) return;

        Vector3 dir = smoothMoveDir;
        if (isGrounded)
            dir = Vector3.ProjectOnPlane(dir, groundHit.normal);

        Quaternion targetRot = Quaternion.LookRotation(dir);
        rb.rotation = Quaternion.Slerp(rb.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime);
    }

    private void Jump()
    {
        isJumping = true;
        jumpGroundTimer = jumpGroundDisableTime;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        animator.SetTrigger("Jump");
    }

    private void CheckGround()
    {
        if (jumpGroundTimer > 0f)
        {
            jumpGroundTimer -= Time.fixedDeltaTime;
            isGrounded = false;
            return;
        }

        isGrounded = Physics.Raycast(
            groundCheck.position,
            Vector3.down,
            out groundHit,
            groundCheckDistance,
            groundLayer
        );

        if (isGrounded)
            isJumping = false;
    }

    private float GetSlopeSpeedMultiplier()
    {
        if (!isGrounded) return 1f;

        float slopeAngle = Vector3.Angle(groundHit.normal, Vector3.up);
        return Mathf.Lerp(1f, slopeSpeedMultiplier, slopeAngle / maxSlopeAngle);
    }

    private void StickToGround()
    {
        if (!isGrounded || isJumping) return;

        rb.AddForce(-groundHit.normal * stickToGroundForce, ForceMode.Acceleration);
    }

    private void ApplyExtraGravity()
    {
        if (!isGrounded)
            rb.AddForce(Vector3.down * gravityMultiplier, ForceMode.Acceleration);
    }

    private void UpdateAnimator()
    {
        animator.SetFloat("Speed", currentSpeed);
        animator.SetBool("isGrounded", isGrounded);
    }

    private void UpdateHeadIK()
    {
        if (!headIKTarget) return;

        Vector3 camDir = cameraTransform.forward;
        camDir.y = 0f;
        camDir.Normalize();

        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();

        float angleToCamera = Vector3.Angle(forward, camDir);

        // Kamera arkadaysa → IK kapat
        if (angleToCamera > headIKDisableAngle)
        {
            // IK hedefini öne doğru sabitle
            Vector3 forwardTarget =
                transform.position +
                forward * ikTargetDistance +
                Vector3.up * ikTargetHeight;

            ikTargetPos = Vector3.Lerp(
                ikTargetPos,
                forwardTarget,
                ikSmoothSpeed * Time.deltaTime
            );

            headIKTarget.position = ikTargetPos;
            return;
        }

        // Yumuşak sınır (açı kısıtlama)
        float t = Mathf.InverseLerp(maxHeadTurnAngle, headIKDisableAngle, angleToCamera);
        t = Mathf.Clamp01(1f - t);

        Vector3 blendedDir = Vector3.Slerp(forward, camDir, t);

        Vector3 targetPos =
            transform.position +
            blendedDir * ikTargetDistance +
            Vector3.up * ikTargetHeight;

        ikTargetPos = Vector3.Lerp(
            ikTargetPos,
            targetPos,
            ikSmoothSpeed * Time.deltaTime
        );

        headIKTarget.position = ikTargetPos;
    }
}
