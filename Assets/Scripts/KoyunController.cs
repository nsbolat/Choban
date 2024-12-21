using UnityEngine;

public class KoyunController : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float maxMoveSpeed = 5f; // Maksimum hız
    [SerializeField] private float acceleration = 2f; // Hızlanma
    [SerializeField] private float deceleration = 5f; // Yavaşlama
    [SerializeField] private LayerMask groundLayer;

    private Vector3 targetPosition;
    private float currentSpeed = 0f; // Mevcut hız
    private bool isMoving = false;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0)) // Sağ tık basılıyken hareket
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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            targetPosition = hit.point;
            Vector3 direction = (targetPosition - transform.position).normalized;
            direction.y = 0; // Yalnızca yatay eksende dönüş

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }
        }
    }

    private void MoveTowardsTarget()
    {
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        moveDirection.y = 0; // Dikey hareketi önle

        // Mevcut hızı yavaş yavaş artır
        currentSpeed = Mathf.MoveTowards(currentSpeed, maxMoveSpeed, acceleration * Time.fixedDeltaTime);

        _rigidbody.MovePosition(transform.position + moveDirection * currentSpeed * Time.fixedDeltaTime);
    }

    private void SlowDown()
    {
        if (currentSpeed > 0)
        {
            // Hızı yavaş yavaş azalt
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.fixedDeltaTime);
            _rigidbody.MovePosition(transform.position + transform.forward * currentSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // Hız sıfırsa tamamen dur
            _rigidbody.velocity = Vector3.zero;
        }
    }
}
