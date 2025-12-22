using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Random = UnityEngine.Random; 

public class Barking : NetworkBehaviour
{
    [SerializeField] private AudioClip[] barkSheepSounds, barkWolfSounds; 
    [SerializeField] private AudioSource audioSource; 
    [SerializeField] private Animator _playerAnim;
    [SerializeField] private Image qCooldownImage; 
    [SerializeField] private Image eCooldownImage; 

    private float qCooldown = 0f; 
    private float eCooldown = 0f; 
    private float qCooldownDuration = 3f; 
    private float eCooldownDuration = 5f; 

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // Kendi animatörünü al
        if (_playerAnim == null)
            _playerAnim = GetComponent<Animator>();
            
        // Eğer UI referansları prefab'de yoksa ve scene'den bulunması gerekiyorsa:
        // Multiplayer'da UI genelde her oyuncu için ayrı Canvas gerektirir veya
        // Local oyuncu sahnedeki MainCanvas'ı bulup ikonlarını oraya atar.
        // Şimdilik null check ile geçelim, yoksa hata vermesin.
    }

    private void Update()
    {
        // Sadece kendi karakterimiz kontrol edelim
        if (!IsOwner) return;

        // Cooldown sürelerini azalt
        if (qCooldown > 0)
        {
            qCooldown -= Time.deltaTime;
            if (qCooldownImage != null)
                qCooldownImage.fillAmount = 1 - (qCooldown / qCooldownDuration);
        }

        if (eCooldown > 0)
        {
            eCooldown -= Time.deltaTime;
            if (eCooldownImage != null)
                eCooldownImage.fillAmount = 1 - (eCooldown / eCooldownDuration);
        }

        // E tuşu için cooldown kontrolü
        if (Input.GetKeyDown(KeyCode.E) && eCooldown <= 0)
        {
            _playerAnim.SetTrigger("Havla");
            eCooldown = eCooldownDuration; 
            if (eCooldownImage != null)
                eCooldownImage.fillAmount = 0; 
            Debug.Log("E tuşu kullanıldı. Cooldown başladı: " + eCooldownDuration + " saniye.");
            
            // Sesi herkese duyurmak için ServerRpc gerekebilir, şimdilik local çalışır.
        }

        // Q tuşu için cooldown kontrolü
        if (Input.GetKeyDown(KeyCode.Q) && qCooldown <= 0)
        {
            _playerAnim.SetTrigger("Havla2");
            qCooldown = qCooldownDuration; 
            if (qCooldownImage != null)
                qCooldownImage.fillAmount = 0; 
            Debug.Log("Q tuşu kullanıldı. Cooldown başladı: " + qCooldownDuration + " saniye.");
        }
    }

    // Havlama animasyon event'inde çağrılacak fonksiyon
    public void PlayBarkSheepSound()
    {
        if (barkSheepSounds.Length > 0 && audioSource != null)
        {
            AudioClip randomBark = barkSheepSounds[Random.Range(0, barkSheepSounds.Length)];
            audioSource.pitch = Random.Range(1.0f, 1.2f);
            audioSource.PlayOneShot(randomBark);
        }
    }

    public void PlayBarkWolfSound()
    {
        if (barkWolfSounds.Length > 0 && audioSource != null)
        {
            AudioClip randomBark = barkWolfSounds[Random.Range(0, barkWolfSounds.Length)];
            audioSource.pitch = Random.Range(1.0f, 1.2f);
            audioSource.PlayOneShot(randomBark);
        }
    }
}
