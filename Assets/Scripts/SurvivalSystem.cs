using UnityEngine;
using UnityEngine.UI;

public class SurvivalSystem : MonoBehaviour
{
    [SerializeField] private Slider healthSlider; 
    [SerializeField] private Slider thirstSlider; 
    [SerializeField] private Slider hungerSlider; 

    private float maxHealth = 100f; 
    private float maxThirst = 100f; 
    private float maxHunger = 100f; 

    private float currentHealth;
    private float currentThirst;
    private float currentHunger;

    private float thirstDecreaseRate = 10f; 
    private float hungerDecreaseRate = 15f; 
    private float healthDecreaseRate = 5f; 

    private void Start()
    {
        
        currentHealth = maxHealth;
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

        
        if (currentThirst <= 0)
        {
            currentHealth -= healthDecreaseRate * Time.deltaTime;
        }

        if (currentHunger <= 0)
        {
            currentHealth -= healthDecreaseRate * Time.deltaTime;
        }

        
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        
        UpdateSliders();
    }

    private void UpdateSliders()
    {
        healthSlider.value = currentHealth / maxHealth; 
        thirstSlider.value = currentThirst / maxThirst; 
        hungerSlider.value = currentHunger / maxHunger; 
    }
}
