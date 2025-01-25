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
    public Transform dogHouseTrigger; // Köpek kulübesi için trigger alanı
    public GameObject player;
    public float interactionRange = 2f; // Köpek kulübesi ile etkileşim mesafesi

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
    }

    void OnTriggerExit(Collider other)
    {
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

    private void Update()
    {
        int currentDay = worldTime.GetCurrentDay();

        // Eğer oyuncu köpek kulübesi alanındaysa, E tuşuna basıyorsa ve bugünkü dinlenme yapılmadıysa
        if (sheepInBarnCount == totalSheepCount && !isResting && IsNearDogHouse() && Input.GetKeyDown(KeyCode.Z) && lastRestedDay != currentDay)
        {
            StartCoroutine(RestAtBarn());
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
        dayCounterText.text = worldTime.GetCurrentDay().ToString();
    }

    private bool IsNearDogHouse()
    {
        // Oyuncunun köpek kulübesine olan mesafesini kontrol eder
        float distanceToDogHouse = Vector3.Distance(player.transform.position, dogHouseTrigger.position);
        return distanceToDogHouse <= interactionRange;
    }
}
