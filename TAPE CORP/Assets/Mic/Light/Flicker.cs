using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Flicker : MonoBehaviour
{
    [Header("Light2D �Ҵ�")]
    [Tooltip("������ ȿ���� �� Light2D ������Ʈ")]
    public Light2D targetLight;
    [Tooltip("�׽�Ʈ�� ȣ�� �ּ� ���� (��)")]
    public float minTestInterval = 0.5f;
    [Tooltip("�׽�Ʈ�� ȣ�� �ִ� ���� (��)")]
    public float maxTestInterval = 1.5f;
    [Tooltip("�׽�Ʈ ���� �ð� (��)")]
    public float testDuration = 10f;

    [Header("Flicker ����")]
    [Tooltip("�ּ� ���")]
    public float minIntensity = 0.02f;
    [Tooltip("�ִ� ��� ����")]
    public float maxMultiplier = 4f;
    [Tooltip("�� �� �����ӿ� �ɸ��� �ð� (��)")]
    public float flickerTime = 0.2f;
    [Tooltip("������ � (0��1): ��� ���� ��Ⱑ �����˴ϴ�")]
    public AnimationCurve flickerCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private float _baseIntensity;

    void Start()
    {
        if (targetLight == null)
        {
            Debug.LogError("Flicker: targetLight�� �Ҵ��ϼ���.");
            enabled = false;
            return;
        }

        // �ʱ� ��� ����
        _baseIntensity = minIntensity;
        targetLight.intensity = _baseIntensity;

        // �׽�Ʈ�� �ڷ�ƾ
        StartCoroutine(TestFlicker());
    }

    /// <summary>
    /// �ڿ������� �������� �� �� �����մϴ�.
    /// </summary>
    public void FlickOnce()
    {
        StopCoroutine(nameof(DoFlicker));
        StartCoroutine(nameof(DoFlicker));
    }

    private IEnumerator DoFlicker()
    {
        // �����Ӹ��� �ִ� ��� ������ �����ϰ� �̰�
        float peak = Random.Range(1f, maxMultiplier);
        float half = flickerTime * 0.5f;
        float t = 0f;

        // ��Ⱑ base �� base*peak �� base ���� � ����
        while (t < flickerTime)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / flickerTime);
            // curve: 0��1, use symmetric up/down
            float curveVal = flickerCurve.Evaluate(normalized);
            // current multiplier: 1��peak��1
            float currentMul = Mathf.Lerp(1f, peak, curveVal);
            // apply intensity
            targetLight.intensity = _baseIntensity * currentMul;
            yield return null;
        }

        // ������ �ݵ�� ���� ���� ����
        targetLight.intensity = _baseIntensity;
    }

    private IEnumerator TestFlicker()
    {
        float elapsed = 0f;
        while (elapsed < testDuration)
        {
            FlickOnce();
            float wait = Random.Range(minTestInterval, maxTestInterval);
            yield return new WaitForSeconds(wait);
            elapsed += wait;
        }
    }
}
