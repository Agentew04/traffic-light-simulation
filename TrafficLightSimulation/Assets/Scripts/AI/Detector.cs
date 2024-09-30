using Assets.Scripts;
using Assets.Scripts.TextureProviders;
using NN;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Unity.Barracuda;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;
using UnityEngine.UIElements;

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
    [SerializeField]
    private DelayedAIExecution delayedExecution;

    private Task detectionTask;
    private CancellationTokenSource cts;

    public List<ResultBox> PublicResultBoxes { get; private set; } = new();

    private const int CAR_CLASS = 2;

    private void OnEnable()
    {
        nn = new NNHandler(ModelFile);
        yolo = new YOLOv8Segmentation(nn);


        textureProvider = GetTextureProvider(nn.model);
        textureProvider.Start();
        cts = new();
        _ = StartCoroutine(YoloRun());
    }

    private IEnumerator YoloRun() {
        while (!cts.Token.IsCancellationRequested) {
            YOLOv8OutputReader.DiscardThreshold = MinBoxConfidence;
            Texture2D texture = GetNextTexture();

            delayedExecution.StartExecution(texture);
            
            bool canLoopAgain = false;
            delayedExecution.OnFinished += (boxes) => {
                PublicResultBoxes = boxes
                    .Where(box => (box.bestClassIndex % colorArray.Length) == CAR_CLASS)
                    .Where(box => box.rect.x + box.rect.width < 320)
                    .Select(box => {
                        // a IA gera caixas com coords 0-640. Queremos caixas 16:9
                        Rect rect = box.rect;
                        rect.xMin = box.rect.xMin.Remap(new Vector2(0, 640), new Vector2(0, 1280));
                        rect.xMax = box.rect.xMin.Remap(new Vector2(0, 640), new Vector2(0, 1280));
                        rect.yMin = box.rect.yMin.Remap(new Vector2(0, 640), new Vector2(0, 720));
                        rect.yMax = box.rect.yMin.Remap(new Vector2(0, 640), new Vector2(0, 720));
                        return new ResultBox(rect, box.score, box.bestClassIndex);
                    })
                    .ToList();
                canLoopAgain = true;
            };
            while (!canLoopAgain) {
                yield return new WaitForNextFrameUnit();
            }
        }
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
        return textureProvider.GetTexture();
    }

    void OnDisable()
    {
        cts.Cancel();
        nn.Dispose();
        textureProvider.Stop();
    }

    protected void DrawResults(IEnumerable<ResultBox> results, Texture2D img)
    {
        PublicResultBoxes.Clear();
        results
            .Where(box => (box.bestClassIndex % colorArray.Length) == 2)
            .ForEach(box => DrawBox(box, img));
    }

    protected virtual void DrawBox(ResultBox box, Texture2D img)
    {
        Color boxColor = colorArray[box.bestClassIndex % colorArray.Length];
        int boxWidth = (int)(box.score / MinBoxConfidence);
        if ((box.bestClassIndex % colorArray.Length) == 2 && box.rect.x + box.rect.width < 320)
        {
            //TextureTools.DrawRectOutline(img, box.rect, boxColor, boxWidth, rectIsNormalized: false, revertY: true);
            PublicResultBoxes.Add(box);
        }
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
