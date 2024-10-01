using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BoxRenderer : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField]
    private Detector detector;

    [SerializeField]
    private RenderTexture inputTexture;

    private RenderTexture outputTexture;
    public RenderTexture OutputTexture => outputTexture;

    [SerializeField]
    private ComputeShader computeShader;

    [SerializeField]
    private RawImage destinationImage;

    [SerializeField]
    private Color boxColor;

    [SerializeField]
    private float boxWidth;

    struct Box {
        public float x1, y1, x2, y2;
        public float lineWidth;
    }


    // Start is called before the first frame update
    void Start()
    {
        // create a new texture with same dimensions as inputtexture
        outputTexture = new RenderTexture(inputTexture);
        outputTexture.enableRandomWrite = true;
        outputTexture.Create();
    }

    private void OnDestroy() {
        Destroy(outputTexture);
    }

    // Update is called once per frame
    void Update()
    {
        var boxes = detector.PublicResultBoxes;
        Box[] b = boxes.Select(x => new Box() {
            x1 = x.rect.xMin,
            x2 = x.rect.xMax,
            y1 = x.rect.yMin,
            y2 = x.rect.yMax,
            lineWidth = boxWidth
        }).ToArray();

        if(b.Length == 0) {
            return;
        }

        ComputeBuffer boxBuffer = new(b.Length, sizeof(float) * 5);
        boxBuffer.SetData(b);

        //inputTexture.enableRandomWrite = true;

        Debug.Log($"Drawing {b.Length} boxes. W/H({inputTexture.width}/{inputTexture.height})");
        foreach(var box in b) {
            Debug.LogFormat($"Box: ({box.x1}; {box.y1}) ({box.x2}; {box.y2})");
        }
        int kernelHandle = computeShader.FindKernel("CSMain");
        computeShader.SetTexture(kernelHandle, "Input", inputTexture);
        computeShader.SetTexture(kernelHandle, "Output", outputTexture);
        computeShader.SetBuffer(kernelHandle, "Boxes", boxBuffer);
        computeShader.SetVector("boxColor", boxColor);

        int threadGroupsX = inputTexture.width / 8;
        int threadGroupsY = inputTexture.height / 8;
        computeShader.Dispatch(kernelHandle, threadGroupsX, threadGroupsY, 1);
        boxBuffer.Release();

        destinationImage.texture = outputTexture;
        destinationImage.SetAllDirty();
    }
}
