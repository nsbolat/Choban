using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Wolf : MonoBehaviour
{
    private NavMeshAgent navAgent;
    [SerializeField] private SheepManager _sheepManager;
    [SerializeField] private Animator wolfAnim;
    [SerializeField] public Transform flockCenter; // Sürü merkezi
    [SerializeField] private float attackRange = 0f; // Saldırı menzili
    [SerializeField] private float attackInterval = 2f; // Her saldırı arasındaki süre
    [SerializeField] private float damagePerAttack = 10f; // Saldırı başına verilen hasar
    private float attackCooldown = 0f; // Saldırıların arasındaki bekleme süresi
    private bool isAttacking = false; // Saldırı durumu
    public Transform spawnPoint; // Kurtun spawn noktası
    private bool isEscaping = false; // Kaçış durumunu izlemek için
    [SerializeField] private GameObject deathSheep;

    private Vector3 initialSpawnPosition; // Kurtun spawn olduğu ilk nokta

    private void Start()
    {
        _sheepManager = FindObjectOfType<SheepManager>();
        navAgent = GetComponent<NavMeshAgent>();
        wolfAnim = GetComponent<Animator>();
        deathSheep.gameObject.SetActive(false);

        // Kurtun spawn noktasını belirle ve kaydet
        if (WolfSpawner.Instance != null)
        {
            spawnPoint = WolfSpawner.Instance.spawnPoint;
        }
        initialSpawnPosition = transform.position; // Spawn noktasını kaydet

        if (SheepManager.Instance != null && flockCenter == null)
        {
            flockCenter = GetClosestSheep();
        }
    }

    private void FixedUpdate()
    {
        float currentSpeed = navAgent.velocity.magnitude;
        wolfAnim.SetFloat("Speed", currentSpeed);
    }

    private void Update()
    {
        if (isEscaping) return; // Kaçıyorsa başka bir işlem yapma

        // Her frame'de en yakın koyunu tekrar kontrol et ve flockCenter'ı sürekli güncelle
        flockCenter = GetClosestSheep();

        // Eğer flockCenter hala null ise kaçışa başla
        if (flockCenter == null)
        {
            EscapeAfterAttack();
            return; // Kaçışa başladıktan sonra diğer işlemleri yapma
        }

        // NavMeshAgent ile koyuna doğru hedef belirle
        navAgent.SetDestination(flockCenter.position);
        navAgent.speed = 5.5f;

        // Koyuna olan mesafeyi hesapla
        float distanceToFlock = Vector3.Distance(transform.position, flockCenter.position);

        // Koyuna yeterince yaklaşıldığında saldırı başlat
        if (distanceToFlock <= attackRange)
        {
            if (attackCooldown <= 0f)
            {
                isAttacking = true;
                AttackFlock();  // Saldırıyı başlat
                deathSheep.gameObject.SetActive(true);
                attackCooldown = attackInterval; // Saldırılar arasındaki bekleme süresi
            }
        }
        else
        {
            isAttacking = false; // Saldırmıyor
        }

        // Saldırı bekleme süresini güncelle
        attackCooldown -= Time.deltaTime;
        
        if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) <= 5f)
        {
            if (Input.GetKeyDown(KeyCode.Q)) // "Q" tuşuna basıldı mı?
            {
                EscapeToSpawnPoint();
            }
        }
    }

    private void AttackFlock()
    {
        Debug.Log("Kurt saldırıyor!");

        // Koyun sayısını azalt
        if (SheepManager.Instance != null)
        {
            SheepManager.Instance.DecreaseSheepCount();
        }

        // Saldırdıktan sonra kaçışa başla
        EscapeAfterAttack();
    }


    private Transform GetClosestSheep()
    {
        float closestDistance = float.MaxValue;
        Transform closestSheep = null;

        foreach (var sheep in _sheepManager.sheepList)
        {
            float distance = Vector3.Distance(transform.position, sheep.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestSheep = sheep.transform;
            }
        }

        return closestSheep;
    }

    private void EscapeAfterAttack()
    {
        isEscaping = true;
        navAgent.speed = 10f;
        Vector3 randomEscapePosition = new Vector3(
            transform.position.x + Random.Range(20f, 50f),
            transform.position.y,
            transform.position.z + Random.Range(20f, 50f)
        );

        navAgent.SetDestination(randomEscapePosition);
        StartCoroutine(DestroyAfterEscape());
    }

    private void EscapeToSpawnPoint()
    {
        isEscaping = true;
        navAgent.speed = 10f;
        navAgent.SetDestination(initialSpawnPosition); // Go back to the spawn point
        Debug.Log("Kurt kaçarken spawn noktasına dönüyor.");
        StartCoroutine(DestroyAfterEscape()); // Destroy after reaching spawn point
    }

    private IEnumerator DestroyAfterEscape()
    {
        while (Vector3.Distance(transform.position, navAgent.destination) > 1f)
        {
            yield return null;
        }

        Debug.Log("Kurt saldırdıktan sonra kaçtı ve yok oldu.");
        Destroy(gameObject); // Destroy the wolf after it escapes
    }
}
