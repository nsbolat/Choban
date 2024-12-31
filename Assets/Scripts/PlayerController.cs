using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody playerRigidbody; // Oyuncu Rigidbody
    [SerializeField] private Animator playerAnimator; // Animator bileşeni
    [SerializeField] private Transform groundCheck; // Zemin kontrol noktası
    [SerializeField] private GameObject _infoObject; // Hedef işareti
    [SerializeField] private LayerMask groundLayer; // Zemin Layer'ı

    [Header("Movement")]
    [SerializeField] private float maxMoveSpeed = 5f; // Maksimum hız
    [SerializeField] private float acceleration = 2f; // Hızlanma
    [SerializeField] private float deceleration = 5f; // Yavaşlama
    [SerializeField] private float sprintMultiplier = 1.5f; // Sprint çarpanı
    [SerializeField] private AnimationCurve rotationSpeedCurve; // Rotasyon eğrisi
    [SerializeField] private float rotationTime = 1f; // Rotasyon süresi
    [SerializeField] private float jumpForce = 5f; // Zıplama gücü
    [SerializeField] private float gravityScale = 2f; // Yerçekimi çarpanı
    [SerializeField] private float groundCheckRadius = 0.2f; // Zemin kontrol yarıçapı
    [SerializeField] private float jumpCooldown = 0.5f; // Zıplama cooldown süresi

    private Vector3 targetPosition;
    private float currentSpeed = 0f; // Mevcut hız
    private bool isMoving = false;
    private bool isSprinting = false; // Sprint durumu
    private bool isGrounded = false; // Zeminde olup olmadığını kontrol eder
    private float rotationProgress = 0f; // Rotasyon ilerlemesi
    private float jumpCooldownTimer = 0f; // Zıplama cooldown timer'ı

    

    private bool isBoneInteracted = false; // Kemik ile etkileşim durumunu takip eder
    private float boneInteractionTimer = 0f; // Kemik ile etkileşim süresi


    public static PlayerController Instance { get; private set; } // Singleton

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Instance bu scriptin ilk oluşturulan örneği olur
        }
        else
        {
            Destroy(gameObject); // Eğer zaten bir Instance varsa, yenisini yok ederiz
        }
    }

    private void Update()
    {
        isSprinting = Input.GetKey(KeyCode.LeftShift); // Sprint için
        cursorInfo();

        if (Input.GetMouseButton(0)) // Sol tık basılıyken hareket
        {
            RotateTowardsMouse();
            isMoving = true; // Hareket etmeye başla
        }
        else
        {
            isMoving = false; // Hareket etmeyi durdur
            rotationProgress = 0f; // Hareket bitince rotasyon sıfırlanır
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && jumpCooldownTimer <= 0f)
        {
            Jump();
            jumpCooldownTimer = jumpCooldown; // Zıplama cooldown'ını başlat
        }

        if (jumpCooldownTimer > 0f)
        {
            jumpCooldownTimer -= Time.deltaTime;
        }

        playerAnimator.SetFloat("Speed", currentSpeed);
        playerAnimator.SetBool("isGrounded", isGrounded);


        // Kemik etkileşimi varsa sprintMultiplier'ı artır
        if (isBoneInteracted)
        {
            boneInteractionTimer -= Time.deltaTime;

            if (boneInteractionTimer <= 0f)
            {
                // 5 saniye geçti, etkisini sona erdir
                sprintMultiplier = 2.2f;
                isBoneInteracted = false;
            }
        }
    }

    private void FixedUpdate()
    {
        CheckGroundStatus();

        if (isMoving)
        {
            MoveTowardsTarget();
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
    }

    private void cursorInfo()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            Vector3 targetPosition = hit.point;
            _infoObject.transform.position = targetPosition;
        }
    }

    private void RotateTowardsMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            targetPosition = hit.point;
            Vector3 direction = (targetPosition - playerRigidbody.position).normalized;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                rotationProgress = Mathf.Clamp01(rotationProgress + Time.deltaTime / rotationTime);
                float curveValue = rotationSpeedCurve.Evaluate(rotationProgress);

                playerRigidbody.transform.rotation = Quaternion.Slerp(playerRigidbody.transform.rotation, targetRotation, curveValue);
            }
        }
    }

    private void MoveTowardsTarget()
    {
        Vector3 moveDirection = (targetPosition - playerRigidbody.position).normalized;
        moveDirection.y = 0;

        float targetSpeed = maxMoveSpeed * (isSprinting ? sprintMultiplier : 1f);

        

        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);

        playerRigidbody.MovePosition(playerRigidbody.position + moveDirection * currentSpeed * Time.fixedDeltaTime);
    }

    private void SlowDown()
    {
        if (currentSpeed > 0)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.fixedDeltaTime);
            playerRigidbody.MovePosition(playerRigidbody.position + playerRigidbody.transform.forward * currentSpeed * Time.fixedDeltaTime);
        }
        else
        {
            Vector3 velocity = playerRigidbody.velocity;
            velocity.x = 0f;
            velocity.z = 0f;
            playerRigidbody.velocity = velocity;
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

    // Kemik ile etkileşime girildiğinde çağrılacak metod
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bone"))
        {
            sprintMultiplier = 4.4f; // Sprint hızını 3'e çıkar
            boneInteractionTimer = 5f; // 5 saniye süresince etki edecek
            isBoneInteracted = true;

            Destroy(other.gameObject); // Kemik yok edilir
        }
    }     
}
