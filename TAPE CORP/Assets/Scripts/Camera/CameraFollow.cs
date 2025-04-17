using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MultiTargetCamera : MonoBehaviour
{
    [Header("Ÿ�ٵ�")]
    public List<Transform> targets = new List<Transform>(); // �ν����� + ��Ÿ�� �߰���
    private List<Transform> validTargets = new List<Transform>(); // null �ƴ� �ֵ鸸 ����

    [Header("ī�޶� �̵�")]
    public float smoothTime = 0.2f;
    private Vector3 velocity;

    [Header("�� ����")]
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
        // �� �����Ӹ��� ����ִ� Ÿ�ٸ� ��ȿ Ÿ�� ����Ʈ�� �߰�
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

        // �׳� Clamp ���� ������ �̵�
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

    // �ܺο��� Ÿ�� �߰��� �޼���
    public void AddTarget(Transform target)
    {
        if (target != null && !targets.Contains(target))
        {
            targets.Add(target);
        }
    }

    // �ܺο��� Ÿ�� ���ſ� �޼��� (�ʿ� ��)
    public void RemoveTarget(Transform target)
    {
        if (targets.Contains(target))
        {
            targets.Remove(target);
        }
    }
}
