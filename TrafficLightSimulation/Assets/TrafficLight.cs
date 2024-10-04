using System;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField, Tooltip("O objeto da luz vermelha")]
    private MeshRenderer redLight;

    [SerializeField, Tooltip("O objeto da luz amarela")]
    private MeshRenderer yellowLight;

    [SerializeField, Tooltip("O objeto da luz verde")]
    private MeshRenderer greenLight;

    [SerializeField, Tooltip("Material das luzes")]
    private Material lightMaterial;

    [SerializeField, Tooltip("O tempo que o sinal fica no amarelo")]
    [Range(0, 5)]
    private float yellowDelay;

    public enum LightState
    {
        Red,
        Yellow,
        Green
    }

    public LightState lightState;

    public bool IsOpen = false;

    public float openTime; // Variável para armazenar o tempo de abertura

    private void Start()
    {
        redLight.material = lightMaterial;
        redLight.material.EnableKeyword("_LIGHT_RED");
        redLight.material.DisableKeyword("_LIGHT_YELLOW");
        redLight.material.DisableKeyword("_LIGHT_GREEN");
        redLight.material.SetInt("_IsPowered", 1);
        yellowLight.material = lightMaterial;
        yellowLight.material.DisableKeyword("_LIGHT_RED");
        yellowLight.material.EnableKeyword("_LIGHT_YELLOW");
        yellowLight.material.DisableKeyword("_LIGHT_GREEN");
        yellowLight.material.SetInt("_IsPowered", 0);
        greenLight.material = lightMaterial;
        greenLight.material.DisableKeyword("_LIGHT_RED");
        greenLight.material.DisableKeyword("_LIGHT_YELLOW");
        greenLight.material.EnableKeyword("_LIGHT_GREEN");
        greenLight.material.SetInt("_IsPowered", 0);
        IsOpen = false;
    }

    /// <summary>
    /// Abre imediatamente o sinal.
    /// </summary>
    public void Open()
    {
        redLight.material.SetInt("_IsPowered", 0);
        yellowLight.material.SetInt("_IsPowered", 0);
        greenLight.material.SetInt("_IsPowered", 1);
        IsOpen = true;
        lightState = LightState.Green;
        openTime = Time.time; // Registra o tempo de abertura
        OnOpen?.Invoke();
    }

    /// <summary>
    /// Sinaliza que vai fechar. Invoca o evento
    /// <see cref="OnClose"/> quando fechar.
    /// </summary>
    public void StartClosing()
    {
        redLight.material.SetInt("_IsPowered", 0);
        yellowLight.material.SetInt("_IsPowered", 1);
        greenLight.material.SetInt("_IsPowered", 0);
        IsOpen = false;
        openTime = 0;
        lightState = LightState.Yellow;
        Invoke(nameof(Close), yellowDelay);
    }

    /// <summary>
    /// Fecha o sinal imediatamente
    /// </summary>
    public void Close()
    {
        redLight.material.SetInt("_IsPowered", 1);
        yellowLight.material.SetInt("_IsPowered", 0);
        greenLight.material.SetInt("_IsPowered", 0);
        IsOpen = false;
        openTime = 0;
        lightState = LightState.Red;
        OnClose?.Invoke();
    }

    public event Action OnClose = null;
    public event Action OnOpen = null;

    // botoes no playmode pra testar
    [SerializeField, ButtonInvoke(nameof(Open))]
    private bool open;

    [SerializeField, ButtonInvoke(nameof(StartClosing))]
    private bool startclose;

    [SerializeField, ButtonInvoke(nameof(Close))]
    private bool close;
}
