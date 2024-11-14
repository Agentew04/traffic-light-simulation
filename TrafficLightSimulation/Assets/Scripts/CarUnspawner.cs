using UnityEngine;

public class CarUnspawner : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        // Verifique se o objeto que saiu é um carro usando a tag "Car"
        if (other.CompareTag("Car"))
        {
            Debug.Log("Carro saiu da área: " + other.name);
            // Destroi o carro quando ele sai da área
            Destroy(other.gameObject);
        }
    }
}