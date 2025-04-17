using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MultiTargetCamera : MonoBehaviour
{
    [Header("타겟들")]
    public List<Transform> targets = new List<Transform>();
    private List<Transform> validTargets = new List<Transform>();

    [Header("카메라 이동")]
    public float smoothTime = 0.2f;
    private Vector3 velocity;

    [Header("줌 설정")]
    public float minZoom = 5f;
    public float maxZoom = 20f;
    public float zoomSpeed = 5f;
    public float padding = 2.5f;

    [Header("X축 이동 제한")]
    public float minX = -7f;  // 이동 가능한 최소 X값
    public float maxX = 70f;  // 이동 가능한 최대 X값

    [Header("Y축 이동 제한")]
    public float minY = -Mathf.Infinity;  // 이동 가능한 최소 Y값
    public float maxY = Mathf.Infinity;   // 이동 가능한 최대 Y값

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        validTargets.Clear();
        foreach (var t in targets)
            if (t != null) validTargets.Add(t);
        if (validTargets.Count == 0) return;

        Move();
        Zoom();
    }

    void Move()
    {
        // 모든 타겟의 중앙 위치 계산
        Vector3 centerPoint = GetCenterPoint();
        Vector3 desiredPosition = centerPoint;
        desiredPosition.z = transform.position.z;

        // X축을 지정된 범위 내로 제한
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
        // Y축을 지정된 범위 내로 제한
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);

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
            bounds.Encapsulate(validTargets[i].position);

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
            bounds.Encapsulate(validTargets[i].position);

        return bounds.center;
    }

    public void AddTarget(Transform target)
    {
        if (target != null && !targets.Contains(target))
            targets.Add(target);
    }

    public void RemoveTarget(Transform target)
    {
        if (targets.Contains(target))
            targets.Remove(target);
    }
}
