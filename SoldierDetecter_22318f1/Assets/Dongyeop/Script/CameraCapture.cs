using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CameraCapture : MonoBehaviour
{
    [SerializeField]
    private int fileCounter;
    public KeyCode screenshotKey;
    public string folderPath = "/background/";  // 저장할 폴더 경로
    private Camera Camera
    {
        get
        {
            if (!_camera)
            {
                _camera = Camera.main;
            }
            return _camera;
        }
    }

    [SerializeField]
    private Camera _camera;

    private void Start() {
        // 폴더가 없으면 생성
        if (!Directory.Exists(Application.dataPath + folderPath))
        {
            Directory.CreateDirectory(Application.dataPath + folderPath);
        }

        Debug.Log(Application.dataPath + folderPath);
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(screenshotKey))
        {
            Capture();
        }
    }

    public void Capture()
    {
        RenderTexture activeRenderTexture = RenderTexture.active;
        RenderTexture.active = Camera.targetTexture;

        Camera.Render();

        Texture2D image = new Texture2D(Camera.targetTexture.width, Camera.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, Camera.targetTexture.width, Camera.targetTexture.height), 0, 0);
        image.Apply();
        RenderTexture.active = activeRenderTexture;

        byte[] bytes = image.EncodeToPNG();
        Destroy(image);

        File.WriteAllBytes(Application.dataPath + folderPath + fileCounter + ".png", bytes);
        fileCounter++;
    }
}
