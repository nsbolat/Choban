using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace WorldTime
{
    public class WorldTime : MonoBehaviour
    {
        public event EventHandler<TimeSpan> WorldTimeChanged;

        [SerializeField, Tooltip("Bir günün uzunluğu (dakika cinsinden)")]
        private int _dayLengthInMinutes = 1440; // Varsayılan: 1440 dakika (1 gün)

        [SerializeField, Tooltip("Oyunun başlangıç saati (saat cinsinden)")]
        private int _startHour = 6; // Varsayılan: 06:00

        private TimeSpan _currentTime = TimeSpan.Zero;
        private int _currentDay = 1;

        private float _minuteLength => (_dayLengthInMinutes * 60f) / WorldTimeConstans.MinutesInDay; // Dakikayı saniyeye çevir

        public event Action<int> OnDayChanged;

        private bool isPaused = false; // Oyun duraklatıldı mı?

        [Header("Atmosfer Ses Ayarları")]
        [SerializeField] private AudioSource atmosphereAudioSource; // Sahnedeki AudioSource
        [SerializeField] private AudioClip dayClip; // Gündüz sesi
        [SerializeField] private AudioClip nightClip; // Gece sesi

        private void Awake()
        {
            _currentTime = TimeSpan.FromHours(_startHour);
        }

        private void Start()
        {
            StartCoroutine(AddMinute());
            WorldTimeChanged?.Invoke(this, _currentTime);
            UpdateAtmosphereSound(); // İlk başta sesin doğru ayarlanması
        }

        private IEnumerator AddMinute()
        {
            while (true)
            {
                if (!isPaused)
                {
                    _currentTime += TimeSpan.FromMinutes(1);

                    if (_currentTime.TotalMinutes >= WorldTimeConstans.MinutesInDay)
                    {
                        _currentTime = TimeSpan.Zero;
                        IncrementDay();
                    }

                    WorldTimeChanged?.Invoke(this, _currentTime);
                    UpdateAtmosphereSound(); // Zaman her değiştiğinde ses güncelle
                }

                yield return new WaitForSeconds(_minuteLength);
            }
        }

        public void AddTime(TimeSpan timeToAdd)
        {
            _currentTime += timeToAdd;

            while (_currentTime.TotalMinutes >= WorldTimeConstans.MinutesInDay)
            {
                _currentTime = _currentTime.Subtract(TimeSpan.FromMinutes(WorldTimeConstans.MinutesInDay));
                IncrementDay();
            }

            WorldTimeChanged?.Invoke(this, _currentTime);
            UpdateAtmosphereSound(); // Zaman eklendiğinde ses güncelle
        }

        private void IncrementDay()
        {
            _currentDay++;
            OnDayChanged?.Invoke(_currentDay);
            Debug.Log($"New day started! Day: {_currentDay}");
        }

        public int GetCurrentDay()
        {
            return _currentDay;
        }

        public TimeSpan GetCurrentTime()
        {
            return _currentTime;
        }

        // Pause sistemine eklenen fonksiyonlar
        public void PauseTime()
        {
            isPaused = true;
        }

        public void ResumeTime()
        {
            isPaused = false;
        }

        private void UpdateAtmosphereSound()
        {
            // Gündüz saatleri: 6:00 - 18:00
            if (_currentTime.Hours >= 6 && _currentTime.Hours < 18)
            {
                if (atmosphereAudioSource.clip != dayClip) // Gündüz sesi oynatılmıyorsa
                {
                    atmosphereAudioSource.clip = dayClip;
                    atmosphereAudioSource.Play();
                }
            }
            else // Gece saatleri: 18:00 - 6:00
            {
                if (atmosphereAudioSource.clip != nightClip) // Gece sesi oynatılmıyorsa
                {
                    atmosphereAudioSource.clip = nightClip;
                    atmosphereAudioSource.Play();
                }
            }
        }
    }
}
