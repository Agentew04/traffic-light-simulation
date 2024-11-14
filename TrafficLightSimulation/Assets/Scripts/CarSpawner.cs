using UnityEngine;
using System.Collections;

public class CarSpawner : MonoBehaviour
{
    public GameObject[] carPrefabs;          // Array de prefabs de carros
    public Transform[] spawnPoints;          // Posições para os carros aparecerem (4 lados da encruzilhada)
    public TrafficLight[] trafficLights;     // Referências aos semáforos de cada lado (Top, Right, Bottom, Left)
    public int maxCarsPerSide = 2;           // Máximo de carros por lado
    public float minSpawnTime = 1f;          // Tempo mínimo de spawn (em segundos)
    public float maxSpawnTime = 5f;          // Tempo máximo de spawn (em segundos)

    private int[] carsOnSide;                // Para armazenar o número de carros em cada lado

    void Start()
    {
        // Inicializa o array com o número de carros em cada lado
        carsOnSide = new int[spawnPoints.Length];
        StartCoroutine(SpawnCars());
    }

    IEnumerator SpawnCars()
    {
        while (true)
        {
            // Espera um tempo aleatório entre min e max para spawnar
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            // Tenta spawnar um carro
            TrySpawnCar();
        }
    }

    void TrySpawnCar()
    {
        // Escolher aleatoriamente um lado para spawnar o carro
        int spawnSide = Random.Range(0, spawnPoints.Length);

        // Verifica se ainda há espaço para mais carros nesse lado
        if (carsOnSide[spawnSide] < maxCarsPerSide)
        {
            // Escolher aleatoriamente um prefab de carro
            GameObject selectedCarPrefab = carPrefabs[Random.Range(0, carPrefabs.Length)];

            // Instancia o carro no lado escolhido
            GameObject car = Instantiate(selectedCarPrefab, spawnPoints[spawnSide].position, spawnPoints[spawnSide].rotation);

            // Atribui o semáforo correto ao carro com base no lado
            if (car.TryGetComponent<CarController>(out var carController))
            {
                carController.side = (CarController.Side)spawnSide; // Atribui o lado ao carro
                carController.trafficLight = trafficLights[spawnSide]; // Atribui o semáforo correto
            }

            carsOnSide[spawnSide]++;

            // Registra o número de carros no lado
            Debug.Log("Carro spawnado no lado " + spawnSide + ": " + selectedCarPrefab.name);
        }
        else
        {
            Debug.Log("Máximo de carros no lado " + spawnSide);
        }
    }

    // Função para ser chamada quando um carro sai da área, removendo o contador de carros no lado
    public void OnCarExit(int sideIndex)
    {
        if (carsOnSide[sideIndex] > 0)
        {
            carsOnSide[sideIndex]--;
            Debug.Log("Carro saiu, carros restantes no lado " + sideIndex + ": " + carsOnSide[sideIndex]);
        }
    }
}
