using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraControls : MonoBehaviour
{
    public CinemachineVirtualCamera cinemachineCamera; // Cinemachine kameranızı buraya referans verin
    public float zoomOutSize = 10f; // Zoom out olduğunda ulaşacağı boyut
    public float defaultSize = 5f; // Varsayılan boyut
    public float zoomSpeed = 5f; // Zoom hızını ayarlayın

    private CinemachineVirtualCamera virtualCamera;
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
        // Tab tuşuna basılı tutulduğunda zoom out
        if (Input.GetKey(KeyCode.Tab))
        {
            isZoomingOut = true;
        }
        else
        {
            isZoomingOut = false;
        }

        // Kameranın orthographic size değerini değiştirme
        float targetSize = isZoomingOut ? zoomOutSize : defaultSize;
        cinemachineCamera.m_Lens.OrthographicSize = Mathf.Lerp(
            cinemachineCamera.m_Lens.OrthographicSize, 
            targetSize, 
            Time.deltaTime * zoomSpeed
        );
    }
}