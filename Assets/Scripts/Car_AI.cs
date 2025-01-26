using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
// + , ; = *
public class Car_AI : MonoBehaviour
{
    public float safeDistance = 5f; 
    public float carSpeed = 50f;   
    private float currentSpeed;    

    public GameObject trafficLight; 
    private TrafficLightController trafficLightController; 
    

    private bool isNearTrafficLight = false; 

    private void Start()
    {
        currentSpeed = carSpeed;
        
        if (trafficLight != null)
        {
            trafficLightController = trafficLight.GetComponent<TrafficLightController>();
        }
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
    }

    void Move()
    {
        currentSpeed = carSpeed;
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
        if (collision.gameObject.CompareTag("Köpek")) // Köpekle çarpışma
        {
            // Köpek ile çarpışmayı engelle
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());

            Debug.Log("Araba köpek ile çarpıştı ama etkilenmedi!");
        }
    }
}