using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MultiTargetCamera : MonoBehaviour
{
    [Header("타겟들")]
    public List<Transform> targets; // 인스펙터에서 보여지는 리스트
    private List<Transform> validTargets = new List<Transform>(); // 실시간 추적용 내부 리스트

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

        desiredPosition.x = Mathf.Clamp(desiredPosition.x, camWidth, 60f - camWidth);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, camHeight, 60f - camHeight);

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
}
