using System;
using System.Collections;
using UnityEngine;
using TMPro;
using WorldTime;

public class BarnMechanic : MonoBehaviour
{
    public GameObject fadeScreen;
    public GameObject statusTextObject;
    public GameObject restPanel; // "Uyumak için Z'ye bas" paneli
    public GameObject sheepMissingPanel; // "Koyunların hepsi ahırda değil" paneli
    public GameObject alreadyRestedPanel; // "Bu gün zaten uyudun" paneli
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
        restPanel.SetActive(false); // Paneller başlangıçta kapalı
        sheepMissingPanel.SetActive(false);
        alreadyRestedPanel.SetActive(false);
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
            CloseAllPanels(); // Oyuncu uzaklaşınca panelleri kapat
        }

        if (other.CompareTag("Koyun"))
        {
            sheepInBarnCount--;
        }
    }

    private void Update()
    {
        int currentDay = worldTime.GetCurrentDay();

        // Köpek kulübesine yakınlık ve etkileşim kontrolü
        if (IsNearDogHouse())
        {
            if (sheepInBarnCount == totalSheepCount)
            {
                if (lastRestedDay == currentDay)
                {
                    // Bugün zaten uyumuş, "Bu gün zaten uyudun" panelini göster
                    ShowAlreadyRestedPanel();
                }
                else
                {
                    // Tüm koyunlar ahırda, uyumak için paneli göster
                    ShowRestPanel();

                    if (!isResting && Input.GetKeyDown(KeyCode.Z))
                    {
                        StartCoroutine(RestAtBarn());
                    }
                }
            }
            else
            {
                // Tüm koyunlar ahırda değil, uyuyamazsın paneli göster
                ShowSheepMissingPanel();
            }
        }
        else
        {
            CloseAllPanels(); // Köpek kulübesinden uzaksa panelleri kapat
        }
    }

    private IEnumerator RestAtBarn()
    {
        if (isResting) yield break;

        isResting = true;

        CloseAllPanels(); // Dinlenme sırasında panelleri kapat
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

    private void ShowRestPanel()
    {
        restPanel.SetActive(true);
        sheepMissingPanel.SetActive(false);
        alreadyRestedPanel.SetActive(false);
    }

    private void ShowSheepMissingPanel()
    {
        sheepMissingPanel.SetActive(true);
        restPanel.SetActive(false);
        alreadyRestedPanel.SetActive(false);
    }

    private void ShowAlreadyRestedPanel()
    {
        alreadyRestedPanel.SetActive(true);
        restPanel.SetActive(false);
        sheepMissingPanel.SetActive(false);
    }

    private void CloseAllPanels()
    {
        restPanel.SetActive(false);
        sheepMissingPanel.SetActive(false);
        alreadyRestedPanel.SetActive(false);
    }
}
