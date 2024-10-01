using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField]
    private List<RenderTexture> cameras = new();

    [SerializeField]
    private Camera defaultCamera;

    [SerializeField]
    private List<RawImage> cameraPreviews = new();

    [SerializeField]
    private RawImage cameraFocusImage;

    [SerializeField]
    private Button exitPreviewButton;

    [Header("Configuracoes")]

    /// <summary>
    /// The current camera. 0 means default and non-zero
    /// is a specific camera.
    /// </summary>
    int currentCamera = 0;

    /// <summary>
    /// Previews one of the cameras.
    /// </summary>
    /// <param name="number">The 1-based index of the camera</param>
    public void PreviewCamera(int number) {
        Debug.Log("Previewing camera " + number);
        if (number < 0 || number > cameras.Count) {
            Debug.LogError("Invalid camera number!");
            return;
        }

        if(number == 0) {
            ExitPreview();
            return;
        }

        currentCamera = number;
        foreach (var preview in cameraPreviews) {
            preview.gameObject.SetActive(false);
        }
        cameraFocusImage.gameObject.SetActive(true);
        //defaultCamera.gameObject.SetActive(false);
        cameraFocusImage.texture = cameras[number - 1];
        exitPreviewButton.gameObject.SetActive(true);
    }

    public void ExitPreview() {
        cameraFocusImage.gameObject.SetActive(false);
        foreach(var preview in cameraPreviews) {
            preview.gameObject.SetActive(true);
        }
        //defaultCamera.gameObject.SetActive(true);
        currentCamera = 0;
        exitPreviewButton.gameObject.SetActive(true);
    }

    private void Awake() {
        if(cameras.Count != cameraPreviews.Count) {
            Debug.LogWarning($"Esperava mesma quantidade de RenderTextures e Preview Images!");
        }
        for (int i = 0; i < cameras.Count; i++) {
            cameraPreviews[i].texture = cameras[i];
        }
    }
}
