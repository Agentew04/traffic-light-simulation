using UnityEngine;

public class CarController : MonoBehaviour
{
    public float acceleration = 400f;      // Força de aceleração
    public float maxSpeed = 40f;           // Velocidade máxima
    public float deceleration = 1000f;      // Força de desaceleração
    public TrafficLight trafficLight;      // Referência ao script TrafficLight

    private Rigidbody rb;                  // Componente Rigidbody

    void Start()
    {
        // Obtém o Rigidbody do carro
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Verifica se o sinal está aberto (verde)
        if (trafficLight != null && trafficLight.IsOpen)
        {
            // Verifica se a velocidade do carro está abaixo da velocidade máxima
            if (rb.velocity.magnitude < maxSpeed)
            {
                // Aplica força contínua para mover o carro para frente
                rb.AddForce(acceleration * Time.deltaTime * transform.forward);
            }
        }
        else
        {
            // Se o sinal estiver amarelo ou vermelho e o carro ainda estiver em movimento
            if (rb.velocity.magnitude > 0)
            {
                // Aplica uma força de desaceleração oposta à direção do movimento
                Vector3 decelerationForce = deceleration * Time.deltaTime * -rb.velocity.normalized;
                rb.AddForce(decelerationForce);

                // Impede que a velocidade se torne negativa
                if (rb.velocity.magnitude < 0.1f)
                {
                    rb.velocity = Vector3.zero;
                }
            }
        }
    }
}
