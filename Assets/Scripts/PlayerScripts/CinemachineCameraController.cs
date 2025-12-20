using UnityEngine;
using Unity.Cinemachine;

public class CinemachineCameraController : MonoBehaviour
{
    [Header("Cinemachine References")]
    [SerializeField] private CinemachineCamera virtualCamera;
    [SerializeField] private Transform cameraFollowTarget;

    [Header("Camera Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float minVerticalAngle = 10f;
    [SerializeField] private float maxVerticalAngle = 80f;
    [SerializeField] private float smoothSpeed = 10f;

    [Header("Camera Distance")]
    [SerializeField] private float cameraDistance = 8f;
    [SerializeField] private float cameraHeight = 4f;

    [Header("Dynamic FOV")]
    [SerializeField] private float baseFOV = 40f;
    [SerializeField] private float sprintFOV = 60f;
    [SerializeField] private float fovSmoothTime = 5f;

    private float currentYaw;
    private float currentPitch = 30f;
    private float targetYaw;
    private float targetPitch = 30f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraFollowTarget != null)
        {
            currentYaw = cameraFollowTarget.eulerAngles.y;
            targetYaw = currentYaw;
        }
        
        // Başlangıç FOV ayarla
        if (virtualCamera != null)
            virtualCamera.Lens.FieldOfView = baseFOV;
    }

    private void FixedUpdate()
    {
        HandleMouseInput();
        HandleCursorLock();
    }
    
    private void Update()
    {
        HandleDynamicFOV();
    }

    private void LateUpdate()
    {
        if (cameraFollowTarget == null) return;
        UpdateCameraPosition();
    }

    private void HandleDynamicFOV()
    {
        if (virtualCamera == null || PlayerController.Instance == null) return;

        // Player hızına göre hedef FOV belirleme
        // SpeedRatio: 0 (Duruyor) -> 1 (Yürüyor) -> >1 (Koşuyor)
        float ratio = PlayerController.Instance.SpeedRatio;
        
        // Sadece 1'den hızlıysa (koşuyorsa) FOV artır
        // maxMoveSpeed=3, sprintMultiplier=2.5 => Max SpeedRatio = 2.5
        // t = (ratio - 1) / (2.5 - 1) = (ratio - 1) / 1.5
        
        float t = Mathf.Clamp01((ratio - 1f) / 1.5f);
        float targetFOV = Mathf.Lerp(baseFOV, sprintFOV, t);

        virtualCamera.Lens.FieldOfView = Mathf.Lerp(virtualCamera.Lens.FieldOfView, targetFOV, fovSmoothTime * Time.deltaTime);
    }

    private void HandleMouseInput()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        targetYaw += mouseX;
        targetPitch -= mouseY;
        targetPitch = Mathf.Clamp(targetPitch, minVerticalAngle, maxVerticalAngle);

        currentYaw = Mathf.Lerp(currentYaw, targetYaw, smoothSpeed * Time.deltaTime);
        currentPitch = Mathf.Lerp(currentPitch, targetPitch, smoothSpeed * Time.deltaTime);
    }

    private void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);

        // 🔥 HEIGHT & DISTANCE GERÇEKTEN KULLANILIYOR
        Vector3 offset = rotation * new Vector3(0f, cameraHeight, -cameraDistance);
        Vector3 targetPosition = transform.position + offset;

        cameraFollowTarget.position = targetPosition;
        cameraFollowTarget.rotation = rotation;
    }

    private void HandleCursorLock()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Cursor.lockState == CursorLockMode.None && Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
