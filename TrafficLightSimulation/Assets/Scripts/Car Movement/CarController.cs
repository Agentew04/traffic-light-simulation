using UnityEngine;

public class CarController : MonoBehaviour
{
    public enum Side { Top, Right, Bottom, Left } // Definir os 4 lados da encruzilhada
    public Side side;                         // O lado em que o carro está
    public float acceleration = 400f;          // Força de aceleração (valor padrão)
    public float maxSpeed = 40f;              // Velocidade máxima
    public float deceleration = 1000f;        // Força de desaceleração
    public TrafficLight trafficLight;         // Referência ao semáforo específico do lado

    private Rigidbody rb;                     // Componente Rigidbody

    void Start()
    {
        // Obtém o Rigidbody do carro
        rb = GetComponent<Rigidbody>();

        // Ajusta a aceleração de acordo com o lado
        AdjustAcceleration();
    }

    void Update()
    {
        // Verifica se o sinal está aberto (verde) para o lado correto
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

    // Ajusta a aceleração com base no lado
    void AdjustAcceleration()
    {
        // A aceleração será negativa para Bottom e Left, e positiva para Top e Right
        if (side == Side.Bottom || side == Side.Left)
        {
            acceleration = -400f; // Aceleração negativa para os lados Bottom e Left
        }
        else if (side == Side.Top || side == Side.Right)
        {
            acceleration = 400f;  // Aceleração positiva para os lados Top e Right
        }
    }
}
