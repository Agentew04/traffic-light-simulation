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
    private bool isInStopZone = false;        // Indica se o carro está na zona de parada
    private TrafficLight stopZoneLight;       // Semáforo da zona de parada

    void Start()
    {
        // Obtém o Rigidbody do carro
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Verifica se o carro está na zona de parada e o semáforo está vermelho ou amarelo
        if (isInStopZone && stopZoneLight != null && !stopZoneLight.IsOpen)
        {
            // Aplica desaceleração
            if (rb.velocity.magnitude > 0)
            {
                Vector3 decelerationForce = deceleration * Time.deltaTime * -rb.velocity.normalized;
                rb.AddForce(decelerationForce);

                // Impede que a velocidade se torne negativa
                if (rb.velocity.magnitude < 0.1f)
                {
                    rb.velocity = Vector3.zero;
                }
            }
        }
        else
        {
            // O carro se move até a zona de parada, mesmo que o semáforo esteja vermelho
            if (rb.velocity.magnitude < maxSpeed)
            {
                rb.AddForce(acceleration * Time.deltaTime * transform.forward);
            }
        }
    }

    // Configura o estado da zona de parada
    public void SetStopZone(bool inZone, TrafficLight light)
    {
        isInStopZone = inZone;
        stopZoneLight = light;
    }


// Ajusta a aceleração com base no lado
public void AdjustAcceleration()
    {
        if (side == Side.Bottom || side == Side.Left)
        {
            acceleration = 400f; // Configuração para Bottom e Left
        }
        else if (side == Side.Top || side == Side.Right)
        {
            acceleration = 400f; // Configuração para Top e Right
        }
    }
}
