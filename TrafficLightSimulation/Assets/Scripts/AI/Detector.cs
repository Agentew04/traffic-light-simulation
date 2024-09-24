using Assets.Scripts;
using Assets.Scripts.TextureProviders;
using NN;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Unity.Barracuda;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class Detector : MonoBehaviour
{
    [Tooltip("File of YOLO model.")]
    [SerializeField]
    protected NNModel ModelFile;

    [Tooltip("RawImage component which will be used to draw resuls.")]
    [SerializeField]
    protected RawImage ImageUI;

    [Range(0.0f, 1f)]
    [Tooltip("The minimum value of box confidence below which boxes won't be drawn.")]
    [SerializeField]
    protected float MinBoxConfidence = 0.3f;

    [SerializeField]
    protected TextureProviderType.ProviderType textureProviderType;

    [SerializeReference]
    protected TextureProvider textureProvider = null;

    protected NNHandler nn;
    protected Color[] colorArray = new Color[] { Color.red, Color.green, Color.blue, Color.cyan, Color.magenta, Color.yellow };

    YOLOv8 yolo;

    private Task detectionTask;
    private CancellationTokenSource cts;

    private void OnEnable()
    {
        nn = new NNHandler(ModelFile);
        yolo = new YOLOv8Segmentation(nn);

        textureProvider = GetTextureProvider(nn.model);
        textureProvider.Start();
        cts = new();
        Debug.Log("Starting task");
        detectionTask = Task.Run(YoloRun, cts.Token);
    }

    private void OnDestroy() {
        Debug.Log("Cancelling task");
        cts.Cancel();
    }

    private async Task YoloRun() {
        Debug.Log("task started");
        while (!cts.Token.IsCancellationRequested) {
            YOLOv8OutputReader.DiscardThreshold = MinBoxConfidence;
            Debug.Log("Getting texture");
            Texture2D texture = GetNextTexture();
            Debug.Log("Texture got");

            Debug.Log("running");
            var boxes = yolo.Run(texture);
            Debug.Log("ran");
            Debug.Log("drawing results");
            DrawResults(boxes, texture);
            Debug.Log("Results drawn");
            ImageUI.texture = texture;
            await Task.Delay(1);
        }
        Debug.Log("task ended");
    }

    protected TextureProvider GetTextureProvider(Model model)
    {
        var firstInput = model.inputs[0];
        int height = firstInput.shape[5];
        int width = firstInput.shape[6];

        TextureProvider provider;
        switch (textureProviderType)
        {
            case TextureProviderType.ProviderType.WebCam:
                provider = new WebCamTextureProvider(textureProvider as WebCamTextureProvider, width, height);
                break;

            case TextureProviderType.ProviderType.Video:
                provider = new VideoTextureProvider(textureProvider as VideoTextureProvider, width, height);
                break;
            case TextureProviderType.ProviderType.Camera:
                provider = new CameraTextureProvider(textureProvider as CameraTextureProvider, width, height);
                break;
            default:
                throw new InvalidEnumArgumentException();
        }
        return provider;
    }

    protected Texture2D GetNextTexture()
    {
        Debug.Log("getting next texture 2");
        return textureProvider.GetTexture();
    }

    void OnDisable()
    {
        nn.Dispose();
        textureProvider.Stop();
    }

    protected void DrawResults(IEnumerable<ResultBox> results, Texture2D img)
    {
        results.ForEach(box => DrawBox(box, img));
    }

    protected virtual void DrawBox(ResultBox box, Texture2D img)
    {
        Color boxColor = colorArray[box.bestClassIndex % colorArray.Length];
        int boxWidth = (int)(box.score / MinBoxConfidence);
        TextureTools.DrawRectOutline(img, box.rect, boxColor, boxWidth, rectIsNormalized: false, revertY: true);
    }

    private void OnValidate()
    {
        Type t = TextureProviderType.GetProviderType(textureProviderType);
        if (textureProvider == null || t != textureProvider.GetType())
        {
            if (nn == null)
                textureProvider = RuntimeHelpers.GetUninitializedObject(t) as TextureProvider;
            else
            {
                textureProvider = GetTextureProvider(nn.model);
                textureProvider.Start();
            }

        }
    }
}
