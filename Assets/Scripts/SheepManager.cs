using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI elemanları için
using Random = UnityEngine.Random;

public class SheepManager : MonoBehaviour
{
    [SerializeField] private Transform target; // Takip edilen hedef
    [SerializeField] private float baseRadius = 2f; // Dairenin yarıçapı
    [SerializeField] private List<Sheep> sheepList = new List<Sheep>(); // Koyun listesi
    [SerializeField] private LayerMask groundLayer;

    // UI Elements
    [SerializeField] private RectTransform circleRectTransform; // Daire UI'si için referans
    [SerializeField] private float sheepRadius = 0.85f; // Koyun başına alan


    private void Start()
    {
        foreach (Sheep sheep in FindObjectsOfType<Sheep>())
        {
            sheepList.Add(sheep);
        }

        UpdateBaseRadius(); // Başlangıçta base radius hesapla
        UpdateCircleSize(); // UI dairesini güncelle
    }

    void Update()
    {
        // Sağ tıklama yapınca fonklar çalışsın
        if (Input.GetMouseButtonDown(1)) // 1: Sağ tık
        {
            UpdateTargetPosition();
            ArrangeSheepInCircle();
        }
    }

    void UpdateTargetPosition()
    {
        // Kameradan ray çıkar
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            // Hedefi tıklanan noktaya eşitle
            target.position = hit.point;
        }
    }

    void ArrangeSheepInCircle()
    {
        if (sheepList.Count == 0) return;

        UpdateBaseRadius(); // Koyun sayısına göre base radius güncelle
        UpdateCircleSize(); // Daireyi yeniden boyutlandır

        for (int i = 0; i < sheepList.Count; i++)
        {
            // Rastgele bir açı belirle
            float angle = Random.Range(0f, 360f);
            float angleRad = Mathf.Deg2Rad * angle;

            // Rastgele bir yarıçap belirle (çember içinde düzgün dağılım için sqrt kullan)
            float randomRadius = Mathf.Sqrt(Random.Range(0f, 1f)) * baseRadius;

            // Çember içinde rastgele bir pozisyon hesapla
            Vector3 circlePosition = new Vector3(
                target.position.x + randomRadius * Mathf.Cos(angleRad),
                target.position.y,
                target.position.z + randomRadius * Mathf.Sin(angleRad)
            );

            // Koyunu yeni pozisyona yönlendir
            sheepList[i].MoveToPosition(circlePosition);
        }
    }

    void UpdateBaseRadius()
    {
        // Koyun sayısına göre gereken yarıçapı hesapla
        baseRadius = Mathf.Sqrt(sheepList.Count) * sheepRadius;
    }

    void UpdateCircleSize()
    {
        if (circleRectTransform != null)
        {
            // Dairenin boyutunu, baseRadius'a göre ayarla
            float diameter = baseRadius * 4f; // Çapı hesapla
            circleRectTransform.sizeDelta = new Vector2(diameter, diameter); // UI dairesinin boyutunu güncelle
        }
    }

    // Koyun eklendiğinde çağrılacak metod
    public void AddSheep(Sheep newSheep)
    {
        if (!sheepList.Contains(newSheep))
        {
            sheepList.Add(newSheep);
            UpdateBaseRadius();
            UpdateCircleSize();
        }
    }

    // Koyun çıkarıldığında çağrılacak metod
    public void RemoveSheep(Sheep sheepToRemove)
    {
        if (sheepList.Contains(sheepToRemove))
        {
            sheepList.Remove(sheepToRemove);
            UpdateBaseRadius();
            UpdateCircleSize();
        }
    }

    private void OnDrawGizmos()
    {
        if (target != null)
        {
            Gizmos.color = Color.green; // Çemberin rengini belirle
            Gizmos.DrawWireSphere(target.position, baseRadius); // Çemberi çiz
        }
    }
}
