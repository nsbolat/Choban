using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    // Singleton Instance
    public static PlayerController Instance;

    [Header("Bileşenler")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform headIKTarget;

    [Header("Movement")]
    [SerializeField] private float maxMoveSpeed = 3f;
    [SerializeField] private float sprintMultiplier = 2.5f;
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

    [Header("Tilt (Banking)")]
    [SerializeField] private float tiltAngle = 10f; // Maksimum yatma açısı
    [SerializeField] private float tiltSpeed = 8f;  // Yatma hızı

    [Header("Head IK")]
    [SerializeField] private float ikTargetDistance = 3f;
    [SerializeField] private float ikTargetHeight = 1.2f;
    [SerializeField] private float ikSmoothSpeed = 6f;

    [Header("Head IK Limits")]
    [SerializeField] private float maxHeadTurnAngle = 70f;
    [SerializeField] private float headIKDisableAngle = 100f;

    // Özel Değişkenler
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

    // Public Properties
    public float CurrentSpeed => currentSpeed;
    public float MaxMoveSpeed => maxMoveSpeed;
    public float SpeedRatio => currentSpeed / maxMoveSpeed; // 0-1 arası (Koşarken >1 olabilir)

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

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
        // 1. Hedef Hızı Belirle
        float targetSpeed = 0f;
        
        // Input var mı? (Gürültü toleransı 0.05f)
        if (smoothMoveDir.magnitude > 0.05f)
        {
            // Input var, hedef hızı hesapla
            targetSpeed = maxMoveSpeed * (isSprinting ? sprintMultiplier : 1f);
            targetSpeed *= GetSlopeSpeedMultiplier();
            
            // Hızlanma
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            // Input yok, yavaşla (Deceleration)
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.fixedDeltaTime);
        }

        // Hız çok düşükse hareketi tamamen kes
        if (currentSpeed < 0.01f)
        {
            currentSpeed = 0f;
            return;
        }

        // 2. Hareket Yönünü Belirle
        // Eğer input varsa input yönünü kullan, yoksa karakterin baktığı yönü (momentum) kullan
        Vector3 moveDir = smoothMoveDir.magnitude > 0.05f ? smoothMoveDir.normalized : transform.forward;

        if (isGrounded)
            moveDir = Vector3.ProjectOnPlane(moveDir, groundHit.normal).normalized;

        // 3. Hareketi Uygula
        Vector3 movement = moveDir * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
    }

    private void RotateCharacter()
    {
        if (smoothMoveDir.magnitude < 0.1f) 
        {
            // Dururken tilt'i sıfırla
            if (transform.rotation.z != 0)
            {
                Quaternion upright = Quaternion.Euler(0, transform.eulerAngles.y, 0);
                rb.rotation = Quaternion.Slerp(rb.rotation, upright, tiltSpeed * Time.fixedDeltaTime);
            }
            return;
        }

        Vector3 dir = smoothMoveDir;
        if (isGrounded)
            dir = Vector3.ProjectOnPlane(dir, groundHit.normal);

        // Hedef Yön (Y Ekseninde)
        Quaternion targetLookRot = Quaternion.LookRotation(dir);

        // Tilt Hesaplama
        // Hareket vektörü ile karakterin sağ vektörü arasındaki ilişkiyi kullanarak dönüş yönünü buluyoruz
        // Ancak daha basit ve stabil yöntem: Input ve Hıza dayalı tilt
        
        float turnAmount = Vector3.SignedAngle(transform.forward, dir, Vector3.up);
        float targetTiltZ = 0f;

        // Sadece hareket halindeyken tilt yap
        // Dönüş açısı çok keskinse daha çok yat
        if (Mathf.Abs(turnAmount) > 1f) 
        {
            // turnAmount pozitif (sağa dönüş) -> Z negatif (sağa yatış) olmalı
            float leanFactor = Mathf.Clamp(turnAmount / 45f, -1f, 1f); 
            targetTiltZ = -leanFactor * tiltAngle * Mathf.Clamp01(SpeedRatio); 
        }

        // Mevcut rotasyonun Y ve X bileşenlerini koru, Z'yi tilt ile değiştir
        Quaternion tiltRot = Quaternion.Euler(0, 0, targetTiltZ);
        
        // Final Rotasyon: Hedef Yön * Tilt
        // Önce Y ekseninde dön, sonra Local Z'de yat
        Quaternion finalRot = targetLookRot * tiltRot;

        rb.rotation = Quaternion.Slerp(rb.rotation, finalRot, rotationSpeed * Time.fixedDeltaTime);
    }

    private void Jump()
    {
        isJumping = true;
        jumpGroundTimer = jumpGroundDisableTime;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // Hızı sıfırla
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
        // interaction kodlarından kalan 'IsPushing' parametresini sildim
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

        if (angleToCamera > headIKDisableAngle)
        {
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