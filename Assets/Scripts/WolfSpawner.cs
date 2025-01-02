using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;

public class WolfSpawner : MonoBehaviour
{
    public static WolfSpawner Instance { get; private set; } // Singleton

    [SerializeField] private Transform flockCenter;
    [SerializeField] private GameObject wolfPrefab;
    [SerializeField] public Transform spawnPoint;
    [SerializeField] private float spawnInterval = 10f; // Kurt oluşturma sıklığı
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Singleton nesnesi kendisi olarak ayarlanıyor
        }
        else
        {
            Destroy(gameObject); // Eğer bir instance varsa, bu nesneyi yok et
        }
    }

    private void Start()
    {
        InvokeRepeating(nameof(SpawnWolf), spawnInterval, spawnInterval); // Kurtları spawn etmek için periyodik çağrı
    }

    public void SetFlockTarget(Transform target)
    {
        flockCenter = target; // Sürü hedefini güncelle
    }

    private void SpawnWolf()
    {
        if (wolfPrefab != null)
        {
            // Ekranın dış sınırlarında bir rastgele spawn pozisyonu oluştur
            Vector3 randomSpawnPosition = new Vector3(
                Random.Range(-50f, 50f),
                0f, // Yükseklik
                Random.Range(-50f, 50f)
            );

            GameObject newWolf = Instantiate(wolfPrefab, randomSpawnPosition, Quaternion.identity);
            Wolf wolf = newWolf.GetComponent<Wolf>();
            
        }
    }
}