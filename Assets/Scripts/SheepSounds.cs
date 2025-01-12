using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepSounds : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;  // AudioSource bileşeni
    [SerializeField] private List<AudioClip> footStepSounds;  // Ayak sesleri listesi
    [SerializeField] private List<AudioClip> meeleSounds;  // Meleme sesleri listesi
    [SerializeField] private float minFootStepInterval = 0.4f;  // Minimum ayak sesi aralığı
    [SerializeField] private float maxFootStepInterval = 0.8f;  // Maksimum ayak sesi aralığı
    [SerializeField] private float minMeleeInterval = 2f;  // Minimum meleme sesi aralığı
    [SerializeField] private float maxMeleeInterval = 5f;  // Maksimum meleme sesi aralığı
    [SerializeField] private float minVolume = 0.6f;  // Sesin minimum seviyesi
    [SerializeField] private float maxVolume = 1f;  // Sesin maksimum seviyesi

    private void Start()
    {
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component not found on the child object!");
        }

        if (footStepSounds.Count == 0)
        {
            Debug.LogError("Footstep sounds list is empty!");
        }

        if (meeleSounds.Count == 0)
        {
            Debug.LogError("Melee sounds list is empty!");
        }

        // Her koyun için meleme sesi için Coroutine başlatıyoruz
        StartCoroutine(PlayRandomMeleeSound());
    }

    // Animasyon event'i ile çağrılacak fonksiyon
    public void PlayRandomFootStepSound()
    {
        if (audioSource != null && footStepSounds.Count > 0)
        {
            // Rastgele bir ses seç
            int randomIndex = Random.Range(0, footStepSounds.Count);
            AudioClip randomFootStep = footStepSounds[randomIndex];

            // Rastgele bir ses seviyesi belirle
            float randomVolume = Random.Range(minVolume, maxVolume);

            // Rastgele bir aralık belirle (adım aralığı)
            float randomInterval = Random.Range(minFootStepInterval, maxFootStepInterval);

            // Seçilen sesi çal
            audioSource.PlayOneShot(randomFootStep, randomVolume);

            // Bir sonraki ayak sesinin aralığını ayarlamak için gecikme ekle
            StartCoroutine(WaitForNextFootStep(randomInterval));
        }
    }

    // Bir sonraki ayak sesinin çalması için gecikme ekler
    private IEnumerator WaitForNextFootStep(float interval)
    {
        yield return new WaitForSeconds(interval);
    }

    // Koyunun rastgele zamanlarda meleme sesi çalması için Coroutine
    private IEnumerator PlayRandomMeleeSound()
    {
        // Sonsuz döngüde her koyun meleme yapacak
        while (true)
        {
            if (audioSource != null && meeleSounds.Count > 0)
            {
                // Rastgele bir meleme sesi seç
                int randomIndex = Random.Range(0, meeleSounds.Count);
                AudioClip randomMelee = meeleSounds[randomIndex];

                // Rastgele bir ses seviyesi belirle
                float randomVolume = Random.Range(minVolume, maxVolume);

                // Meleme sesini çal
                audioSource.PlayOneShot(randomMelee, randomVolume);

                // Rastgele bir aralık belirle (meleme sesi aralığı)
                float randomMeleeInterval = Random.Range(minMeleeInterval, maxMeleeInterval);

                // Bir sonraki meleme sesi için gecikme ekle
                yield return new WaitForSeconds(randomMeleeInterval);
            }
            else
            {
                yield return null;
            }
        }
    }
}
