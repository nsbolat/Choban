using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random; // UI Image kullanabilmek için

public class Barking : MonoBehaviour
{
    [SerializeField] private AudioClip[] barkSheepSounds, barkWolfSounds; // Havlama sesleri
    [SerializeField] private AudioSource audioSource; // Ses çalacak kaynak
    [SerializeField] private Animator _playerAnim;
    [SerializeField] private Image qCooldownImage; // Q tuşu için cooldown görseli
    [SerializeField] private Image eCooldownImage; // E tuşu için cooldown görseli

    private float qCooldown = 0f; // Q tuşu bekleme süresi
    private float eCooldown = 0f; // E tuşu bekleme süresi
    private float qCooldownDuration = 3f; // Q için cooldown süresi
    private float eCooldownDuration = 5f; // E için cooldown süresi
    [SerializeField] private GameObject KurtBasariPanel; // Başarı paneli
    private bool isFirstWolfEscape = true; // İlk kez tetiklenmesi için
    [SerializeField] private GameObject KoyunBasariPanel; // Başarı paneli
    public static Barking Instance { get; private set; } // Singleton Instanc
    private void Start()
    {
        // AudioSource referansını al
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        _playerAnim = GameObject.FindWithTag("Köpek").GetComponent<Animator>();
        // Başlangıçta paneli kapalı yap
        if (KurtBasariPanel != null)
        {
            KurtBasariPanel.SetActive(false);
        }
    }
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
    }
    private void Update()
    {
        // Cooldown sürelerini azalt
        if (qCooldown > 0)
        {
            qCooldown -= Time.deltaTime;
            qCooldownImage.fillAmount = 1 - (qCooldown / qCooldownDuration); // Q için resmin dolmasını sağla
        }

        if (eCooldown > 0)
        {
            eCooldown -= Time.deltaTime;
            eCooldownImage.fillAmount = 1 - (eCooldown / eCooldownDuration); // E için resmin dolmasını sağla
        }

        // E tuşu için cooldown kontrolü
        if (Input.GetKeyDown(KeyCode.E) && eCooldown <= 0)
        {
            _playerAnim.SetTrigger("Havla");
            SheepManager.Instance.RejoinEscapedSheep(); // Kaçan koyunları geri getir
            eCooldown = eCooldownDuration; // E tuşu cooldown başlat
            eCooldownImage.fillAmount = 0; // E tuşu görselini sıfırla
            Debug.Log("E tuşu kullanıldı. Cooldown başladı: " + eCooldownDuration + " saniye.");
        }

        // Q tuşu için cooldown kontrolü
        if (Input.GetKeyDown(KeyCode.Q) && qCooldown <= 0)
        {
            _playerAnim.SetTrigger("Havla2");
            WolfSpawner.Instance.SendNearbyWolvesToEscapePoint(); // Yakındaki kurtları kaçışa gönder
            qCooldown = qCooldownDuration; // Q tuşu cooldown başlat
            qCooldownImage.fillAmount = 0; // Q tuşu görselini sıfırla
            Debug.Log("Q tuşu kullanıldı. Cooldown başladı: " + qCooldownDuration + " saniye.");
        }
    }

    // Havlama animasyon event'inde çağrılacak fonksiyon
    public void PlayBarkSheepSound()
    {
        if (barkSheepSounds.Length > 0)
        {
            // Sesleri rastgele seç
            AudioClip randomBark = barkSheepSounds[Random.Range(0, barkSheepSounds.Length)];

            // Pitch değerini rastgele ayarla (0.8 ile 1.2 arası)
            audioSource.pitch = Random.Range(1.0f, 1.2f);

            // PlayOneShot ile ses çal
            audioSource.PlayOneShot(randomBark);
        }
    }
    public void OnWolfEscape()
    {
        if (isFirstWolfEscape) // Eğer ilk kez kaçıyorsa paneli aç
        {
            if (KurtBasariPanel)
            {
                KurtBasariPanel.SetActive(true);
            }
            Debug.Log("İlk kurt kaçtı, başarı paneli açıldı.");
            isFirstWolfEscape = false; // Sonraki çağrılarda tekrar açılmasın
            StartCoroutine(HideSuccessPanel());
        }
    }
    private IEnumerator HideSuccessPanel()
    {
        yield return new WaitForSeconds(4f);

        if (KurtBasariPanel != null)
        {
            KurtBasariPanel.SetActive(false);
        }

        Debug.Log("Başarı paneli 4 saniye sonra kapatıldı.");
    }

    public void PlayBarkWolfSound()
    {
        if (barkWolfSounds.Length > 0)
        {
            // Sesleri rastgele seç
            AudioClip randomBark = barkWolfSounds[Random.Range(0, barkWolfSounds.Length)];

            // Pitch değerini rastgele ayarla (0.8 ile 1.2 arası)
            audioSource.pitch = Random.Range(1.0f, 1.2f);

            // PlayOneShot ile ses çal
            audioSource.PlayOneShot(randomBark);
        }
    }
    public void ShowBasariPanel()
    {
        if (KoyunBasariPanel != null)
        {
            KoyunBasariPanel.SetActive(true); // Paneli aktif et

            // 4 saniye sonra paneli gizle
            StartCoroutine(HideBasariPanel());
        }
    }

    private IEnumerator HideBasariPanel()
    {
        yield return new WaitForSeconds(4f); // 4 saniye bekle

        if (KoyunBasariPanel != null)
        {
            KoyunBasariPanel.SetActive(false); // Paneli gizle
        }

        Debug.Log("Başarı paneli 4 saniye sonra kapatıldı.");
    }
}
