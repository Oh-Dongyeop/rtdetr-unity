using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DrawAndCapture : MonoBehaviour {

    public static DrawAndCapture Instance { get; private set;}

    public List<ObjectLabel> objectLabels = new List<ObjectLabel>();  // 오브젝트 리스트
    public string folderPath = "Dataset";  // 저장할 폴더 경로
    private Camera mainCamera;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        // 폴더가 없으면 생성
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // 메인 카메라 가져오기
        mainCamera = Camera.main;

        // 오브젝트 목록 초기화 (여기에 동적으로 추가할 수 있음)
        objectLabels.Add(new ObjectLabel(GameObject.Find("Soldier1"), 0));  // 클래스 ID가 0인 오브젝트
        objectLabels.Add(new ObjectLabel(GameObject.Find("Soldier2"), 1));  // 클래스 ID가 1인 오브젝트
        objectLabels.Add(new ObjectLabel(GameObject.Find("Soldier3"), 2));  // 클래스 ID가 1인 오브젝트
    }

    public void ObjectRectUpdate(){
        if (objectLabels.Count > 0)
        {
            // 각 오브젝트에 대해 Rect 업데이트
            foreach (var objectLabel in objectLabels)
            {
                objectLabel.UpdateRect();
            }
        }
    }

    private void OnGUI() {
        // 각 오브젝트에 대해 테두리만 있는 사각형 그리기
        foreach (var objectLabel in objectLabels)
        {
            if (objectLabel.rect != Rect.zero)
            {
                DrawOutline(objectLabel.rect, 2, Color.red);  // 두께 2의 빨간 테두리
            }
        }
    }

    // 테두리만 그리기 위한 함수
    private void DrawOutline(Rect rect, float thickness, Color color)
    {
        GUI.color = color;

        // 위쪽 선
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, thickness), Texture2D.whiteTexture);
        // 아래쪽 선
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), Texture2D.whiteTexture);
        // 왼쪽 선
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, thickness, rect.height), Texture2D.whiteTexture);
        // 오른쪽 선
        GUI.DrawTexture(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), Texture2D.whiteTexture);
    }

    // GameView 캡처 및 YOLO 라벨 데이터 생성
    public void CaptureAndSaveYoloData(int fileNubmer)
    {
        // FHD 해상도 설정
        int width = 1920;
        int height = 1080;

        // 캡처할 텍스처 생성
        RenderTexture rt = new RenderTexture(width, height, 24);
        mainCamera.targetTexture = rt;
        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
        mainCamera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenShot.Apply();
        mainCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        // 파일명 설정
        string fileName = $"{folderPath}/capture_{fileNubmer:D5}";

        // 이미지 저장
        byte[] bytes = screenShot.EncodeToJPG();
        File.WriteAllBytes(fileName + ".jpg", bytes);

        // YOLO 라벨 데이터 생성
        string labelData = GenerateYoloData();

        // 텍스트 파일로 라벨 저장
        File.WriteAllText(fileName + ".txt", labelData);

        Debug.Log($"Saved {fileName}.jpg and {fileName}.txt");
    }

    // YOLO 형식으로 모든 오브젝트의 라벨 데이터 생성
    private string GenerateYoloData()
    {
        string yoloData = "";

        // 각 오브젝트에 대해 라벨 데이터 생성
        foreach (var objectLabel in objectLabels)
        {
            yoloData += objectLabel.GenerateYoloData() + "\n";
        }

        return yoloData.Trim();  // 마지막에 불필요한 줄바꿈 제거
    }    
}