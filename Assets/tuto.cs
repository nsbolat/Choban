using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tuto : MonoBehaviour
{
    public GameObject panel; // UI Panelini atamak için
    private bool isPlayerInside = false;

    void Start()
    {
        panel.SetActive(false); // Başlangıçta paneli kapalı tut
    }

    void Update()
    {
        if (isPlayerInside && Input.GetKeyDown(KeyCode.Z))
        {
            panel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Köpek")) // Oyuncu içeri girdiğinde
        {
            isPlayerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Köpek")) // Oyuncu çıktığında
        {
            isPlayerInside = false;
        }
    }

    public void ClosePanel()
    {
        panel.SetActive(false);            Time.timeScale = 1f;

    }
}