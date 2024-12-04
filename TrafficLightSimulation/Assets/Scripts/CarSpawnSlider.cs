using UnityEngine;
using UnityEngine.UI;

public class CarSpawnSlider : MonoBehaviour
{
    public Slider spawnRateSlider;       // Referência ao Slider
    public CarSpawner carSpawner;       // Referência ao CarSpawner

    void Start()
    {
        // Configura o Slider para chamar a função ao mudar de valor
        spawnRateSlider.onValueChanged.AddListener(UpdateSpawnRate);

        // Inicializa o Slider com valores padrões
        spawnRateSlider.value = 1f;
    }

    void UpdateSpawnRate(float value)
    {
        carSpawner.UpdateSpawnRate(value);
    }
}
