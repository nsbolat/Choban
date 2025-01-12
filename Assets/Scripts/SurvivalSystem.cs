using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SurvivalSystem : MonoBehaviour
{
    [SerializeField] private Slider thirstSlider;
    [SerializeField] private Slider hungerSlider;
    [SerializeField] private TextMeshProUGUI thirstText;
    [SerializeField] private TextMeshProUGUI hungerText;
    public float maxThirst = 100f;
    public float maxHunger = 100f;
    [SerializeField] private float azalmaHızıHunger;
    [SerializeField] private float azalmaHızıSusuzluk;
    [SerializeField] private float artmaHızıSusuzluk;
    [SerializeField] private float artmaHızıHunger;
    public float currentThirst;
    public float currentHunger;
    [SerializeField] private float sheepReductionInterval = 5f;
    private float thirstReductionTimer = 0f;
    private float hungerReductionTimer = 0f;
    public bool isFeeding, isWatering = false;

    private void Awake()
    {
        thirstSlider = GameObject.Find("sheepThirst").GetComponent<Slider>();
        hungerSlider = GameObject.Find("sheepHunger").GetComponent<Slider>();
        thirstText = GameObject.Find("ThirstText").GetComponent<TextMeshProUGUI>();
        hungerText = GameObject.Find("HungerText").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        currentThirst = maxThirst;
        currentHunger = maxHunger;
        UpdateSliders();
    }

    private void Update()
    {
        if (isFeeding)
        {
            currentHunger += artmaHızıHunger * Time.deltaTime;
            currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger); // 100'ü aşmaz

            // Beslenme 100 olsa bile devam eder
        }
        else
        {
            currentHunger -= azalmaHızıHunger * Time.deltaTime;
            currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);
        }

        if (isWatering)
        {
            currentThirst += artmaHızıSusuzluk * Time.deltaTime;
            currentThirst = Mathf.Clamp(currentThirst, 0, maxThirst);
        }
        else
        {
            currentThirst -= azalmaHızıSusuzluk * Time.deltaTime;
            currentThirst = Mathf.Clamp(currentThirst, 0, maxThirst);
        }


        if (currentThirst <= 0 && !isWatering)
        {
            thirstReductionTimer += Time.deltaTime;
            if (thirstReductionTimer >= sheepReductionInterval)
            {
                SheepManager.Instance.DecreaseSheepCount();
                thirstReductionTimer = 0f;
            }
        }

        if (currentHunger <= 0)
        {
            Debug.Log("Susuzluk bitti");
            hungerReductionTimer += Time.deltaTime;
            if (hungerReductionTimer >= sheepReductionInterval)
            {
                SheepManager.Instance.DecreaseSheepCount();
                hungerReductionTimer = 0f;
            }
        }
        else
        {
            hungerReductionTimer = 0f;
        }

        UpdateSliders();
    }

    public void UpdateSliders()
    {
        thirstSlider.value = currentThirst / maxThirst;
        hungerSlider.value = currentHunger / maxHunger;
        thirstText.text = $"{Mathf.FloorToInt(currentThirst)}/{Mathf.FloorToInt(maxThirst)}";
        hungerText.text = $"{Mathf.FloorToInt(currentHunger)}/{Mathf.FloorToInt(maxHunger)}";
    }

    public void IncreaseHunger(float amount)
    {
        currentHunger += amount;
        currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);
    }

    public void StartHungerIncrease()
    {
        isFeeding = true;
    }

    public void StopHungerIncrease()
    {
        isFeeding = false;
    }
    public void IncreaseThirst(float amount)
    {
        currentThirst += amount;
        currentThirst = Mathf.Clamp(currentThirst, 0, maxThirst);
    }

    public void StartThirstIncrease()
    {
        isWatering = true; // Or create a separate bool for thirst-related feeding
        Debug.Log("Su içiyo");

    }

    public void StopThirstIncrease()
    {
        isWatering = false; // Or create a separate bool for thirst-related feeding
        Debug.Log("Su içmiyo");
    }
    
}
