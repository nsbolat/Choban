using System.Collections.Generic;
using UnityEngine;

public class SheepManager : MonoBehaviour
{
    public Transform target; // Takip edilen hedef
    public float baseRadius = 2f; // Dairenin yarýçapý
    public List<Sheep> sheepList = new List<Sheep>(); // Koyun listesi

    void Update()
    {
        // Sað týklama yapýldýðýnda hedefi güncelle
        if (Input.GetMouseButtonDown(1)) // 1: Sað týk
        {
            UpdateTargetPosition();
            ArrangeSheepInCircle();

        }

        // Koyunlarý daire içinde düzenle

    }

    void UpdateTargetPosition()
    {
        // Kameradan bir ray çýkar
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Hedefi týklanan noktaya taþý
            target.position = hit.point;
        }
    }

    void ArrangeSheepInCircle()
    {
        if (sheepList.Count == 0) return;

        for (int i = 0; i < sheepList.Count; i++)
        {
            // Rastgele bir açý seç
            float angle = Random.Range(0f, 360f);
            float angleRad = Mathf.Deg2Rad * angle;

            // Rastgele bir yarýçap seç (0 ile baseRadius arasýnda)
            float randomRadius = Random.Range(0f, baseRadius);

            // Dairenin içindeki rastgele pozisyonu belirle
            Vector3 circlePosition = new Vector3(
                target.position.x + randomRadius * Mathf.Cos(angleRad),
                target.position.y,
                target.position.z + randomRadius * Mathf.Sin(angleRad)
            );

            // Koyunu daire içindeki pozisyona yönlendir
            sheepList[i].MoveToPosition(circlePosition);
        }
    }
}
