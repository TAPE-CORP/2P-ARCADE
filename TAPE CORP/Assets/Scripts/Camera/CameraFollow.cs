using System.Collections.Generic;
using UnityEngine;

public class MultiTargetCamera : MonoBehaviour
{
    [Header("타겟들")]
    public List<Transform> targets;

    [Header("카메라 이동")]
    public float smoothTime = 0.2f;
    private Vector3 velocity;

    [Header("줌 설정")]
    public float minZoom = 5f;
    public float maxZoom = 15f;
    public float zoomLimiter = 50f;
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

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
    }

    void Zoom()
    {
        float greatestDistance = GetGreatestDistance();
        float newZoom = Mathf.Lerp(maxZoom, minZoom, greatestDistance / zoomLimiter);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newZoom, Time.deltaTime);
    }

    float GetGreatestDistance()
    {
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 1; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }
        return bounds.size.x > bounds.size.y ? bounds.size.x : bounds.size.y;
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
