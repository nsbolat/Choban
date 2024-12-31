using System;
using UnityEngine;
using UnityEngine.AI;

public class Sheep : MonoBehaviour
{
    private NavMeshAgent navAgent;
    [SerializeField] private Animator sheepAnim;
    [SerializeField] private bool isEscaped = false;// Koyunun kaçıp kaçmadığını kontrol eder


    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.stoppingDistance = 0.1f;
    }

    private void Update()
    {
        if (sheepAnim != null && navAgent != null)
        {
            float currentSpeed = navAgent.velocity.magnitude;
            sheepAnim.SetFloat("Speed", currentSpeed);
        }

        // Oyuncuya belirli mesafede mi?
        if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) <= 5f)
        {
            if (Input.GetKeyDown(KeyCode.E) && isEscaped) // "E" tuşuna basıldı mı?
            {
                RejoinFlock();
            }
        }
    }

    public void MoveToPosition(Vector3 position)
    {
        if (navAgent != null)
        {
            navAgent.SetDestination(position);
        }
    }

    public void Escape(Vector3 escapePosition)
    {
        if (navAgent != null)
        {
            isEscaped = true;
            navAgent.SetDestination(escapePosition);
        }
    }

    private void RejoinFlock()
    {
        isEscaped = false;
        SheepManager.Instance.AddSheep(this);
        MoveToPosition(SheepManager.Instance.target.position);
        Debug.Log("Koyun sürüye geri katıldı!");
    }
    
    public void FollowTarget(Vector3 targetPosition)
    {
        if (!isEscaped) // Kaçma durumunda değilse
        {
            // Hedefe doğru hareket etmek için NavMeshAgent'ı kullan
            navAgent.SetDestination(targetPosition);
        }
    }
}