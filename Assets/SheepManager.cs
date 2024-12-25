using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SheepManager : MonoBehaviour
{
    [SerializeField] private Transform target; // Takip edilen hedef
    [SerializeField] private float baseRadius = 2f; // Dairenin yarıçapı
    [SerializeField] private List<Sheep> sheepList = new List<Sheep>(); // Koyun listesi
    [SerializeField] private LayerMask groundLayer;

    private void Start()
    {
        foreach (Sheep sheep in FindObjectsOfType<Sheep>())
        {
            sheepList.Add(sheep);
        }
        
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
            // hedefi tıklanan noktaya eşitle
            target.position = hit.point;
        }
    }

    void ArrangeSheepInCircle()
    {
        if (sheepList.Count == 0) return;

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

    private void OnDrawGizmos()
    {
        if (target != null)
        {
            Gizmos.color = Color.green; // Çemberin rengini belirle
            Gizmos.DrawWireSphere(target.position, baseRadius); // Çemberi çiz
        }
    }
}
