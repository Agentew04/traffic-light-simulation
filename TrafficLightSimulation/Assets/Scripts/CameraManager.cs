using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private List<Camera> cameras = new();

    [SerializeField]
    private Camera defaultCamera;

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
        if (number < 0 || number >= cameras.Count) {
            Debug.LogError("Invalid camera number!");
            return;
        }

        // deactivate all cameras
        foreach (var camera in cameras) {
            camera.gameObject.SetActive(false);
        }
        defaultCamera.gameObject.SetActive(false);

        // activate selected
        currentCamera = number;
        Camera cam = cameras[number - 1];
        cam.gameObject.SetActive(true);
    }

    public void ExitPreview() {
        foreach (var camera in cameras) {
            camera.gameObject.SetActive(false);
        }
        defaultCamera.gameObject.SetActive(true);
        currentCamera = 0;
    }

    private void Awake() {
        if(cameras.Count != 4) {
            Debug.LogWarning($"Expected 4 cameras. Got {cameras.Count}!");
        }
    }
}
