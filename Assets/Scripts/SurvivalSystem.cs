using System;
using UnityEngine;
using UnityEngine.UI;

public class SurvivalSystem : MonoBehaviour
{
    [SerializeField] private Slider healthSlider; 
    [SerializeField] private Slider thirstSlider; 
    [SerializeField] private Slider hungerSlider; 

    [SerializeField]private float maxHealth = 100f; 
    [SerializeField] private float maxThirst = 100f; 
    [SerializeField] private float maxHunger = 100f; 

    [SerializeField] private float currentHealth;
    [SerializeField] private float currentThirst;
    [SerializeField] private float currentHunger;

   [SerializeField] private float thirstDecreaseRate = 10f; 
   [SerializeField] private float hungerDecreaseRate = 15f; 
   [SerializeField] private float healthDecreaseRate = 5f;

    private void Awake()
    {
        healthSlider= GameObject.Find("sheepHealth").gameObject.GetComponent<Slider>();
        thirstSlider= GameObject.Find("sheepThirst").gameObject.GetComponent<Slider>();
        hungerSlider= GameObject.Find("sheepHunger").gameObject.GetComponent<Slider>();
    }

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
    public void TakeDamage(float damage)
    {
        currentHealth -= damage; // Sağlığı azalt
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Sağlığı sınırla

        UpdateSliders(); // UI güncelle

        if (currentHealth <= 0)
        {
            Debug.Log("Sürü öldü!"); // Sağlık sıfırlandığında işlem yapılabilir
        }
    }
}
