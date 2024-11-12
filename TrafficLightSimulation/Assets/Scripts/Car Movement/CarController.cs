using UnityEngine;

public class CarController : MonoBehaviour
{
    public float acceleration = 400f;  // Força de aceleração
    public float maxSpeed = 40f;       // Velocidade máxima

    private Rigidbody rb;              // Componente Rigidbody

    void Start()
    {
        // Obtém o Rigidbody do carro
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Verifica se a velocidade do carro está abaixo da velocidade máxima
        if (rb.velocity.magnitude < maxSpeed)
        {
            // Aplica força contínua para mover o carro para frente
            rb.AddForce(acceleration * Time.deltaTime * transform.forward);
        }
    }
}
