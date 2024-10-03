using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrafficLightManager : MonoBehaviour
{
    public TrafficLight[] trafficLights;
    public Detector[] detectors; // Referência ao Detector
    public int[] carCount; // Vetor para armazenar a quantidade de carros detectados

    private bool someTrafficLightOpen = false;

    private List<TrafficLight> queue = new List<TrafficLight>();

    private void Start()
    {
        // Encontra todos os objetos do tipo TrafficLight na cena
        //trafficLights = FindObjectsOfType<TrafficLight>();

        // Encontra todos os objetos do tipo Detector na cena
        //detectors = FindObjectsOfType<Detector>();

        // Inicializa o array carCount com o tamanho dos detectors
        carCount = new int[detectors.Length];

        foreach (var trafficLight in trafficLights)
        {
            queue.Add(trafficLight);
        }

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
            // Printar no debug a quantidade de carros detectados
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
            if (!someTrafficLightOpen)
            {
                trafficLight.Open();
                someTrafficLightOpen = true;
            }
            if (trafficLight.IsOpen)
            {
                if (Time.time - trafficLight.openTime >= 5)
                {
                    trafficLight.StartClosing();
                    queue[0].Open();
                }
                
            }

            OnLightChanged(trafficLight, trafficLight.lightState);
        }

        // Ordena a fila de semáforos
        OrderTrafficLightsQueue();
    }

    private void OrderTrafficLightsQueue()
    {
        // Cria um dicionário para armazenar a contagem de carros por semáforo
        Dictionary<TrafficLight, int> trafficLightCarCount = new Dictionary<TrafficLight, int>();

        for (int i = 0; i < trafficLights.Length; i++)
        {
            trafficLightCarCount[trafficLights[i]] = carCount[i];
        }

        // Ordena a fila com base na contagem de carros
        queue = trafficLightCarCount.OrderByDescending(t => t.Value).Select(t => t.Key).ToList();

        // Garante que o semáforo atualmente aberto seja o último na fila
        var openTrafficLight = trafficLights.FirstOrDefault(t => t.IsOpen);
        if (openTrafficLight != null)
        {
            queue.Remove(openTrafficLight);
            queue.Add(openTrafficLight);
        }
    }
}
