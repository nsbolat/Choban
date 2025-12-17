using UnityEngine;
using Unity.Cinemachine;

public class CinemachineCameraController : MonoBehaviour
{
    [Header("Cinemachine References")]
    [SerializeField] private CinemachineCamera virtualCamera;
    [SerializeField] private Transform cameraFollowTarget; // Kameranın takip edeceği boş GameObject

    [Header("Camera Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float minVerticalAngle = 10f; // Minimum dikey açı
    [SerializeField] private float maxVerticalAngle = 80f; // Maximum dikey açı
    [SerializeField] private float smoothSpeed = 10f; // Kamera yumuşatma hızı

    [Header("Camera Distance")]
    [SerializeField] private float cameraDistance = 8f; // Kameradan hedefe uzaklık
    [SerializeField] private float cameraHeight = 4f; // Kameranın yüksekliği

    private float currentYaw = 0f; // Yatay rotasyon
    private float currentPitch = 30f; // Dikey rotasyon (başlangıç değeri)
    private float targetYaw = 0f;
    private float targetPitch = 30f;

    private void Start()
    {
        // Mouse'u kilitle (ESC ile çıkılabilir)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Başlangıç rotasyonunu ayarla
        if (cameraFollowTarget != null)
        {
            currentYaw = cameraFollowTarget.eulerAngles.y;
            currentPitch = 30f;
        }
    }

    private void Update()
    {
        HandleMouseInput();
        HandleCursorLock();
    }

    private void LateUpdate()
    {
        if (cameraFollowTarget == null) return;

        UpdateCameraPosition();
    }

    private void HandleMouseInput()
    {
        // Mouse hareketini al
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Yatay rotasyonu güncelle (sınırsız)
        targetYaw += mouseX;

        // Dikey rotasyonu güncelle (sınırlı)
        targetPitch -= mouseY;
        targetPitch = Mathf.Clamp(targetPitch, minVerticalAngle, maxVerticalAngle);

        // Yumuşak geçiş
        currentYaw = Mathf.Lerp(currentYaw, targetYaw, smoothSpeed * Time.deltaTime);
        currentPitch = Mathf.Lerp(currentPitch, targetPitch, smoothSpeed * Time.deltaTime);
    }

    private void UpdateCameraPosition()
    {
        // Kamera hedef pozisyonunu hesapla
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
        
        // Kameranın arkadan ve yukarıdan bakacağı pozisyonu hesapla
        Vector3 offset = rotation * new Vector3(0f, cameraHeight, -cameraDistance);
        
        cameraFollowTarget.position = transform.position;
        cameraFollowTarget.rotation = rotation;
    }

    private void HandleCursorLock()
    {
        // ESC tuşu ile mouse'u serbest bırak
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Ekrana tıklayınca tekrar kilitle
        if (Cursor.lockState == CursorLockMode.None && Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Inspector'da ayarları görselleştirmek için
    private void OnDrawGizmosSelected()
    {
        if (cameraFollowTarget == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, cameraFollowTarget.position);
    }
}