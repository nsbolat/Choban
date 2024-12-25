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

        private void Awake()
        {
            // Inspector'dan gelen başlangıç saatini TimeSpan'e çevir
            _currentTime = TimeSpan.FromHours(_startHour);
        }

        private void Start()
        {
            StartCoroutine(AddMinute());
            WorldTimeChanged?.Invoke(this, _currentTime);
        }

        private IEnumerator AddMinute()
        {
            _currentTime += TimeSpan.FromMinutes(1);

            if (_currentTime.TotalMinutes >= WorldTimeConstans.MinutesInDay)
            {
                _currentTime = TimeSpan.Zero;
                IncrementDay();
            }

            WorldTimeChanged?.Invoke(this, _currentTime);
            yield return new WaitForSeconds(_minuteLength);
            StartCoroutine(AddMinute());
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
    }
}
