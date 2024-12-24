using System.Collections.Generic;
using UnityEngine;

public class SheepManager : MonoBehaviour
{
    public Transform target; // Takip edilen hedef
    public float baseRadius = 2f; // Dairenin yar��ap�
    public List<Sheep> sheepList = new List<Sheep>(); // Koyun listesi

    void Update()
    {
        // Sa� t�klama yap�ld���nda hedefi g�ncelle
        if (Input.GetMouseButtonDown(1)) // 1: Sa� t�k
        {
            UpdateTargetPosition();
            ArrangeSheepInCircle();

        }

        // Koyunlar� daire i�inde d�zenle

    }

    void UpdateTargetPosition()
    {
        // Kameradan bir ray ��kar
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Hedefi t�klanan noktaya ta��
            target.position = hit.point;
        }
    }

    void ArrangeSheepInCircle()
    {
        if (sheepList.Count == 0) return;

        for (int i = 0; i < sheepList.Count; i++)
        {
            // Rastgele bir a�� se�
            float angle = Random.Range(0f, 360f);
            float angleRad = Mathf.Deg2Rad * angle;

            // Rastgele bir yar��ap se� (0 ile baseRadius aras�nda)
            float randomRadius = Random.Range(0f, baseRadius);

            // Dairenin i�indeki rastgele pozisyonu belirle
            Vector3 circlePosition = new Vector3(
                target.position.x + randomRadius * Mathf.Cos(angleRad),
                target.position.y,
                target.position.z + randomRadius * Mathf.Sin(angleRad)
            );

            // Koyunu daire i�indeki pozisyona y�nlendir
            sheepList[i].MoveToPosition(circlePosition);
        }
    }
}
