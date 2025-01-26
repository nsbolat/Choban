using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public GameObject carPrefab; // Araba prefab'ı
    public Transform spawnPoint; // Spawn noktası
    public float spawnInterval = 20f; // Araba spawn süresi
    public GameObject trafficLight; // Trafik ışığı objesi

    private void Start()
    {
        StartCoroutine(SpawnCarRoutine());
    }

    private IEnumerator SpawnCarRoutine()
    {
        while (true) 
        {
            SpawnCar();
            yield return new WaitForSeconds(spawnInterval); 
        }
    }

    private void SpawnCar()
    {
        if (carPrefab != null && spawnPoint != null && trafficLight != null)
        {
            GameObject newCar = Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation);
            Car_AI carAI = newCar.GetComponent<Car_AI>();
            carAI.trafficLight = trafficLight; // Trafik ışığını arabanın AI'sına atıyoruz
        }
    }
}