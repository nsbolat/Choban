using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraControls : MonoBehaviour
{
    public CinemachineVirtualCamera cinemachineCamera; // Reference to your Cinemachine camera
    public float zoomOutFOV = 60f; // Field of View when zoomed out
    public float defaultFOV = 40f; // Default Field of View
    public float zoomSpeed = 5f; // Speed of zooming

    private bool isZoomingOut = false;

    void Start()
    {
        if (cinemachineCamera == null)
        {
            Debug.LogError("Cinemachine Camera is not assigned!");
        }
    }

    void Update()
    {
        // Zoom out while holding the Tab key
        if (Input.GetKey(KeyCode.Tab))
        {
            isZoomingOut = true;
        }
        else
        {
            isZoomingOut = false;
        }

        // Change the camera's Field of View
        float targetFOV = isZoomingOut ? zoomOutFOV : defaultFOV;
        cinemachineCamera.m_Lens.FieldOfView = Mathf.Lerp(
            cinemachineCamera.m_Lens.FieldOfView,
            targetFOV,
            Time.deltaTime * zoomSpeed
        );
    }
}