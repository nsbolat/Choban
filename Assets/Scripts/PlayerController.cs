using UnityEngine;

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

        // Zıplama ve cooldown kontrolü
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && jumpCooldownTimer <= 0f) // Zıplama ve cooldown kontrolü
        {
            Jump();
            jumpCooldownTimer = jumpCooldown; // Zıplama cooldown'ını başlat
        }

        // Zıplama cooldown'ını güncelle
        if (jumpCooldownTimer > 0f)
        {
            jumpCooldownTimer -= Time.deltaTime;
        }

        // Animasyon parametrelerini güncelle
        playerAnimator.SetFloat("Speed", currentSpeed); 
        playerAnimator.SetBool("isGrounded", isGrounded);
    }

    private void FixedUpdate()
    {
        CheckGroundStatus(); // Zemin durumunu kontrol et

        // Eğer hareket ediliyorsa, sadece hareket etmeye başlandıysa yavaşlatma devreye girsin
        if (isMoving)
        {
            MoveTowardsTarget(); // Hareket et
        }
        else
        {
            // Hareket etmiyor ise, yavaşlatma işlemi yapılmasın
            if (isGrounded)
            {
                SlowDown(); // Yavaşlatma işlemi sadece yerden hareket ediyor iken çalışsın
            }
        }

        // Yerçekimi çarpanını uygula
        if (!isGrounded)
        {
            playerRigidbody.AddForce(Vector3.down * gravityScale, ForceMode.Acceleration);
        }
    }

    private void cursorInfo()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Cast the ray and check if it hits the ground layer
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            // Only proceed if the normal of the surface is facing upwards (i.e., it's the top surface)
            if (Vector3.Dot(hit.normal, Vector3.up) > 0.9f) // Check if the normal is close to the up direction
            {
                Vector3 targetPosition = hit.point;
                _infoObject.transform.position = targetPosition; // Move the cursor info object to the hit position
            }
        }
    }

    private void RotateTowardsMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Cast the ray and check if it hits the ground layer
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            // Only rotate towards the point if the normal of the surface is facing upwards (top surface)
            if (Vector3.Dot(hit.normal, Vector3.up) > 0.9f) // Check if the normal is close to the up direction
            {
                targetPosition = hit.point;
                Vector3 direction = (targetPosition - playerRigidbody.position).normalized;
                direction.y = 0; // Ignore vertical movement for rotation

                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);

                    // Apply rotation using the curve value
                    rotationProgress = Mathf.Clamp01(rotationProgress + Time.deltaTime / rotationTime);
                    float curveValue = rotationSpeedCurve.Evaluate(rotationProgress);

                    playerRigidbody.transform.rotation = Quaternion.Slerp(playerRigidbody.transform.rotation, targetRotation, curveValue);
                }
            }
        }
    }

    private void MoveTowardsTarget()
    {
        Vector3 moveDirection = (targetPosition - playerRigidbody.position).normalized;
        moveDirection.y = 0; // Dikey hareketi önle

        float targetSpeed = maxMoveSpeed * (isSprinting ? sprintMultiplier : 1f);

        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime); // Hız artır

        playerRigidbody.MovePosition(playerRigidbody.position + moveDirection * currentSpeed * Time.fixedDeltaTime);
    }

    private void SlowDown()
    {
        // Hız yavaşlatma sadece hareket ederken çalışacak
        if (currentSpeed > 0)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.fixedDeltaTime); // Hızı azalt
            playerRigidbody.MovePosition(playerRigidbody.position + playerRigidbody.transform.forward * currentSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // Hızı sıfırlama, sadece yatay ekseni sıfırla
            Vector3 velocity = playerRigidbody.velocity;
            velocity.x = 0f;
            velocity.z = 0f;
            playerRigidbody.velocity = velocity;
        }
    }

    private void Jump()
    {
        playerAnimator.SetTrigger("Jump"); // Zıplama animasyonunu tetikle

        // Zıplama kuvveti ve hareket yönü
        Vector3 forwardMovement = playerRigidbody.transform.forward * 2f; // 2f, ileriye doğru eklenen kuvvetin büyüklüğü, isteğe göre ayarlayın

        // Yukarı doğru zıplama kuvveti ekle
        playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Yukarı doğru zıplama kuvveti

        // Sadece zıplama sırasında ileriye doğru hareket kuvveti ekle
        if (isGrounded) // Sadece zıplama yaparken
        {
            playerRigidbody.AddForce(forwardMovement, ForceMode.Impulse); // Zıplama sırasında ileri doğru kuvvet uygula
        }
    }

    private void CheckGroundStatus()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer); // Zemin kontrolü

        if (isGrounded)
        {
            playerAnimator.ResetTrigger("Jump"); // Zıplama animasyonunu sıfırla
        }
    }
}
