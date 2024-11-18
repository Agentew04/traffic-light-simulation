using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISemaphore : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField]
    private Image redImage;

    [SerializeField]
    private Image yellowImage;

    [SerializeField]
    private Image greenImage;

    [SerializeField]
    private TrafficLight trafficLight;

    // Update is called once per frame
    void Update()
    {
        if(trafficLight == null) {
            return;
        }
        redImage.gameObject.SetActive(trafficLight.lightState == TrafficLight.LightState.Red);
        yellowImage.gameObject.SetActive(trafficLight.lightState == TrafficLight.LightState.Yellow);
        greenImage.gameObject.SetActive(trafficLight.lightState == TrafficLight.LightState.Green);
    }
}
