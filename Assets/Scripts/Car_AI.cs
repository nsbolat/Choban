using UnityEngine;

public class Car_AI : MonoBehaviour
{
    public float safeDistance = 5f; 
    public float carSpeed = 50f;   
    private float currentSpeed;
    public bool isMoving;
    public GameObject gameoverPanel;

    public GameObject trafficLight; 
    private TrafficLightController trafficLightController; 
    
    private bool isNearTrafficLight = false; 

    [Header("Tekerlekler")]
    public Transform frontWheel; // Ön tekerlek
    public Transform backWheel;  // Arka tekerlek
    public float wheelRotationSpeed = 500f; // Tekerlek dönüş hızı

    private void Start()
    {
        currentSpeed = carSpeed;
        
        if (trafficLight != null)
        {
            trafficLightController = trafficLight.GetComponent<TrafficLightController>();
        }
        gameoverPanel = GameObject.Find("GameOver");
    }

    private void Update()
    {
        RaycastHit hit;
        bool isHit = Physics.Raycast(transform.position, transform.forward, out hit, safeDistance);

        if (isHit && hit.transform.CompareTag("car"))
        {
            Stop(); 
        }
        else if (isNearTrafficLight && (IsRedLight() || IsYellowLight())) 
        {
            Stop(); 
        }
        else
        {
            Move(); 
        }
        
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
        RotateWheels(); // Tekerlekleri döndür
    }

    private void RotateWheels()
    {
        if (frontWheel != null)
        {
            frontWheel.Rotate(Vector3.back * currentSpeed * wheelRotationSpeed * Time.deltaTime);
        }
        if (backWheel != null)
        {
            backWheel.Rotate(Vector3.back * currentSpeed * wheelRotationSpeed * Time.deltaTime);
        }
    }

    private bool IsYellowLight()
    {
        if (trafficLightController == null) return false;
        return trafficLightController.IsYellowLight();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * safeDistance);
    }

    void Stop()
    {
        currentSpeed = 0f;
        isMoving = false;
    }

    void Move()
    {
        currentSpeed = carSpeed;
        isMoving = true;

    }

    private bool IsRedLight()
    {
        if (trafficLightController == null) return false;
        return trafficLightController.IsRedLight();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("TrafficLightTrigger"))
        {
            isNearTrafficLight = true; 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("TrafficLightTrigger"))
        {
            isNearTrafficLight = false; 
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Köpek") && isMoving) // Köpekle çarpışma
        {
            gameoverPanel.SetActive(true);
            Time.timeScale = 0f;

            Debug.Log("Araba köpek ile çarpıştı ama etkilenmedi!");
        }
    }
}
