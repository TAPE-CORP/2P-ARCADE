using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MultiTargetCamera : MonoBehaviour
{
    [Header("타겟들")]
    public List<Transform> targets;

    [Header("카메라 이동")]
    public float smoothTime = 0.2f;
    private Vector3 velocity;

    [Header("줌 설정")]
    public float minZoom = 5f;
    public float maxZoom = 20f;
    public float zoomSpeed = 5f;
    public float padding = 2f;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (targets.Count == 0) return;

        Move();
        Zoom();
    }

    void Move()
    {
        Vector3 centerPoint = GetCenterPoint();
        Vector3 desiredPosition = centerPoint;
        desiredPosition.z = transform.position.z;

        // 제한된 위치 보정
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        desiredPosition.x = Mathf.Clamp(desiredPosition.x, camWidth, 200f - camWidth);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, camHeight, 100f - camHeight);

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
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 1; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        float height = bounds.size.y;
        float width = bounds.size.x / cam.aspect;

        return Mathf.Max(height, width) / 2f + padding;
    }

    Vector3 GetCenterPoint()
    {
        if (targets.Count == 1)
            return targets[0].position;

        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 1; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.center;
    }
}
