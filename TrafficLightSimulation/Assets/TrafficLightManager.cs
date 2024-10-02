using System;
using UnityEngine;

public class TrafficLightManager : MonoBehaviour
{
    public TrafficLight[] trafficLights;
    public Detector[] detectors; // Referência ao Detector
    public int[] carCount; // Vetor para armazenar a quantidade de carros detectados
    public bool someTrafficLightIsOpen = false;

    private void Start()
    {
        // Encontra todos os objetos do tipo TrafficLight na cena
        trafficLights = FindObjectsOfType<TrafficLight>();

        // Encontra todos os objetos do tipo Detector na cena
        detectors = FindObjectsOfType<Detector>();

        // Inicializa o array carCount com o tamanho dos detectors
        carCount = new int[detectors.Length];

        // Chame o método para atualizar os semáforos periodicamente
        InvokeRepeating(nameof(UpdateTrafficLights), 0, 1.0f); // Atualiza a cada 1 segundo
    }

    private void OnLightChanged(TrafficLight trafficLight, TrafficLight.LightState lightState)
    {
        Debug.Log($"Traffic light {trafficLight.name} changed to {lightState}");
    }

    private void UpdateTrafficLights()
    {
        for (int i = 0; i < detectors.Length; i++)
        {
            carCount[i] = detectors[i].PublicResultBoxes.Count;
            //printar no debug a quantidade de carros detectados
            Debug.Log($"Detector {i} detected {carCount[i]} cars");
        }

        int totalCarCount = 0;
        foreach (int count in carCount)
        {
            totalCarCount += count;
        }

        // Atualiza o estado dos semáforos
        foreach (var trafficLight in trafficLights)
        {
            switch (trafficLight.lightState)
            {
                case TrafficLight.LightState.Red:
                    if (totalCarCount > 0)
                    {
                        trafficLight.lightState = TrafficLight.LightState.Yellow;
                        trafficLight.Open();
                    }
                    break;
                case TrafficLight.LightState.Yellow:
                    trafficLight.lightState = TrafficLight.LightState.Green;
                    trafficLight.Open();
                    break;
                case TrafficLight.LightState.Green:
                    trafficLight.lightState = TrafficLight.LightState.Red;
                    trafficLight.Open();
                    break;
            }

            OnLightChanged(trafficLight, trafficLight.lightState);
        }
    }
}
