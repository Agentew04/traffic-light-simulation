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
            bool parentDestroyed = false;
            // Notifica o CarSpawner que o carro saiu
            if (other.TryGetComponent(out CarController carController))
            {
                carSpawner.OnCarExit((int)carController.side);
                if(carController.Parent != carController.gameObject) {
                    // eh o golzinho com pivo!
                    Destroy(carController.Parent);
                    parentDestroyed = true;
                }
            }

            // Destroi o carro quando ele sai da área
            if (!parentDestroyed) {
                Destroy(other.gameObject);
            }
        }
    }
}
