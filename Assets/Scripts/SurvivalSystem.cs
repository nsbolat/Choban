using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SurvivalSystem : MonoBehaviour
{
    [SerializeField] private Slider thirstSlider;
    [SerializeField] private Slider hungerSlider;

    [SerializeField] private TextMeshProUGUI thirstText;
    [SerializeField] private TextMeshProUGUI hungerText;

    [SerializeField] private float maxThirst = 100f;
    [SerializeField] private float maxHunger = 100f;

    [SerializeField] private float currentThirst;
    [SerializeField] private float currentHunger;

    [SerializeField] private float thirstDecreaseRate = 10f;
    [SerializeField] private float hungerDecreaseRate = 15f;

    [SerializeField] private float sheepReductionInterval = 5f; // 5 saniye
    private float thirstReductionTimer = 0f;
    private float hungerReductionTimer = 0f;

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
        currentThirst -= thirstDecreaseRate * Time.deltaTime;
        currentHunger -= hungerDecreaseRate * Time.deltaTime;

        currentThirst = Mathf.Clamp(currentThirst, 0, maxThirst);
        currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);

        // Susuzluk sıfırsa timer'ı çalıştır
        if (currentThirst <= 0)
        {
            thirstReductionTimer += Time.deltaTime;
            if (thirstReductionTimer >= sheepReductionInterval)
            {
                SheepManager.Instance.DecreaseSheepCount(); // Koyun sayısını azalt
                thirstReductionTimer = 0f; // Timer sıfırla
            }
        }
        else
        {
            thirstReductionTimer = 0f; // Timer sıfırla
        }

        // Açlık sıfırsa timer'ı çalıştır
        if (currentHunger <= 0)
        {
            hungerReductionTimer += Time.deltaTime;
            if (hungerReductionTimer >= sheepReductionInterval)
            {
                SheepManager.Instance.DecreaseSheepCount(); // Koyun sayısını azalt
                hungerReductionTimer = 0f; // Timer sıfırla
            }
        }
        else
        {
            hungerReductionTimer = 0f; // Timer sıfırla
        }

        UpdateSliders();
    }

    private void UpdateSliders()
    {
        thirstSlider.value = currentThirst / maxThirst;
        hungerSlider.value = currentHunger / maxHunger;

        thirstText.text = $"{Mathf.FloorToInt(currentThirst)}/{Mathf.FloorToInt(maxThirst)}";
        hungerText.text = $"{Mathf.FloorToInt(currentHunger)}/{Mathf.FloorToInt(maxHunger)}";
    }
}
