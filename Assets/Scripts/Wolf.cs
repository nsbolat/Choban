using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Wolf : MonoBehaviour
{
    private NavMeshAgent navAgent;
    [SerializeField] public Transform flockCenter; // Sürü merkezi
    [SerializeField] private float attackRange = 5f; // Saldırı menzili
    [SerializeField] private float attackInterval = 2f; // Her saldırı arasındaki süre
    [SerializeField] private float damagePerAttack = 10f; // Saldırı başına verilen hasar
    private float attackCooldown = 0f; // Saldırıların arasındaki bekleme süresi
    private bool isAttacking = false; // Saldırı durumu
    public Transform spawnPoint; // Kurtun spawn noktası
    private bool isEscaping = false; // Kaçış durumunu izlemek için

    private void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();

        // Eğer flockCenter atanmadıysa, SheepManager üzerinden ayarlayın
        if (SheepManager.Instance != null && flockCenter == null)
        {
            flockCenter = SheepManager.Instance.target;
        }

        // Kurtun spawn noktasını belirle
        spawnPoint = WolfSpawner.Instance != null ? WolfSpawner.Instance.spawnPoint : transform;
    }

    private void Update()
    {
        if (isEscaping) return; // Kaçıyorsa başka bir işlem yapma

        if (flockCenter != null)
        {
            // Sürüye doğru hareket et
            navAgent.SetDestination(flockCenter.position);

            // Sürüye olan mesafeyi kontrol et
            float distanceToFlock = Vector3.Distance(transform.position, flockCenter.position);

            if (distanceToFlock <= attackRange)
            {
                isAttacking = true;

                // Saldırı için bekleme süresini kontrol et
                if (attackCooldown <= 0f)
                {
                    AttackFlock();
                    attackCooldown = attackInterval; // Bekleme süresini sıfırla
                }
            }
            else
            {
                isAttacking = false;
            }

            // Bekleme süresini azalt
            attackCooldown -= Time.deltaTime;
        }
    }

    public void SetFlockCenter(Transform target)
    {
        flockCenter = target; // Kurtun sürü hedefini güncelle
    }

    private void AttackFlock()
    {
        Debug.Log("Kurt saldırıyor!");

        // Koyun sayısını azalt
        if (SheepManager.Instance != null)
        {
            SheepManager.Instance.DecreaseSheepCount();
        }

        // Kaçış işlemini başlat
        EscapeAfterAttack();
    }

    private void EscapeAfterAttack()
    {
        isEscaping = true;

        // Rastgele bir pozisyon belirle
        Vector3 randomEscapePosition = new Vector3(
            transform.position.x + Random.Range(20f, 50f),
            transform.position.y,
            transform.position.z + Random.Range(20f, 50f)
        );

        // Hedefi kaçış pozisyonuna ayarla
        navAgent.SetDestination(randomEscapePosition);

        // Kaçış hedefine ulaşıldığında yok et
        StartCoroutine(DestroyAfterEscape());
    }

    private IEnumerator DestroyAfterEscape()
    {
        // Hedefe ulaşana kadar bekle
        while (Vector3.Distance(transform.position, navAgent.destination) > 1f)
        {
            yield return null; // Bir sonraki frame'e kadar bekle
        }

        Debug.Log("Kurt saldırdıktan sonra kaçtı ve yok oldu.");
        Destroy(gameObject); // Kurtu yok et
    }
}