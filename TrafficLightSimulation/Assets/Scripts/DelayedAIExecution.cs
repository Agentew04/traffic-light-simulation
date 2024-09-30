using NN;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Barracuda;
using UnityEngine;

public class DelayedAIExecution : MonoBehaviour
{
    [Header("Configuracoes")]
    [SerializeField, Tooltip("Modelo de IA que sera executado.")]
    private NNModel model;

    private NNHandler nn;
    private Texture2D currentTexture;
    private bool isJobRunning = false;
    private YOLOv8OutputReader outputReader;

    public event System.Action<List<ResultBox>> OnFinished = null;

    private void OnEnable() {
        nn ??= new(model);
        outputReader ??= new();
    }

    private void OnDisable() {
        nn.Dispose();
    }

    public void StartExecution(Texture2D texture) {
        if (isJobRunning) {
            Debug.LogError("O modelo ja esta rodando!");
            return;
        }
        isJobRunning = true;
        currentTexture = texture;
        nn ??= new(model);
        outputReader ??= new();

        _ = StartCoroutine(Execute());
    }

    private IEnumerator Execute() {
        System.Diagnostics.Stopwatch sw = new();
        sw.Start();
        Debug.Log("Starting");
        var inputTensor = new Tensor(currentTexture);

        var output = nn.worker.Execute(inputTensor).PeekOutput();
        //yield return new WaitForCompletion(output);
        yield return new WaitForSeconds(1f);
        inputTensor.tensorOnDevice.Dispose();
        var outs = PeekOutputs().ToArray();
        Tensor boxesOutput = outs[0];
        List<ResultBox> boxes = outputReader.ReadOutput(boxesOutput).ToList();
        boxes = DuplicatesSupressor.RemoveDuplicats(boxes);
        foreach (var box in boxes) { 
            Debug.Log(box);
        }
        sw.Stop();
        //Debug.Log("Finished! Took: " + sw.Elapsed);
        OnFinished?.Invoke(boxes);
        isJobRunning = false;
    }

    private IEnumerable<Tensor> PeekOutputs() {
        foreach (string outputName in nn.model.outputs) {
            Tensor output = nn.worker.PeekOutput(outputName);
            yield return output;
        }
    }
}
