using System;
using UnityEngine;
using UnityEngine.AI;

public class Sheep : MonoBehaviour
{
    private NavMeshAgent navAgent;
    [SerializeField] private Animator sheepAnim;

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.stoppingDistance = 0.1f; // Çok yakın bir mesafede durur
        
    }

    private void Update()
    {
        // NavMeshAgent'ın hızı Animator'a aktarılır
        if (sheepAnim != null && navAgent != null)
        {
            float currentSpeed = navAgent.velocity.magnitude; // NavMeshAgent'in anlık hızı
            sheepAnim.SetFloat("Speed", currentSpeed); // Animator'daki Speed parametresini güncelle
        }
    }

    public void MoveToPosition(Vector3 position)
    {
        if (navAgent != null)
        {
            navAgent.SetDestination(position);
        }
    }
}
