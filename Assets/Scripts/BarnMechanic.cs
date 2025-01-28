using System;
using System.Collections;
using UnityEngine;
using TMPro;
using WorldTime;
using System.Collections.Generic;

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
    private HashSet<GameObject> sheepInBarn = new HashSet<GameObject>(); // Ahırdaki koyunlar
    private int lastRestedDay = -1; // En son dinlenilen günü takip eder (-1: hiç dinlenilmedi)

    private void Start()
    {
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Köpek"))
        {
            isPlayerInBarn = true;
        }

        if (other.CompareTag("Koyun"))
        {
            sheepInBarn.Add(other.gameObject); // Koyunu listeye ekle
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isResting) return;

        if (other.CompareTag("Köpek"))
        {
            isPlayerInBarn = false;
            CloseAllPanels(); // Oyuncu uzaklaşınca panelleri kapat
        }

        if (other.CompareTag("Koyun"))
        {
            sheepInBarn.Remove(other.gameObject); // Koyunu listeden çıkar
        }
    }

    private void Update()
    {
        int currentDay = worldTime.GetCurrentDay();

        // Köpek kulübesine yakınlık ve etkileşim kontrolü
        if (IsNearDogHouse())
        {
            if (sheepInBarn.Count == GameObject.FindGameObjectsWithTag("Koyun").Length) // Ahırdaki ve toplam koyun sayısını karşılaştır
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
        AddSheepToBarn(5);
        fadeScreen.SetActive(false);
        statusTextObject.SetActive(false);

        worldTime.AddTime(TimeSpan.FromHours(7)); // Zamanı 7 saat ilerlet
        lastRestedDay = worldTime.GetCurrentDay(); // Bugünkü dinlenme tamamlandı

        dayCounterText.gameObject.SetActive(true);

        isResting = false;
    }

    private void AddSheepToBarn(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (SheepManager.Instance != null)
            {
                GameObject newSheepPrefab = Instantiate(
                    SheepManager.Instance.sheepList[0].gameObject, // Mevcut bir koyun prefab'ını baz al
                    SheepManager.Instance.target.position + new Vector3(UnityEngine.Random.Range(-2f, 2f), 0, UnityEngine.Random.Range(-2f, 2f)),
                    Quaternion.identity
                );

                Sheep newSheep = newSheepPrefab.GetComponent<Sheep>();
                if (newSheep != null)
                {
                    SheepManager.Instance.AddSheep(newSheep); // Yeni koyunu sürüye ekle
                    sheepInBarn.Add(newSheep.gameObject); // Yeni koyunu ahır listesine ekle
                    Debug.Log("Ahıra yeni bir koyun eklendi.");
                }
            }
            else
            {
                Debug.LogError("SheepManager bulunamadı!");
            }
        }
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