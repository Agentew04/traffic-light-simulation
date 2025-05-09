using UnityEngine;

public class StopZone : MonoBehaviour
{
    public TrafficLight trafficLight; // Referência ao semáforo associado a esta zona de parada

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto que entrou na zona é um carro
        if (other.TryGetComponent<CarController>(out CarController car))
        {
            // Passa a referência do semáforo para o carro
            car.SetStopZone(true, trafficLight);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Verifica se o objeto que saiu da zona é um carro
        if (other.TryGetComponent<CarController>(out CarController car))
        {
            // Remove a referência do semáforo para o carro
            car.SetStopZone(false, null);
        }
    }
}
