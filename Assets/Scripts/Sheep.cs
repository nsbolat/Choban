using System;
using UnityEngine;
using UnityEngine.AI;

public class Sheep : MonoBehaviour
{
    private NavMeshAgent navAgent;
    [SerializeField] private Animator sheepAnim;
    [SerializeField] private Animator _playerAnim;
    [SerializeField] public bool isEscaped = false;// Koyunun kaçıp kaçmadığını kontrol eder,
    [SerializeField] private SurvivalSystem _survivalSystem;
    [SerializeField] private GameObject deadSheepPrefab;
    public float deadbodyTime;


    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        _playerAnim = GameObject.FindWithTag("Köpek").GetComponent<Animator>();
        _survivalSystem = FindObjectOfType<SurvivalSystem>();
        navAgent.stoppingDistance = 0.1f;
        FollowTarget(GameObject.Find("suruTarget").transform.position);
    }

    private void FixedUpdate()
    {
        if (sheepAnim != null && navAgent != null)
        {
            float currentSpeed = navAgent.velocity.magnitude;
            sheepAnim.SetFloat("Speed", currentSpeed);
        }

        // Oyuncuya belirli mesafede mi?
        if (PlayerController.LocalInstance != null && Vector3.Distance(transform.position, PlayerController.LocalInstance.transform.position) <= 5f)
        {
            if (Input.GetKeyDown(KeyCode.Q)) // "Q" tuşuna basıldı mı?
            {
                //EscapeToSpawnPoint();
            }
        }

        if (_survivalSystem.isFeeding || _survivalSystem.isWatering)
        {
            sheepAnim.SetBool("otlama",true);
            sheepAnim.SetTrigger("Otla");
        }
        else
        {
            sheepAnim.SetBool("otlama",false);
            sheepAnim.ResetTrigger("Otla");
            
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
    
    public void Die()
    {
        // Mevcut konum ve rotasyonu kaydet
        Vector3 deathPosition = transform.position;
        Quaternion deathRotation = transform.rotation;

        // Ölü koyunu oluştur
        GameObject deadSheep = Instantiate(deadSheepPrefab, deathPosition, deathRotation);

        // Ölü koyunu belirli bir süre sonra yok et
        Destroy(deadSheep, deadbodyTime);

        // Canlı koyunu yok et
        Destroy(gameObject);
    }
}