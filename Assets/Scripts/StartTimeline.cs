using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartTimeline : MonoBehaviour
{
    public GameObject targetObject,blackscreen; // Aktif hale getirilecek GameObject
    
    public float delay = 1.0f; // Gecikme süresi

    private void Awake()
    {
        targetObject.SetActive(false);
        blackscreen.SetActive(true);
    }

    private void Start()
    {
        // Başlangıçta GameObject'i devre dışı bırak
        
        // Coroutine başlat
        StartCoroutine(ActivateObjectAfterDelay());
    }

    private IEnumerator ActivateObjectAfterDelay()
    {
        // Belirtilen süre kadar bekle
        yield return new WaitForSeconds(delay);

        // GameObject'i aktif hale getir
        targetObject.SetActive(true);
    }
}
