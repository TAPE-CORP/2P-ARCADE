using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Player1Con : MonoBehaviour
{
    public Camera P1Cam;

    [Header("������ Transform (Player �ڽ����� �μ���)")]
    public Transform flashlight;

    [Header("Light2D �Ҵ� & ���� �÷��� �÷�")]
    public Light2D flashlightLight;
    public Color electricColor = Color.cyan;

    [Header("Electric �±� �˻� �ݰ�")]
    [Tooltip("Light2D�� Outer Radius�� ����� ��� 0���� ����")]
    public float detectRadius = 0f;

    [Header("���� Ȱ��ȭ �Ⱓ (��)")]
    public float electricActiveTime = 3f;

    [Header("Ŭ�� �ڿ� ���� ����")]
    [Tooltip("Ŭ�� Ȧ�� ���� �� �⺻ �ʴ� ���ҷ�")]
    public float clickDrainBase = 1f;
    [Tooltip("Ŭ�� Ȧ�� �����ð��� �߰� �ʴ� ���ҷ�")]
    public float clickDrainAccel = 1f;

    private Vector3 _originalScale;
    private Color _originalColor;
    private float _originalOuterRadius;

    // Ŭ�� ���� �ð�
    private float clickHoldTime = 0f;

    void Start()
    {
        _originalScale = transform.localScale;

        if (flashlight == null)
            Debug.LogError("Flashlight Transform�� �Ҵ��ϼ���.");
        if (flashlightLight == null)
            Debug.LogError("Light2D ������Ʈ�� �Ҵ��ϼ���.");
        else
        {
            _originalColor = flashlightLight.color;
            _originalOuterRadius = flashlightLight.pointLightOuterRadius;
        }
    }

    void Update()
    {
        if (flashlight == null) return;

        // 1) ���콺 ���� ��ǥ & ����
        Vector3 mouseWorld = P1Cam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = flashlight.position.z;
        Vector3 dir = (mouseWorld - flashlight.position).normalized;

        // 2) ������ ȸ��
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        flashlight.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

        // 3) ĳ���� �¿� ����
        bool faceLeft = dir.x < 0f;
        transform.localScale = new Vector3(
            faceLeft ? -Mathf.Abs(_originalScale.x) : Mathf.Abs(_originalScale.x),
            _originalScale.y,
            _originalScale.z
        );

        // 4) Ŭ�� �� ����Ʈ �ݰ� Ȯ��
        if (Input.GetMouseButton(0))
        {
            flashlightLight.pointLightOuterRadius = _originalOuterRadius * 2f;
        }
        else
        {
            flashlightLight.pointLightOuterRadius = _originalOuterRadius;
        }

        // 5) Ŭ�� �� �ڿ�(Gage) ����
        if (Input.GetMouseButton(0))
        {
            clickHoldTime += Time.deltaTime;
            float clickDrainRate = clickDrainBase + clickDrainAccel * clickHoldTime;
            if (ResourceCon.instance != null)
            {
                ResourceCon.instance.Gage -= clickDrainRate * Time.deltaTime;
            }
        }
        else
        {
            clickHoldTime = 0f;
        }

        // 6) Ŭ�� ���� ���� ȿ��
        if (Input.GetMouseButtonDown(0))
            StartCoroutine(ElectricFlash());
    }

    private IEnumerator ElectricFlash()
    {
        // ����Ʈ ���� ����
        flashlightLight.color = electricColor;

        // Electric �±� ������Ʈ ó��
        float radius = detectRadius > 0f
            ? detectRadius
            : flashlightLight.pointLightOuterRadius;

        Collider2D[] hits = Physics2D.OverlapCircleAll(flashlight.position, radius);
        var toggledConveyors = new HashSet<Conveyor>();

        foreach (var col in hits)
        {
            if (!col.CompareTag("Electric"))
                continue;

            Conveyor conveyor = col.GetComponent<Conveyor>();
            if (conveyor != null && toggledConveyors.Add(conveyor))
            {
                conveyor.SetDirection();
            }
        }

        // ��� �� ���� ����
        yield return new WaitForSeconds(1f);
        flashlightLight.color = _originalColor;
    }

    private IEnumerator ActivateUserComponents(GameObject obj, float duration)
    {
        var components = obj
            .GetComponents<MonoBehaviour>()
            .Where(c =>
            {
                if (c == null) return false;
                var ns = c.GetType().Namespace;
                return string.IsNullOrEmpty(ns) || !ns.StartsWith("UnityEngine");
            })
            .ToArray();

        foreach (var comp in components)
        {
            if (comp.GetType().Name == nameof(Conveyor))
            {
                var conveyor = comp as Conveyor;
                if (conveyor != null)
                    conveyor.SetDirection();
            }
            comp.enabled = true;
        }

        yield return new WaitForSeconds(duration);

        foreach (var comp in components)
        {
            comp.enabled = false;
        }
    }
}
