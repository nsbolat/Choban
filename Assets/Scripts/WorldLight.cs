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
        [SerializeField] private Vector3 _sunriseRotation; // Gündoğumu açısı
        [SerializeField] private Vector3 _sunsetRotation; // Günbatımı açısı
        [SerializeField] private float _rotationSmoothingSpeed = 5f; // Rotasyon geçiş hızını belirler

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
            float percentOfDay = PercentOfDay(newTime);

            // Işık rengini ve yoğunluğunu güncelle
            _light.color = _gradient.Evaluate(percentOfDay);
            _light.intensity = _intensityCurve.Evaluate(percentOfDay);

            // Işık açısını güncelle
            UpdateLightRotation(percentOfDay);
        }

        private void UpdateLightRotation(float percentOfDay)
        {
            // Gündoğumu ve günbatımı arasındaki rotasyonu hesapla
            Vector3 targetRotation = Vector3.Lerp(_sunriseRotation, _sunsetRotation, percentOfDay);

            // Şu anki rotasyonu hedef rotasyona doğru yumuşak bir şekilde kaydır
            Quaternion targetQuaternion = Quaternion.Euler(targetRotation);
            _light.transform.rotation = Quaternion.Slerp(
                _light.transform.rotation,
                targetQuaternion,
                Time.deltaTime * _rotationSmoothingSpeed
            );
        }
    }
}
