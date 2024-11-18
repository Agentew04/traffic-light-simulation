using UnityEngine;

public class SecondaryStopZone : MonoBehaviour
{
    public StopZone primaryStopZone; // Referência à primeira zona de parada
    public TrafficLight trafficLight; // Referência ao semáforo associado a esta zona de parada

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto que entrou na zona é um carro
        if (other.TryGetComponent<CarController>(out CarController car))
        {
            // Se há um carro na zona de parada principal e o semáforo não está verde, configura o carro para parar
            if (PrimaryStopZoneHasCar() && !trafficLight.IsOpen)
            {
                car.SetStopZone(true, trafficLight);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Verifica se o objeto que saiu da zona é um carro
        if (other.TryGetComponent<CarController>(out CarController car))
        {
            // Só remove o estado de parada da segunda zona se o carro não estiver na zona principal
            if (!PrimaryStopZoneHasCar() && CarIsInPrimaryStopZone(car))
            {
                car.SetStopZone(false, null);
            }
        }
    }

    // Função que verifica se há carros na primeira zona de parada
    private bool PrimaryStopZoneHasCar()
    {
        Collider[] colliders = Physics.OverlapBox(primaryStopZone.transform.position, primaryStopZone.transform.localScale / 2);
        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent<CarController>(out CarController car))
            {
                return true;
            }
        }
        return false;
    }

    // Função que verifica se um carro está atualmente na zona principal
    private bool CarIsInPrimaryStopZone(CarController car)
    {
        Collider[] colliders = Physics.OverlapBox(primaryStopZone.transform.position, primaryStopZone.transform.localScale / 2);
        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent<CarController>(out CarController primaryCar) && primaryCar == car)
            {
                return true;
            }
        }
        return false;
    }
}
