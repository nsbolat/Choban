using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody playerRigidbody; // Oyuncu Rigidbody
    [SerializeField] private Animator playerAnimator; // Animator bileşeni

    [SerializeField] private float maxMoveSpeed = 5f; // Maksimum hız
    [SerializeField] private float acceleration = 2f; // Hızlanma
    [SerializeField] private float deceleration = 5f; // Yavaşlama
    [SerializeField] private float sprintMultiplier = 1.5f; // Sprint çarpanı
    [SerializeField] private LayerMask groundLayer;

    private Vector3 targetPosition;
    private float currentSpeed = 0f; // Mevcut hız
    private bool isMoving = false;
    private bool isSprinting = false; // Sprint durumu

    private void Update()
    {
        isSprinting = Input.GetKey(KeyCode.LeftShift); // Shift'e basılıyken sprint

        if (Input.GetMouseButton(0)) // Sol tık basılıyken hareket
        {
            RotateTowardsMouse();
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        // Animator'deki speed değerini güncelle
        playerAnimator.SetFloat("Speed", currentSpeed);
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            MoveTowardsTarget();
        }
        else
        {
            SlowDown();
        }
    }

    private void RotateTowardsMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            targetPosition = hit.point;
            Vector3 direction = (targetPosition - playerRigidbody.position).normalized;
            direction.y = 0; // Yalnızca yatay eksende dönüş

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                playerRigidbody.transform.rotation = Quaternion.Slerp(playerRigidbody.transform.rotation, targetRotation, Time.deltaTime * 10f);
            }
        }
    }

    private void MoveTowardsTarget()
    {
        Vector3 moveDirection = (targetPosition - playerRigidbody.position).normalized;
        moveDirection.y = 0; // Dikey hareketi önle

        float targetSpeed = maxMoveSpeed * (isSprinting ? sprintMultiplier : 1f);

        // Mevcut hızı yavaş yavaş artır
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);

        playerRigidbody.MovePosition(playerRigidbody.position + moveDirection * currentSpeed * Time.fixedDeltaTime);
    }

    private void SlowDown()
    {
        if (currentSpeed > 0)
        {
            // Hızı yavaş yavaş azalt
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.fixedDeltaTime);
            playerRigidbody.MovePosition(playerRigidbody.position + playerRigidbody.transform.forward * currentSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // Hız sıfırsa tamamen dur
            playerRigidbody.velocity = Vector3.zero;
        }
    }
}
