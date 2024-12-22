using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace WorldTime
{
    [RequireComponent(typeof(Light))]
    public class WorldLight : MonoBehaviour
    {
        [SerializeField] private WorldTime _worldTime; // WorldTime referansı
        [SerializeField] private Gradient _gradient; // Günün rengine göre ışık
        [SerializeField] private AnimationCurve _intensityCurve; // Günün yoğunluğuna göre ışık

        private Light _light;

        private void Awake()
        {
            // Light bileşenini al
            _light = GetComponent<Light>();

            // WorldTime referansı atanmadıysa hata ver
            if (_worldTime == null)
            {
                Debug.LogError("WorldTime reference is missing in WorldLight!");
                return;
            }

            // Event'e abone ol
            _worldTime.WorldTimeChanged += OnWorldTimeChanged;
        }

        private void OnDestroy()
        {
            // Event'ten ayrıl
            if (_worldTime != null)
                _worldTime.WorldTimeChanged -= OnWorldTimeChanged;
        }
        private float PercentOfDay(TimeSpan timeSpan)
        {
            // Gün içindeki toplam dakikayı al ve toplam dakika sayısına göre yüzdesini hesapla
            return (float)(timeSpan.TotalMinutes % WorldTimeConstans.MinutesInDay) / WorldTimeConstans.MinutesInDay;
        }
        private void OnWorldTimeChanged(object sender, TimeSpan newTime)
        {
            // Gün içindeki yüzdeyi hesapla
            float percentOfDay = (float)(newTime.TotalMinutes % WorldTimeConstans.MinutesInDay) / WorldTimeConstans.MinutesInDay;

            // Işık rengini ve yoğunluğunu güncelle
            _light.color = _gradient.Evaluate(percentOfDay);
            _light.intensity = _intensityCurve.Evaluate(percentOfDay);
        }
    }
}
