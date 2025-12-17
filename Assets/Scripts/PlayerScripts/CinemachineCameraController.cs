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
    }

    private void FixedUpdate()
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
