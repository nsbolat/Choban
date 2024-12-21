using UnityEngine;

public class MultiObjectController : MonoBehaviour
{
    public GameObject player1; // İlk obje (sağ tık ile kontrol edilecek)
    public GameObject player2; // İkinci obje (Alt + sağ tık ile kontrol edilecek)
    public float moveSpeed = 5f; // Hareket hızı
    public LayerMask groundLayer; // Mouse'un etkileşeceği zemin katmanı

    private GameObject currentTarget; // Hangi obje hareket edecek
    private Vector3 targetPosition; // Hedef pozisyonu

    void Update()
    {
        // Sağ tıklamaya basıldığında
        if (Input.GetMouseButton(1))
        {
            // Alt tuşuna basılıysa ikinci objeyi kontrol et
            if (Input.GetKey(KeyCode.LeftAlt))
            {
                currentTarget = player2;
            }
            else
            {
                currentTarget = player1;
            }

            UpdateTargetPosition(); // Mouse hedefini güncelle
        }

        // Hareketi gerçekleştir
        if (currentTarget != null)
        {
            MoveAndRotate(currentTarget);
        }
    }

    // Mouse'un olduğu dünya pozisyonunu hesapla
    void UpdateTargetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            targetPosition = hit.point;
        }
    }

    // Objeyi hareket ettir ve döndür
    void MoveAndRotate(GameObject obj)
    {
        // Hedef pozisyon ile mevcut pozisyon arasındaki yönü hesapla
        Vector3 direction = (targetPosition - obj.transform.position).normalized;
        direction.y = 0; // Y eksenindeki farkı sıfırla (isometric için)

        // Objeyi hedefe doğru döndür
        if (direction != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            obj.transform.rotation = Quaternion.Lerp(obj.transform.rotation, toRotation, Time.deltaTime * 10f);
        }

        // Objeyi hareket ettir
        obj.transform.position = Vector3.MoveTowards(obj.transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }
}
