using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Barking : MonoBehaviour
{
    [SerializeField] private AudioClip[] barkSheepSounds, barkWolfSounds; // Havlama sesleri
    [SerializeField] private AudioSource audioSource; // Ses çalacak kaynak
    [SerializeField] private Animator _playerAnim;

    private void Start()
    {
        // AudioSource referansını al
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        _playerAnim = GameObject.FindWithTag("Köpek").GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            _playerAnim.SetTrigger("Havla");
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            _playerAnim.SetTrigger("Havla2");
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
}