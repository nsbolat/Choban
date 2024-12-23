using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace WorldTime
{

    public class WorldTime : MonoBehaviour
    {
        public event EventHandler<TimeSpan> WorldTimeChanged;
        [SerializeField] private float _dayLength;

        private TimeSpan _currentTime = TimeSpan.Zero; // Günü 00:00'den başlat
        private int _currentDay = 1; // Gün sayacı
        private float _minuteLength => _dayLength / WorldTimeConstans.MinutesInDay;

        public event Action<int> OnDayChanged; // Gün değiştiğinde çağrılacak event

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
                _currentTime = TimeSpan.Zero; // Zamanı sıfırla
                IncrementDay(); // Gün sayısını artır
            }

            WorldTimeChanged?.Invoke(this, _currentTime);
            yield return new WaitForSeconds(_minuteLength);
            StartCoroutine(AddMinute());
        }

        public void AddTime(TimeSpan timeToAdd)
        {
            _currentTime += timeToAdd;

            // Gün değişimini kontrol et
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
            OnDayChanged?.Invoke(_currentDay); // Gün değişim event'ini çağır
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
