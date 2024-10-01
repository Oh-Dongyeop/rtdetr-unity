using System.Collections;
using System.Collections.Generic;
using Enviro;
using UnityEngine;
using UnityEngine.UI;

public class RandomMovement : MonoBehaviour
{
    private DrawAndCapture DrawAndCapture => DrawAndCapture.Instance;
    
    private RandomEnviroManager RandomEnviroManager => RandomEnviroManager.Instance;

    public Collider areaCollider; // 이동할 수 있는 영역을 정의하는 콜라이더
    public List<GameObject> objectsToMove = new List<GameObject>(); // 이동시킬 오브젝트들

    public float moveInterval = 0.1f;  // 이동 간격 (초)
    public float yPosition = 0.1f;  // 고정된 Y 좌표
    public int repeatCount = 100;  // 반복할 횟수
    private int currentRepeat = 0;  // 현재 실행 횟수

    private bool isRunning = false;  // 현재 실행 상태    

    private Rect intervalRect = new Rect(10, 10, 200, 20);  // 인터벌 입력 필드 위치
    private Rect repeatCountRect = new Rect(10, 40, 200, 20);  // 반복 횟수 입력 필드 위치
    private Rect startCountRect = new Rect(10, 70, 200, 20);  // 반복 횟수 입력 필드 위치
    private Rect startButtonRect = new Rect(10, 110, 100, 30);  // 시작 버튼 위치
    private Rect stopButtonRect = new Rect(120, 110, 100, 30);  // 종료 버튼 위치
    private Rect statusRect = new Rect(10, 140, 300, 20);  // 상태 메시지 위치
    private Rect messageRect = new Rect(10, 170, 300, 20);  // 상태 메시지 위치

    private string intervalInput = "0.1";  // 사용자 입력 인터벌
    private string repeatCountInput = "100";  // 사용자 입력 반복 횟수
    private string startCountInput = "0"; // 시작 카운트
    private string statusText = "";  // 상태 메시지
    private string statusCount = "";

    void Start()
    {

         // 초기 상태 메시지 설정
        statusText = "Ready";  
        statusCount = "Count : 0";
    }

    private void OnGUI() {
        // 인터벌 입력 필드
        intervalInput = GUI.TextField(intervalRect, intervalInput, 25);
        GUI.Label(new Rect(220, 10, 100, 20), "Interval (s)");

        // 반복 횟수 입력 필드
        repeatCountInput = GUI.TextField(repeatCountRect, repeatCountInput, 25);
        GUI.Label(new Rect(220, 40, 100, 20), "Repeat Count");

        // 시작 카운트 입력
        startCountInput = GUI.TextField(startCountRect, startCountInput, 25);
        GUI.Label(new Rect(220, 70, 100, 20), "Start Count");        

        // 시작 버튼
        if (GUI.Button(startButtonRect, "Start"))
        {
            StartMovement();
        }

        // 종료 버튼
        if (GUI.Button(stopButtonRect, "Stop"))
        {
            StopMovement();
        }

        // 상태 메시지 출력
        GUI.Label(statusRect, $"Count : {currentRepeat}");

        // 상태 메시지 출력
        GUI.Label(messageRect, statusText);

    }

    // 이동 및 캡처 작업을 시작하는 함수
    public void StartMovement()
    {
        // 입력 필드에서 인터벌 및 반복 횟수를 가져옴
        if (!float.TryParse(intervalInput, out moveInterval))
        {
            moveInterval = 5f;  // 기본값 설정
        }
        
        if (!int.TryParse(repeatCountInput, out repeatCount))
        {
            repeatCount = 10;  // 기본값 설정
        }

        isRunning = true;

        // 시작 카운트 
        if (!int.TryParse(startCountInput, out currentRepeat))
        {
            currentRepeat = 0;  // 기본값 설정
        }

        // currentRepeat = startCountInput;

        statusText = $"Movement Started";

        // 지정된 간격으로 MoveObjects 메서드를 실행
        InvokeRepeating(nameof(MoveObjects), moveInterval, moveInterval);
    }

    // 이동 및 캡처 작업을 중단하는 함수
    public void StopMovement()
    {
        isRunning = false;
        CancelInvoke(nameof(MoveObjects));

        statusText = $"Movement Started";
    }

    // 오브젝트들을 랜덤으로 이동시키고 YOLO 캡처를 수행하는 함수
    void MoveObjects()
    {
        // 실행 횟수가 지정된 반복 횟수에 도달했는지 확인
        if (currentRepeat >= repeatCount)
        {
            CancelInvoke(nameof(MoveObjects));  // 더 이상 반복 실행하지 않음
            Debug.Log("MoveObjects 반복 종료");
            return;
        }

        // 각 오브젝트 이동 및 회전
        foreach (var obj in objectsToMove)
        {
            // 콜라이더 영역 안에서 랜덤 좌표 생성
            Vector3 randomPosition = GetRandomPositionWithinCollider(areaCollider);
            // Y 좌표는 고정
            randomPosition.y = yPosition;

            // 오브젝트의 위치를 랜덤 좌표로 이동
            obj.transform.position = randomPosition;

            // Y축만 랜덤 회전 적용
            float randomYRotation = Random.Range(0f, 360f);
            obj.transform.rotation = Quaternion.Euler(0, randomYRotation, 0);
        }

        // 랜덤환경 설정 
        RandomEnviroManager.RandomizeTime();
        RandomEnviroManager.RandomizeWeather();


        // 각 오브젝트 Rect 업데이트
        DrawAndCapture.ObjectRectUpdate();

         // YOLO 데이터 캡처 및 저장 (currentRepeat을 파일명으로 사용)
        DrawAndCapture.CaptureAndSaveYoloData(currentRepeat + 1);  // 파일명은 1부터 시작하도록 설정

        // 반복 횟수 증가
        currentRepeat++;
    }


    // 주어진 콜라이더 내에서 랜덤 위치를 반환하는 함수
    Vector3 GetRandomPositionWithinCollider(Collider col)
    {
        // 콜라이더의 경계 박스 가져오기
        Bounds bounds = col.bounds;

        // 경계 박스 내에서 랜덤 좌표 생성 (X와 Z만 랜덤)
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);

        // Y는 나중에 고정된 값으로 설정될 것이므로 임의로 반환
        return new Vector3(randomX, 0, randomZ);
    }
}
