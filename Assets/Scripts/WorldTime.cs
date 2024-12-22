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
        private float _minuteLength => _dayLength / WorldTimeConstans.MinutesInDay;

        private void Start()
        {
            StartCoroutine(AddMinute());
            // Oyunu başlatır başlatmaz ilk zaman olayını tetikle
            WorldTimeChanged?.Invoke(this, _currentTime);
        }

        private IEnumerator AddMinute()
        {
            _currentTime += TimeSpan.FromMinutes(1);

            // Eğer zaman bir günü geçtiyse sıfırla
            if (_currentTime.TotalMinutes >= WorldTimeConstans.MinutesInDay)
            {
                _currentTime = TimeSpan.Zero;
                Debug.Log("New day started!");
            }

            // Olayı tetikle
            WorldTimeChanged?.Invoke(this, _currentTime);

            // Bir sonraki dakika için bekle
            yield return new WaitForSeconds(_minuteLength);
            StartCoroutine(AddMinute());
        }
    }
}
    
