using UnityEngine;
using UnityEngine.AI;

public class Sheep : MonoBehaviour
{
    private NavMeshAgent navAgent;

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.stoppingDistance = 0.1f; // �ok yak�n bir mesafede durur
    }

    public void MoveToPosition(Vector3 position)
    {
        if (navAgent != null)
        {
            navAgent.SetDestination(position);

            // Rotasyonu hedef pozisyona bakacak �ekilde ayarla
            Vector3 direction = position - transform.position;
            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
            }
        }
    }
}
