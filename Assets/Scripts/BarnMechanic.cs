using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WorldTime;

public class BarnMechanic : MonoBehaviour
{
    public GameObject fadeScreen;
    public GameObject statusTextObject;
    public TextMeshProUGUI dayCounterText;
    public float fadeDuration = 5f;
    public WorldTime.WorldTime worldTime;

    private bool isResting = false;
    private bool isPlayerInBarn = false;
    private int sheepInBarnCount = 0;
    private int totalSheepCount = 0;
    private int lastRestedDay = -1; // En son dinlenilen günü takip eder (-1: hiç dinlenilmedi)

    private void Start()
    {
        totalSheepCount = GameObject.FindGameObjectsWithTag("Koyun").Length; // Toplam koyun sayısını belirle
        UpdateDayCounter();
        worldTime.OnDayChanged += OnDayChanged; // Gün değişim event'ine abone ol
    }

    private void OnDestroy()
    {
        worldTime.OnDayChanged -= OnDayChanged; // Aboneliği kaldır
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Köpek"))
        {
            isPlayerInBarn = true;
        }

        if (other.CompareTag("Koyun"))
        {
            sheepInBarnCount++;
        }

        int currentDay = worldTime.GetCurrentDay();

        // Tüm koşullar sağlanıyorsa ve bugünkü dinlenme yapılmadıysa
        if (isPlayerInBarn && sheepInBarnCount == totalSheepCount && !isResting && lastRestedDay != currentDay)
        {
            StartCoroutine(RestAtBarn());
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Eğer dinlenme işlemi devam ediyorsa çıkışları kontrol etme
        if (isResting) return;

        if (other.CompareTag("Köpek"))
        {
            isPlayerInBarn = false;
        }

        if (other.CompareTag("Koyun"))
        {
            sheepInBarnCount--;
        }
    }

    private IEnumerator RestAtBarn()
    {
        if (isResting) yield break;

        isResting = true;

        fadeScreen.SetActive(true);
        statusTextObject.SetActive(true);
        dayCounterText.gameObject.SetActive(false);

        yield return new WaitForSeconds(fadeDuration);

        fadeScreen.SetActive(false);
        statusTextObject.SetActive(false);

        worldTime.AddTime(TimeSpan.FromHours(7)); // Zamanı 7 saat ilerlet
        lastRestedDay = worldTime.GetCurrentDay(); // Bugünkü dinlenme tamamlandı

        dayCounterText.gameObject.SetActive(true);

        isResting = false;
    }

    private void OnDayChanged(int newDay)
    {
        UpdateDayCounter();
    }

    private void UpdateDayCounter()
    {
        dayCounterText.text = "Gün:" + worldTime.GetCurrentDay();
    }
}



