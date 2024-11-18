using UnityEngine;

public class CarUnspawner : MonoBehaviour
{
    public CarSpawner carSpawner; // Referência ao spawner de carros

    private void OnTriggerExit(Collider other)
    {
        // Verifique se o objeto que saiu é um carro usando a tag "Car"
        if (other.CompareTag("Car") || other.CompareTag("Gol"))
        {
            Debug.Log("Carro saiu da área: " + other.name);

            // Notifica o CarSpawner que o carro saiu
            if (other.TryGetComponent<CarController>(out CarController carController))
            {
                carSpawner.OnCarExit((int)carController.side);
            }

            // Destroi o carro quando ele sai da área
            Destroy(other.gameObject);
        }
    }
}
