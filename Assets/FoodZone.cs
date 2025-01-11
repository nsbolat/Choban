using UnityEngine;
using UnityEngine.UI;

public class FoodZone : MonoBehaviour
{
    public Slider foodTimerBar; // Beslenme süresi göstergesi
    public float foodTimer = 5f; // 5 saniyelik süre
    public int hungerIncreasePerSecond = 0; // Her saniye artacak açlýk deðeri. Bunu silecem dursun bi

    private bool isFeeding = false; // Þu anda besleniyor mu?
    private float currentTimer = 0f; // Geri sayým için timer
    private int totalSheep = 0; // Alandaki koyun sayýsý

    private SurvivalSystem survivalSystem; // SurvivalSystem referansý

    void Start()
    {
        // SurvivalSystem scriptini bul ve referans al
        survivalSystem = FindObjectOfType<SurvivalSystem>();

        // Timer slider baþlangýç deðerleri
        foodTimerBar.gameObject.SetActive(false);
        foodTimerBar.maxValue = foodTimer;
        foodTimerBar.value = foodTimer;
    }

    void OnTriggerEnter(Collider other)
    {
        // Koyunlar "Sheep" tag'ine sahip olmalý
        if (other.CompareTag("Koyun"))
        {
            totalSheep++;
            CheckFeedingStatus();
            // Beslenme baþladýðýnda açlýk artmaya baþlasýn
            if (survivalSystem != null)
            {
                survivalSystem.isFeeding = true;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Koyun"))
        {
            totalSheep--;
            CheckFeedingStatus();
            // Beslenme bittiðinde açlýk azalmaya baþlasýn
            if (survivalSystem != null)
            {
                survivalSystem.isFeeding = false;
            }
        }
    }

    void Update()
    {
        if (isFeeding)
        {
            currentTimer -= Time.deltaTime;
            foodTimerBar.value = currentTimer;

            if (currentTimer <= 0f)
            {
                StopFeeding();
                
            }
        }
    }

    void CheckFeedingStatus()
    {
        if (totalSheep > 0 && !isFeeding)
        {
            StartFeeding();
        }
        else if (totalSheep == 0 && isFeeding)
        {
            StopFeeding();
        }
    }

    void StartFeeding()
    {
        isFeeding = true;
        currentTimer = foodTimer;
        foodTimerBar.value = foodTimer;
        foodTimerBar.gameObject.SetActive(true);
    }

    void StopFeeding()
    {
        isFeeding = false;
        foodTimerBar.gameObject.SetActive(false);
    }
}
