using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BarnMechanic : MonoBehaviour
{
    public GameObject fadeScreen; // Ekran kararması için siyah ekran UI.
    public GameObject statusTextObject;       // "Dinleniyor..." yazısını göstermek için UI Text.
    public TextMeshProUGUI dayCounterText;
    public float fadeDuration = 5f; // Siyah ekran süresi.
    
    private bool isResting = false; // Dinlenme kontrolü.
    private int dayCount = 1;       // Gün sayacı.
    private int sheepCount = 1;    // Başlangıç koyun sayısı.
    
    private bool isPlayerInBarn = false; // Çoban köpeği ahırda mı?
    private bool isSheepInBarn = false;  // Koyun ahırda mı?

    private void Start()
    {
        UpdateDayCounter();
    }

    void OnTriggerEnter(Collider other)
    {
        // Eğer çoban köpeği ahıra girdiyse
        if (other.CompareTag("Köpek"))
        {
            isPlayerInBarn = true;
        }

        // Eğer koyun ahıra girdiyse
        if (other.CompareTag("Koyun"))
        {
            isSheepInBarn = true;
        }

        // Her iki oyuncu da ahırdaysa
        if (isPlayerInBarn && isSheepInBarn && !isResting)
        {
            StartCoroutine(RestAtBarn());
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Çoban köpeği ahırdan çıkarsa
        if (other.CompareTag("Köpek"))
        {
            isPlayerInBarn = false;
        }

        // Koyun ahırdan çıkarsa
        if (other.CompareTag("Koyun"))
        {
            isSheepInBarn = false;
        }
    }

    private IEnumerator RestAtBarn()
    {
        if (isResting) yield break; // Eğer zaten dinleniliyorsa coroutine'i durdur.

        isResting = true;

        // Siyah ekranı ve yazıyı aktif et
        fadeScreen.SetActive(true);
        statusTextObject.SetActive(true); // "Dinleniyor..." yazısını aktif et.
        dayCounterText.gameObject.SetActive(false); // Gün sayısını gizle
        // Dinlenme süresi
        yield return new WaitForSeconds(fadeDuration);

        // Siyah ekranı ve yazıyı kapat
        fadeScreen.SetActive(false);
        statusTextObject.SetActive(false);

        // Gün sayısını artır ve UI'yi güncelle
        IncrementDay();
        dayCounterText.gameObject.SetActive(true); // Gün sayısını tekrar göster
        isResting = false;
    }

    private void IncrementDay()
    {
        dayCount++; // Gün sayısını artır.
        UpdateDayCounter(); // Gün sayacını ekranda güncelle.
    }
    private void UpdateDayCounter()
    {
        dayCounterText.text = "Gün:" + dayCount; // UI metnini güncelle.
    }
    private void ProcessDayEnd()
    {
        // Koyun çoğaltma: Mevcut koyun sayısının yarısı kadar eklenir
        int newSheep = Mathf.Max(1, sheepCount / 2);
        sheepCount += newSheep;

        // Gün sayısını artır
        dayCount++;
    }
}

