using UnityEngine;
using UnityEngine.UI;

public class WaterZone : MonoBehaviour
{
    public Slider waterTimerBar;
    private float waterValue = Mathf.Infinity; // No limit on waterValue
    private bool isWatering = false;
    private int totalSheep = 0;
    private SurvivalSystem survivalSystem;
    [SerializeField] SheepManager _sheepManager;
    private bool hasLoggedMessage = false; // Mesajın yazılıp yazılmadığını takip eden değişken
    [SerializeField] private float drinkTime;

    void Start()
    {
        waterTimerBar.gameObject.SetActive(false); // Removed unnecessary gameObject manipulations
        survivalSystem = FindObjectOfType<SurvivalSystem>();
        _sheepManager = FindObjectOfType<SheepManager>();
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Koyun"))
        {
            totalSheep++;
            CheckWateringStatus();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Koyun"))
        {
            totalSheep--;
            CheckWateringStatus();
        }
    }

    void Update()
    {
        if (isWatering && survivalSystem.currentThirst < 100 && survivalSystem.currentThirst > 0) // Allow watering if thirst is below max and not 0
        {
            // Increase thirst instead of hunger
            float thirstIncreaseAmount = Time.deltaTime * drinkTime;  // Adjust the rate as necessary
            waterValue -= thirstIncreaseAmount;
            waterTimerBar.value = waterValue;  // If you want a visual representation, but waterValue is infinite

            survivalSystem.IncreaseThirst(thirstIncreaseAmount); // Use IncreaseThirst instead of IncreaseHunger
        }
        else if (survivalSystem.currentThirst == 0)
        {
            // Restart watering if thirst reaches 0 after sheep die
            if (totalSheep > 0) // Only restart if there are sheep
            {
                StartWatering();
            }
        }
    }

    void CheckWateringStatus()
    {
        if (totalSheep == _sheepManager.sheepList.Count)
        {
            StartWatering();
            hasLoggedMessage = false; // Tüm koyunlar alana girdiğinde mesajı sıfırla
        }
        else
        {
            StopWatering();
            if (!hasLoggedMessage)
            {
                Debug.Log("Tüm koyunlar alanda değil!");
                hasLoggedMessage = true; // Mesajı yazdırdıktan sonra true yap
            }
        }
    }

    void StartWatering()
    {
        isWatering = true;
        waterTimerBar.gameObject.SetActive(true); // Show the water timer bar

        if (survivalSystem != null)
        {
            survivalSystem.StartThirstIncrease(); // Start increasing thirst
        }
    }

    void StopWatering()
    {
        isWatering = false;
        waterTimerBar.gameObject.SetActive(false); // Hide the water timer bar

        if (survivalSystem != null)
        {
            survivalSystem.StopThirstIncrease(); // Stop thirst increase
        }
    }
}
