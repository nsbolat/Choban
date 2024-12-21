using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody player1Rigidbody; // Player 1 Rigidbody
    [SerializeField] private Rigidbody player2Rigidbody; // Player 2 Rigidbody

    [SerializeField] private float maxMoveSpeed = 5f; // Maksimum hız
    [SerializeField] private float acceleration = 2f; // Hızlanma
    [SerializeField] private float deceleration = 5f; // Yavaşlama
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody activePlayer; // Şu an kontrol edilen oyuncu
    private Vector3 targetPosition;
    private float currentSpeed = 0f; // Mevcut hız
    private bool isMoving = false;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Sol tık ile Player 2'yi seç
        {
            activePlayer = player2Rigidbody;
        }
        else if (Input.GetMouseButtonDown(1)) // Sağ tık ile Player 1'i seç
        {
            activePlayer = player1Rigidbody;
        }

        if (Input.GetMouseButton(0) || Input.GetMouseButton(1)) // Sol veya sağ tık basılıyken hareket
        {
            RotateTowardsMouse();
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
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
        if (activePlayer == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            targetPosition = hit.point;
            Vector3 direction = (targetPosition - activePlayer.position).normalized;
            direction.y = 0; // Yalnızca yatay eksende dönüş

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                activePlayer.transform.rotation = Quaternion.Slerp(activePlayer.transform.rotation, targetRotation, Time.deltaTime * 10f);
            }
        }
    }

    private void MoveTowardsTarget()
    {
        if (activePlayer == null) return;

        Vector3 moveDirection = (targetPosition - activePlayer.position).normalized;
        moveDirection.y = 0; // Dikey hareketi önle

        // Mevcut hızı yavaş yavaş artır
        currentSpeed = Mathf.MoveTowards(currentSpeed, maxMoveSpeed, acceleration * Time.fixedDeltaTime);

        activePlayer.MovePosition(activePlayer.position + moveDirection * currentSpeed * Time.fixedDeltaTime);
    }

    private void SlowDown()
    {
        if (activePlayer == null) return;

        if (currentSpeed > 0)
        {
            // Hızı yavaş yavaş azalt
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.fixedDeltaTime);
            activePlayer.MovePosition(activePlayer.position + activePlayer.transform.forward * currentSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // Hız sıfırsa tamamen dur
            activePlayer.velocity = Vector3.zero;
        }
    }
}
