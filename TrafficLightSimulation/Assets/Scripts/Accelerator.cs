using UnityEngine;

public class Accelerator : MonoBehaviour
{
    public float accelerationBoost = 450f;  // A aceleração desejada quando o carro entra na zona

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto que entrou no trigger é um carro
        if (other.CompareTag("Car"))
        {
            if (other.TryGetComponent<CarController>(out var carController))
            {
                // Chama o método SetAcceleration para mudar a aceleração para 400
                carController.SetAcceleration(accelerationBoost);
                Debug.Log("Aceleração aumentada para 450.");
            }
        }
    }

}
