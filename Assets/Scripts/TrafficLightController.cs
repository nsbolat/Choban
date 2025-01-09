using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLightController : MonoBehaviour
{
    public GameObject redLight;
    public GameObject yellowLight;
    public GameObject greenLight;

    public float redDuration = 5f;
    public float yellowDuration = 2f;
    public float greenDuration = 5f;

    private float timer;
    private enum TrafficLightState { Red, Yellow, Green }
    private TrafficLightState currentState;

    private void Start()
    {
        currentState = TrafficLightState.Red; 
        UpdateLights();
        timer = redDuration; 
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            ChangeLight(); 
        }
    }

    private void ChangeLight()
    {
        if (currentState == TrafficLightState.Red)
        {
            currentState = TrafficLightState.Yellow; 
            timer = yellowDuration; 
        }
        else if (currentState == TrafficLightState.Yellow)
        {
            currentState = TrafficLightState.Green; 
            timer = greenDuration; 
        }
        else if (currentState == TrafficLightState.Green)
        {
            currentState = TrafficLightState.Red; 
            timer = redDuration; 
        }

        UpdateLights(); 
    }
    
    private void UpdateLights()
    {
        redLight.SetActive(currentState == TrafficLightState.Red);
        yellowLight.SetActive(currentState == TrafficLightState.Yellow);
        greenLight.SetActive(currentState == TrafficLightState.Green);
    }

    public bool IsRedLight()
    {
        return currentState == TrafficLightState.Red;
    }

    public bool IsYellowLight()
    {
        return currentState == TrafficLightState.Yellow;
    }

    public bool IsGreenLight()
    {
        return currentState == TrafficLightState.Green;
    }
}