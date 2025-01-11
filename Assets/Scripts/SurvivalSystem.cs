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

    public float currentThirst;
    public float currentHunger;
  
    [SerializeField] private float sheepReductionInterval = 5f; // 5 saniye
    private float thirstReductionTimer = 0f;
    private float hungerReductionTimer = 0f;

    public bool isFeeding = false; // Besin alanı içinde olup olmadığını kontrol etmek için eklendi

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
        float previousHunger = currentHunger;  // Önceki açlık değerini kaydediyoruz

        if (isFeeding)
        {
            // Besin alanında olduğunda açlık artmaya devam eder
            currentHunger += 5 * Time.deltaTime; // 20 açlık değeri her saniye artacak
            currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);
        }
        else
        {
            // Besin alanı dışında olduğunda açlık azalmaya devam eder
            currentHunger -= 5 * Time.deltaTime;
            currentHunger = Mathf.Clamp(currentHunger, 0, maxHunger);
        }

        

        currentThirst -= 5 * Time.deltaTime;
        currentThirst = Mathf.Clamp(currentThirst, 0, maxThirst);

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

    public void UpdateSliders()
    {
        thirstSlider.value = currentThirst / maxThirst;
        hungerSlider.value = currentHunger / maxHunger;

        thirstText.text = $"{Mathf.FloorToInt(currentThirst)}/{Mathf.FloorToInt(maxThirst)}";
        hungerText.text = $"{Mathf.FloorToInt(currentHunger)}/{Mathf.FloorToInt(maxHunger)}";
    }
}
