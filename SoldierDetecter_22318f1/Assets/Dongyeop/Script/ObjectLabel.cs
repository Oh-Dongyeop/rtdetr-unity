using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class ObjectLabel
{
    public GameObject obj;  // 오브젝트
    public int classId;     // 클래스 ID
    public Rect rect;       // 오브젝트의 Rect (화면 상 위치)

    public ObjectLabel(GameObject obj, int classId)
    {
        this.obj = obj;
        this.classId = classId;
        this.rect = Rect.zero;
    }

    // 오브젝트의 화면 좌표 Rect를 갱신
    public void UpdateRect(Camera camera)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            Vector3 screenPosMin = camera.WorldToScreenPoint(renderer.bounds.min);
            Vector3 screenPosMax = camera.WorldToScreenPoint(renderer.bounds.max);

            // Y 좌표 반전
            rect = new Rect(screenPosMin.x, Screen.height - screenPosMin.y, 
                            screenPosMax.x - screenPosMin.x, screenPosMin.y - screenPosMax.y);
        }
    }
    
    public void UpdateRect()
    {
        rect = GetObjectScreenBound(obj);
    }


    /// <summary>
    /// 개체가 화면상에 차지하는 영역 산출
    /// </summary>
    /// <param name="targetObject">대상 개체</param>
    private Rect GetObjectScreenBound(GameObject targetObject)
    {
        /**
        개체가 화면상에 차지하는 영역 산출
        개체가 화면상에 차지하는 영역 산출
        **/

        // BoxCollider 컴포넌트 가져오기
        BoxCollider boxCollider = targetObject.GetComponent<BoxCollider>();

        if (boxCollider != null)
        {
            // BoxCollider의 로컬 모서리 좌표를 가져오기
            Vector3[] localCorners = new Vector3[8] {
                new Vector3(boxCollider.center.x - boxCollider.size.x / 2, boxCollider.center.y - boxCollider.size.y / 2, boxCollider.center.z - boxCollider.size.z / 2),
                new Vector3(boxCollider.center.x + boxCollider.size.x / 2, boxCollider.center.y - boxCollider.size.y / 2, boxCollider.center.z - boxCollider.size.z / 2),
                new Vector3(boxCollider.center.x - boxCollider.size.x / 2, boxCollider.center.y + boxCollider.size.y / 2, boxCollider.center.z - boxCollider.size.z / 2),
                new Vector3(boxCollider.center.x + boxCollider.size.x / 2, boxCollider.center.y + boxCollider.size.y / 2, boxCollider.center.z - boxCollider.size.z / 2),
                new Vector3(boxCollider.center.x - boxCollider.size.x / 2, boxCollider.center.y - boxCollider.size.y / 2, boxCollider.center.z + boxCollider.size.z / 2),
                new Vector3(boxCollider.center.x + boxCollider.size.x / 2, boxCollider.center.y - boxCollider.size.y / 2, boxCollider.center.z + boxCollider.size.z / 2),
                new Vector3(boxCollider.center.x - boxCollider.size.x / 2, boxCollider.center.y + boxCollider.size.y / 2, boxCollider.center.z + boxCollider.size.z / 2),
                new Vector3(boxCollider.center.x + boxCollider.size.x / 2, boxCollider.center.y + boxCollider.size.y / 2, boxCollider.center.z + boxCollider.size.z / 2)
            };

            // 모서리 점들의 월드 좌표를 구하고 화면 좌표로 변환
            Vector3[] screenCorners = new Vector3[8];
            for (int i = 0; i < 8; i++)
            {
                screenCorners[i] = Camera.main.WorldToScreenPoint(targetObject.transform.TransformPoint(localCorners[i]));
            }

            // BoxCollider를 감싸는 최소 사각형을 찾기 위해 가장 작은 x, y 및 가장 큰 x, y 찾기
            float minX = screenCorners[0].x;
            float minY = screenCorners[0].y;
            float maxX = screenCorners[0].x;
            float maxY = screenCorners[0].y;

            for (int i = 1; i < 8; i++)
            {
                minX = Mathf.Min(minX, screenCorners[i].x);
                minY = Mathf.Min(minY, screenCorners[i].y);
                maxX = Mathf.Max(maxX, screenCorners[i].x);
                maxY = Mathf.Max(maxY, screenCorners[i].y);
            }

            // 화면 상에서 개체가 차지하는 사각형을 Rect로 생성하려 리턴
            return new Rect(minX, Screen.height - maxY, maxX - minX, maxY - minY);
        }
        else
        {
            return Rect.zero;
        }
    }    

    // YOLO 형식으로 라벨 데이터를 생성
    public string GenerateYoloData()
    {
        float centerX = (rect.xMin + rect.xMax) / 2f / Screen.width;
        float centerY = (rect.yMin + rect.yMax) / 2f / Screen.height;
        float rectWidth = rect.width / Screen.width;
        float rectHeight = rect.height / Screen.height;

        // YOLO 라벨 형식: classId centerX centerY width height
        return $"{classId} {centerX} {centerY} {rectWidth} {rectHeight}";
    }
}