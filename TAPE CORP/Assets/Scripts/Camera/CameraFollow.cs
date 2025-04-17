using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MultiTargetCamera : MonoBehaviour
{
    [Header("타겟들")]
    public List<Transform> targets = new List<Transform>(); // 인스펙터 + 런타임 추가용
    private List<Transform> validTargets = new List<Transform>(); // null 아닌 애들만 추적

    [Header("카메라 이동")]
    public float smoothTime = 0.2f;
    private Vector3 velocity;

    [Header("줌 설정")]
    public float minZoom = 5f;
    public float maxZoom = 20f;
    public float zoomSpeed = 5f;
    public float padding = 2.5f;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        // 매 프레임마다 살아있는 타겟만 유효 타겟 리스트에 추가
        validTargets.Clear();

        foreach (var t in targets)
        {
            if (t != null)
                validTargets.Add(t);
        }

        if (validTargets.Count == 0) return;

        Move();
        Zoom();
    }

    void Move()
    {
        Vector3 centerPoint = GetCenterPoint();
        Vector3 desiredPosition = centerPoint;
        desiredPosition.z = transform.position.z;

        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        // 그냥 Clamp 빼면 무제한 이동
        desiredPosition.x = centerPoint.x;
        desiredPosition.y = centerPoint.y;


        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
    }

    void Zoom()
    {
        float requiredSize = GetRequiredSize();
        float targetZoom = Mathf.Clamp(requiredSize, minZoom, maxZoom);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);
    }

    float GetRequiredSize()
    {
        var bounds = new Bounds(validTargets[0].position, Vector3.zero);
        for (int i = 1; i < validTargets.Count; i++)
        {
            bounds.Encapsulate(validTargets[i].position);
        }

        float height = bounds.size.y;
        float width = bounds.size.x / cam.aspect;

        return Mathf.Max(height, width) / 2f + padding;
    }

    Vector3 GetCenterPoint()
    {
        if (validTargets.Count == 1)
            return validTargets[0].position;

        var bounds = new Bounds(validTargets[0].position, Vector3.zero);
        for (int i = 1; i < validTargets.Count; i++)
        {
            bounds.Encapsulate(validTargets[i].position);
        }

        return bounds.center;
    }

    // 외부에서 타겟 추가용 메서드
    public void AddTarget(Transform target)
    {
        if (target != null && !targets.Contains(target))
        {
            targets.Add(target);
        }
    }

    // 외부에서 타겟 제거용 메서드 (필요 시)
    public void RemoveTarget(Transform target)
    {
        if (targets.Contains(target))
        {
            targets.Remove(target);
        }
    }
}
