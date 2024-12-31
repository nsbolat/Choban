using UnityEngine;

public class DogHavlamUi : MonoBehaviour
{
    public GameObject targetObject; // Kameraya dönmesi gereken GameObject
    public Camera mainCamera; // Kamerayı tanımlayın

    private void Awake()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(true);
        }
    }

    private void LateUpdate()
    {
        // Eğer kamera atanmadıysa, ana kamerayı kullan
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (targetObject != null && mainCamera != null)
        {
            // GameObject'in rotasyonunu kameraya bakacak şekilde ayarla
            Vector3 cameraForward = mainCamera.transform.forward;
            targetObject.transform.rotation = Quaternion.LookRotation(cameraForward, Vector3.up);
        }
    }
}