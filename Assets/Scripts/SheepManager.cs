using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.Serialization;

public class SheepManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public Transform target; // Takip edilen hedef
    [SerializeField] private RectTransform circleRectTransform; // Daire UI'si için referans
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private TMP_Text sheepCountText;
    [FormerlySerializedAs("notificationText")] [SerializeField] private GameObject kacanKoyunBildirim; // Notification Text

    [Header("Values")]
    [SerializeField] private float baseRadius = 2f; // Dairenin yarıçapı
    [SerializeField] private float sheepRadius = 0.85f; // Koyun başına alan
    
    [Header("EscapeSheep")]
    [SerializeField] private float escapeChance = 20f; // Kaçma olasılığı (0-100 arası)
    [SerializeField] private float escapeInterval = 5f; // Kaçma sıklığı (saniye cinsinden)
    
    [Header("List")]
    [SerializeField] public List<Sheep> sheepList = new List<Sheep>(); // Sürüdeki koyunlar
    [SerializeField] private List<Sheep> escapedSheepList = new List<Sheep>(); // Kaçan koyunlar


    //UI
    [SerializeField] private GameObject gameOverPanel;
    private bool isFirstSheepRejoin = true; // İlk kez koyun geri döndüğünü kontrol edecek flag
    [SerializeField] private GameObject SuruBuyuyorPanel; // Sürü Büyüyor paneli için referans
    [SerializeField] private TMPro.TextMeshProUGUI SuruBasari; // Başarı yazısı
    
    [Header("Sheep Prefab")]
    [SerializeField] private GameObject sheepPrefab; // Yeni koyun prefab'ı
    public static SheepManager Instance { get; private set; } // Singleton

    private void Awake()
    {
        // Singleton kontrolü
        if (Instance == null)
        {
            Instance = this; // Eğer Instance null ise kendisini atar
        }
        else
        {
            Destroy(gameObject); // Eğer Instance zaten varsa, bu objeyi yok eder
        }

        // Burada da konsol mesajı ile kontrol edebiliriz
        if (Instance == this)
        {
            Debug.Log("Barking singleton instance'ı oluşturuldu.");
        }
    }

    private void Start()
    {
        if (gameOverPanel!=null)
        {
            gameOverPanel.SetActive(false);
        }

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
        if (WolfSpawner.Instance != null)
        {
            WolfSpawner.Instance.SetFlockTarget(target);
        }
    }

    private void Update()
    {
        HandleRightClickTargetChange();
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            SpawnNewSheep();
        }
    }
    
    private void SpawnNewSheep()
    {
        if (sheepPrefab != null && target != null) // Eğer prefab ve hedef varsa
        {
            Vector3 spawnPosition = target.position + new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f)); // Hedefin yakınına koyun spawnla
            GameObject newSheepObject = Instantiate(sheepPrefab, spawnPosition, Quaternion.identity);
            Sheep newSheep = newSheepObject.GetComponent<Sheep>();

            if (newSheep != null)
            {
                AddSheep(newSheep); // Yeni koyunu sürüye ekle
                newSheep.FollowTarget(target.position); // Hedefe yönlendir
            }
        }
        else
        {
            Debug.LogWarning("Sheep prefab veya hedef eksik!");
        }
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
        for (int i = 0; i < sheepList.Count; i++) // Burada <= değil < olmalı çünkü index sıfırdan başlar
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
            ShowEscapeNotification("Bir koyun kaçıyor!"); // Bildirimi göster
        }
    }

    private void ShowEscapeNotification(string message)
    {
        if (kacanKoyunBildirim != null)
        {
            kacanKoyunBildirim.gameObject.SetActive(true);

            // Belirli bir süre sonra metni sıfırla
            Invoke("ClearNotification", 4f);  // 4 saniye sonra bildirimi kaldır
        }
    }

    private void ClearNotification()
    {
        if (kacanKoyunBildirim != null)
        {
            kacanKoyunBildirim.gameObject.SetActive(false);  // UI'yi pasifleştir
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
            sheepCountText.text = sheepList.Count.ToString();
        }

        if (sheepList.Count == 0)
        {
            gameOverPanel.SetActive(true); 
            Debug.Log("Oyun Bitti! Tüm koyunlar kaybedildi.");
        }
        if (sheepList.Count >= 30 && SuruBuyuyorPanel != null)
        {
            SuruBuyuyorPanel.SetActive(true);
            Invoke("HideSuruBuyuyorPanel", 4f);
        }
    }

    private void HideSuruBuyuyorPanel()
    {
        if (SuruBuyuyorPanel != null)
        {
            SuruBuyuyorPanel.SetActive(false);
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

    public void RejoinEscapedSheep()
    {
        bool anySheepRejoined = false; // Geri dönen koyun olup olmadığını kontrol etmek için
        for (int i = escapedSheepList.Count - 1; i >= 0; i--)
        {
            Sheep escapedSheep = escapedSheepList[i];
            AddSheep(escapedSheep); // Kaçan koyunu sürüye geri ekle
            escapedSheep.MoveToPosition(target.position); // Hedefe doğru hareket ettir
            Debug.Log("Kaçan koyun sürüye geri döndü!");
            anySheepRejoined = true; // Geri dönen koyun var
        }

        // İlk kez geri dönen koyun olduğunda sadece bir kez bu paneli aktif et
        if (anySheepRejoined)
        {
            /*if (isFirstSheepRejoin && Barking.Instance != null)
            {
                Debug.Log("İlk koyun geri döndü, başarı paneli açılıyor.");
                isFirstSheepRejoin = false; // Sonraki geri dönüşlerde paneli gösterme
            }*/
        }
    }

    public void DecreaseSheepCount(bool kurt)
    {
        if (sheepList.Count > 0)
        {
            Sheep sheepToRemove = sheepList[0]; // İlk koyunu al (saldırıya uğrayan)
            sheepList.Remove(sheepToRemove); // Koyunu listeden çıkar
            UpdateSheepCountUI(); // UI'yi güncelle
            if (kurt)
            {
                Destroy(sheepToRemove.gameObject);
            }
            else
            {
                sheepToRemove.GetComponent<Sheep>().Die();
            }
            Debug.Log("Bir koyun öldü! Koyun sayısı: " + sheepList.Count);
        }
    }
}
