using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class Glow : MonoBehaviour
{
    private Light2D glowLight;

    [Header("Intensity (���) ����")]
    [Tooltip("�ּ� ���")]
    public float minIntensity = 0.5f;
    [Tooltip("�ִ� ���")]
    public float maxIntensity = 1.5f;

    [Header("Radius (ũ��) ����")]
    [Tooltip("�ּ� �ݰ�")]
    public float minRadius = 0.5f;
    [Tooltip("�ִ� �ݰ�")]
    public float maxRadius = 2f;

    [Header("Flicker Timing (��¦�� ��)")]
    [Tooltip("��¦�� �� �ּ� ���ð�")]
    public float minFlickerTime = 0.05f;
    [Tooltip("��¦�� �� �ִ� ���ð�")]
    public float maxFlickerTime = 0.2f;

    void Awake()
    {
        glowLight = GetComponent<Light2D>();
    }

    void OnEnable()
    {
        StartCoroutine(FlickerRoutine());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator FlickerRoutine()
    {
        while (true)
        {
            // 1) ���� ���/�ݰ� ����
            glowLight.intensity = Random.Range(minIntensity, maxIntensity);
            glowLight.pointLightOuterRadius = Random.Range(minRadius, maxRadius);

            // 2) ���� ��¦�ӱ��� ���� ���
            float wait = Random.Range(minFlickerTime, maxFlickerTime);
            yield return new WaitForSeconds(wait);
        }
    }
}
