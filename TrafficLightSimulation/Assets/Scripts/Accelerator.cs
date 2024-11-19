using UnityEngine;

public class Accelerator : MonoBehaviour
{
    public float accelerationBoost = 450f;  // A aceleração desejada quando o carro entra na zona
    public float accelerationBoostGol = -450f;

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se o objeto que entrou no trigger é um carro
        if (other.CompareTag("Car"))
        {
            if (other.TryGetComponent<CarController>(out var carController))
            {
                // Chama o método SetAcceleration para mudar a aceleração para 400
                carController.SetAcceleration(accelerationBoost);
                Debug.Log("Aceleração audi aumentada para 450.");
            }
        }
        if (other.CompareTag("Gol"))
        {
            if (other.TryGetComponent<CarController>(out var carController))
            {
                // Chama o método SetAcceleration para mudar a aceleração para 400
                carController.SetAcceleration(accelerationBoostGol);
                Debug.Log("Aceleração gol aumentada para 450.");
            }
        }
    }

}
