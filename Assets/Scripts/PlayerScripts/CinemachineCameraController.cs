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

    // PlayerController referansını dinamik alacağız
    private PlayerController attachedPlayer;

    private void Awake()
    {
        attachedPlayer = GetComponent<PlayerController>();
    }

    private void Start()
    {
        // Eğer bu script local oyuncuda çalışıyorsa, cursor'ı kilitle
        // Bunu PlayerController'dan tetiklemek daha güvenli olabilir ama burada da durabilir.
        // Ancak Network ortamında sadece "aktif" olduğunda çalışmalı.
        if (isActiveAndEnabled)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (cameraFollowTarget != null)
        {
            currentYaw = cameraFollowTarget.eulerAngles.y;
            targetYaw = currentYaw;
        }
        
        // Sanal kamera atanmamışsa sahnede ara (Main Camera üzerindeki Brain'den değil, CinemachineCamera arıyoruz)
        if (virtualCamera == null)
        {
            virtualCamera = Object.FindFirstObjectByType<CinemachineCamera>();
        }

        if (virtualCamera != null)
        {
            virtualCamera.Lens.FieldOfView = baseFOV;
            // Kameranın Follow hedefini bu objenin target'ı yap
            if (cameraFollowTarget)
                virtualCamera.Follow = cameraFollowTarget;
        }
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
        if (virtualCamera == null || attachedPlayer == null) return;

        // Player hızına göre hedef FOV belirleme
        float ratio = attachedPlayer.SpeedRatio;
        
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

        Vector3 offset = rotation * new Vector3(0f, cameraHeight, -cameraDistance);
        Vector3 targetPosition = transform.position + offset;

        cameraFollowTarget.position = targetPosition;
        cameraFollowTarget.rotation = rotation;
    }

    private void HandleCursorLock()
    {
        // Sadece local oyuncu için çalışmalı (bu script sadece localde enable olacak)
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
    
    // Dışarıdan kamerayı set etmek istersek
    public void SetVirtualCamera(CinemachineCamera cam)
    {
        virtualCamera = cam;
        if (virtualCamera != null && cameraFollowTarget != null)
        {
            virtualCamera.Follow = cameraFollowTarget;
        }
    }

    public GameObject DetachReferenceTarget()
    {
        if (cameraFollowTarget != null)
        {
            cameraFollowTarget.SetParent(null);
            return cameraFollowTarget.gameObject;
        }
        return null;
    }
}
