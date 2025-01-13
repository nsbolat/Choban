using UnityEngine;
using UnityEngine.UI;

public class FoodZone : MonoBehaviour
{
    public Slider foodTimerBar;
    public float foodValue = 100f;
    private bool isFeeding = false;
    private int totalSheep = 0;
    private SurvivalSystem survivalSystem;
    [SerializeField] SheepManager _sheepManager;
    private bool hasLoggedMessage = false; // Mesajın yazılıp yazılmadığını takip eden değişken
    [SerializeField] private GameObject noEatGrass, eatedGrass;
    private WorldTime.WorldTime worldTime;
    private int daysSinceEmpty = 0;

    void Start()
    {
        noEatGrass.gameObject.SetActive(true);
        eatedGrass.SetActive(false);
        survivalSystem = FindObjectOfType<SurvivalSystem>();
        _sheepManager = FindObjectOfType<SheepManager>();
        foodTimerBar.gameObject.SetActive(false);
        foodTimerBar.maxValue = foodValue;
        foodTimerBar.value = foodValue;
        
        worldTime = FindObjectOfType<WorldTime.WorldTime>(); // WorldTime referansı alın
        if (worldTime!=null)
        {
            worldTime.OnDayChanged += OnDayChanged; // OnDayChanged olayını dinleyin
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Koyun"))
        {
            totalSheep++;
            CheckFeedingStatus();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Koyun"))
        {
            totalSheep--;
            CheckFeedingStatus();
        }
    }

    void Update()
    {
        if (isFeeding && survivalSystem.currentHunger != 101)
        {
            float hungerIncreaseAmount = Time.deltaTime * 10;
            foodValue -= hungerIncreaseAmount;
            foodTimerBar.value = foodValue;

            survivalSystem.IncreaseHunger(hungerIncreaseAmount);

            if (foodValue <= 0f)
            {
                StopFeeding();
                noEatGrass.gameObject.SetActive(false);
                eatedGrass.SetActive(true);
                daysSinceEmpty = 0; // foodValue sıfırlandığında sayaç sıfırlanır
            }
        }
    }

    void CheckFeedingStatus()
    {
        if (totalSheep == _sheepManager.sheepList.Count)
        {
            StartFeeding();
            hasLoggedMessage = false; // Tüm koyunlar alana girdiğinde mesajı sıfırla
        }
        else
        {
            StopFeeding();
            if (!hasLoggedMessage)
            {
                Debug.Log("Tüm koyunlar alanda değil!");
                hasLoggedMessage = true; // Mesajı yazdırdıktan sonra true yap
            }
        }
    }
    void OnDayChanged(int currentDay)
    {
        if (foodValue <= 0f)
        {
            daysSinceEmpty++;

            if (daysSinceEmpty >= 3)
            {
                foodValue = 100f;
                foodTimerBar.value = foodValue;
                noEatGrass.gameObject.SetActive(true);
                eatedGrass.SetActive(false);
                Debug.Log("Alan yeniden doldu!");
            }
        }
    }

    
     void StartFeeding()
    {
        isFeeding = true;
        foodTimerBar.value = foodValue;
        foodTimerBar.gameObject.SetActive(true);

        if (survivalSystem != null)
        {
            survivalSystem.StartHungerIncrease();
        }
    }

    void StopFeeding()
    {
        isFeeding = false;
        foodTimerBar.gameObject.SetActive(false);

        if (survivalSystem != null)
        {
            survivalSystem.StopHungerIncrease();
        }
    }
    
}
