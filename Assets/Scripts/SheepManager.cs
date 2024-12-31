using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using TMPro;

public class SheepManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public Transform target; // Takip edilen hedef
    [SerializeField] private RectTransform circleRectTransform; // Daire UI'si için referans
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private TMP_Text sheepCountText;

    [Header("Values")]
    [SerializeField] private float baseRadius = 2f; // Dairenin yarıçapı
    [SerializeField] private float sheepRadius = 0.85f; // Koyun başına alan
    
    [Header("EscapeSheep")]
    [SerializeField] private float escapeChance = 20f; // Kaçma olasılığı (0-100 arası)
    [SerializeField] private float escapeInterval = 5f; // Kaçma sıklığı (saniye cinsinden)
    
    [Header("List")]
    [SerializeField] private List<Sheep> sheepList = new List<Sheep>(); // Sürüdeki koyunlar
    [SerializeField] private List<Sheep> escapedSheepList = new List<Sheep>(); // Kaçan koyunlar

    

    public static SheepManager Instance { get; private set; } // Singleton

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        foreach (Sheep sheep in FindObjectsOfType<Sheep>())
        {
            sheepList.Add(sheep);
        }

        UpdateSheepCountUI();
        UpdateBaseRadius();
        UpdateCircleSize();
        ArrangeSheepInCircle();

        // Kaçma kontrolünü belirli bir sıklıkla çağır
        InvokeRepeating(nameof(CheckForEscape), escapeInterval, escapeInterval); // Her 'escapeInterval' saniyede bir kontrol et
    }

    private void Update()
    {
        HandleRightClickTargetChange();
       // UpdateSheepFollowTarget();
    }
    
    private void HandleRightClickTargetChange()
    {
        if (Input.GetMouseButtonDown(1)) // Sağ tık kontrolü
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
            {
                if (target != null)
                {
                    target.position = hit.point; // Hedef pozisyonunu güncelle
                    ArrangeSheepInCircle(); // Koyunları yeni pozisyonlara yerleştir
                }
            }
        }
    }
    private void ArrangeSheepInCircle()
    {
        if (sheepList.Count == 0) return;

        UpdateBaseRadius(); // Koyun sayısına göre base radius güncelle
        UpdateCircleSize(); // Daireyi yeniden boyutlandır

        // Çevreyi 360 derece olarak kabul ederek her koyun için bir açı hesapla
        for (int i = 0; i < sheepList.Count; i++)
        {
            // Koyunun her biri için rastgele bir açı belirle
            float angle = (360f / sheepList.Count) * i; // Her koyunun farklı bir açısı olacak
            float angleRad = Mathf.Deg2Rad * angle;

            // Rastgele bir yarıçap belirle (çember içinde düzgün dağılım için sqrt kullan)
            float randomRadius = Mathf.Sqrt(Random.Range(0f, 1f)) * baseRadius;

            // Çember içinde rastgele bir pozisyon hesapla
            Vector3 circlePosition = new Vector3(
                target.position.x + randomRadius * Mathf.Cos(angleRad),
                target.position.y,
                target.position.z + randomRadius * Mathf.Sin(angleRad)
            );

            // Koyunu yeni pozisyona yönlendir
            sheepList[i].FollowTarget(circlePosition); // Koyunun hedefini yeni konuma ayarla
        }
    }
    private void CheckForEscape()
    {
        if (sheepList.Count > 0 && Random.Range(0f, 100f) < escapeChance) // escapeChance ile kaçma olasılığını kontrol et
        {
            Sheep escapingSheep = sheepList[Random.Range(0, sheepList.Count)];
            RemoveSheep(escapingSheep);

            Vector3 escapePosition = new Vector3(
                target.position.x + Random.Range(20f, 50f),
                target.position.y,
                target.position.z + Random.Range(20f, 50f)
            );

            escapingSheep.Escape(escapePosition);
            escapedSheepList.Add(escapingSheep);
            Debug.Log("Bir koyun kaçtı!");
        }
    }

public void AddSheep(Sheep newSheep)
{
    if (!sheepList.Contains(newSheep))
    {
        sheepList.Add(newSheep);
        if (escapedSheepList.Contains(newSheep))
        {
            escapedSheepList.Remove(newSheep);
        }

        // Koyun tekrar sürüye katıldığında sadece yeni koyunun rastgele bir pozisyona yerleşmesini sağla
        // Kaçan koyun için ArrangeSheepInCircle fonksiyonunu çağır
        if (escapedSheepList.Contains(newSheep))
        {
            Vector3 escapePosition = new Vector3(
                target.position.x + Random.Range(20f, 50f),
                target.position.y,
                target.position.z + Random.Range(20f, 50f)
            );

            newSheep.FollowTarget(escapePosition); // Kaçan koyun hedefi yeni pozisyona ayarla
        }

        UpdateSheepCountUI();
    }
}

    public void RemoveSheep(Sheep sheepToRemove)
    {
        if (sheepList.Contains(sheepToRemove))
        {
            sheepList.Remove(sheepToRemove);
            UpdateBaseRadius();
            UpdateCircleSize();
            UpdateSheepCountUI();
        }
    }

    private void UpdateSheepCountUI()
    {
        if (sheepCountText != null)
        {
            sheepCountText.text = "Koyun Sayısı: " + sheepList.Count;
        }
    }

    void UpdateCircleSize()
    {
        if (circleRectTransform != null)
        {
            float diameter = baseRadius * 4f;
            circleRectTransform.sizeDelta = new Vector2(diameter, diameter);
        }
    }

    void UpdateBaseRadius()
    {
        baseRadius = Mathf.Sqrt(sheepList.Count) * sheepRadius;
    }
    
}
