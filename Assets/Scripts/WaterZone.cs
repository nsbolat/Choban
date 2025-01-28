using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterZone : MonoBehaviour
{
    public Slider WaterTimerBar;   
    private bool isWatering = false;
    private int totalSheep = 0;
    private SurvivalSystem survivalSystem;
    [SerializeField] SheepManager _sheepManager;
    private bool hasLoggedMessage = false; // Mesaj�n yaz�l�p yaz�lmad���n� takip eden deiken
    void Start()
    {
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
    // Update is called once per frame
    void Update()
    {
       
        if (isWatering)
        {
            // Sadece susuzluk bar�n� art�r
            if ( survivalSystem.currentThirst != 101)
            {
                float thirstIncreaseAmount = Time.deltaTime * 10;
                survivalSystem.currentThirst += thirstIncreaseAmount;
                survivalSystem.currentThirst = Mathf.Clamp(survivalSystem.currentThirst, 0, survivalSystem.maxThirst);
                survivalSystem.UpdateSliders();
            }

            // Susuzluk 100 oldu�unda su i�me i�lemini durdur
            if (survivalSystem.currentThirst == 150)
            {
                StopWatering();
            }
        }
    }

    void CheckWateringStatus()
    {
        if (totalSheep == _sheepManager.sheepList.Count)
        {
            StartWatering();
            hasLoggedMessage = false; // T�m koyunlar alana girdi�inde mesaj� s�f�rla
        }
        else
        {
            StopWatering();
            if (!hasLoggedMessage)
            {
                Debug.Log("Tm koyunlar alanda deil!");
                hasLoggedMessage = true; // Mesaj� yazd�rd�ktan sonra true yap
            }
        }
    }

    void StartWatering()
    {
        isWatering = true;
        
        WaterTimerBar.gameObject.SetActive(true);

        if (survivalSystem != null)
        {
            survivalSystem.StartWaterIncrease();
        }
    }

    void StopWatering()
    {
        isWatering = false;
        WaterTimerBar.gameObject.SetActive(false);

        if (survivalSystem != null)
        {
            survivalSystem.StopWaterIncrease();
        }
    }

}
