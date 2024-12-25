using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WorldTime;

public class BarnMechanic : MonoBehaviour
{
    public GameObject fadeScreen;
    public GameObject statusTextObject;
    public TextMeshProUGUI dayCounterText;
    public float fadeDuration = 5f;
    public WorldTime.WorldTime worldTime;

    [SerializeField] private bool isResting = false;
    [SerializeField] private bool isPlayerInBarn = false;
    [SerializeField] private bool isSheepInBarn = false;

    private void Start()
    {
        UpdateDayCounter();
        worldTime.OnDayChanged += OnDayChanged; // Gün değişim event'ine abone ol
    }

    private void OnDestroy()
    {
        worldTime.OnDayChanged -= OnDayChanged; // Aboneliği kaldır
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Köpek"))
        {
            isPlayerInBarn = true;
        }

        if (other.CompareTag("Koyun"))
        {
            isSheepInBarn = true;
        }

        if (isPlayerInBarn && isSheepInBarn && !isResting)
        {
            StartCoroutine(RestAtBarn());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Köpek"))
        {
            isPlayerInBarn = false;
        }

        if (other.CompareTag("Koyun"))
        {
            isSheepInBarn = false;
        }
    }

    private IEnumerator RestAtBarn()
    {
        if (isResting) yield break;

        isResting = true;

        fadeScreen.SetActive(true);
        statusTextObject.SetActive(true);
        dayCounterText.gameObject.SetActive(false);

        yield return new WaitForSeconds(fadeDuration);

        fadeScreen.SetActive(false);
        statusTextObject.SetActive(false);

        worldTime.AddTime(TimeSpan.FromHours(7)); // Zamanı 7 saat ilerlet

        dayCounterText.gameObject.SetActive(true);

        isResting = false;
    }

    private void OnDayChanged(int newDay)
    {
        UpdateDayCounter();
    }

    private void UpdateDayCounter()
    {
        dayCounterText.text = "Gün:" + worldTime.GetCurrentDay();
    }
}

