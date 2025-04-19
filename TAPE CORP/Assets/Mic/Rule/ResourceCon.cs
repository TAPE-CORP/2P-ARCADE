using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ResourceCon : MonoBehaviour
{
    public static ResourceCon instance;

    [Header("Resource Settings")]
    [Tooltip("���� �ڿ���")]
    public float Gage = 100f;

    [Header("Minus Speed ����")]
    [Tooltip("�⺻ ���� �ӵ�")]
    public float defaultMinusSpd = 1f;
    [Tooltip("������ 1�δ� ������ ���")]
    public float upSpd = 1.8f;

    [Header("Spacebar ���ҷ�")]
    [Tooltip("�����̽��ٸ� ���� ������ ���ҽ�ų �ڿ���")]
    public float spaceDecreaseAmount = 10f;

    // ���ο�: �÷��̾ ������ ���� ���� (�ε��� 0=P1, 1=P2)
    private bool[] moverStates = new bool[2];

    // ���� ������ ����� ��
    [HideInInspector]
    public float minusSpd;

    // ���̵�� ������ ����ü
    private class FlashLightData
    {
        public Light2D light;
        public float originalIntensity;
        public float flashIntensity;
        public float duration;
        public float elapsed;
    }
    private List<FlashLightData> _flashingLights = new List<FlashLightData>();

    void Awake()
    {
        instance = this;
        minusSpd = defaultMinusSpd;
    }

    void Update()
    {
        // 0) �����̽��� �Է����� ��� Gage ����
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Gage -= spaceDecreaseAmount;
            Gage = Mathf.Max(Gage, 0f);  // ���� ����
            Debug.Log($"�����̽���! Gage -{spaceDecreaseAmount:F1} �� {Gage:F1}");
        }

        // 1) �� ������ �ڿ� ����
        Gage -= minusSpd * Time.deltaTime;
        Gage = Mathf.Max(Gage, 0f);
        Debug.Log($"���� �ڿ� = {Gage:f1}, minusSpd = {minusSpd:f2}");
        if(Gage <= 0.1f)
        {
            GameManager.Instance.GameOver();
        }
        // 2) ���� �÷��� ���̵�ƿ� ó��
        for (int i = _flashingLights.Count - 1; i >= 0; i--)
        {
            var f = _flashingLights[i];
            f.elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(f.elapsed / f.duration);
            f.light.intensity = Mathf.Lerp(f.flashIntensity, f.originalIntensity, t);

            if (f.elapsed >= f.duration)
            {
                // ������ ���� �� ����Ʈ���� ����
                f.light.intensity = f.originalIntensity;
                _flashingLights.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// ResourceTri���� ȣ��.
    /// BackGroundLight ���̾��� ����Ʈ�� ��� flashIntensity�� �ø� ��,
    /// Update���� duration��ŭ ������ ���� ���� ���̵�ƿ��մϴ�.
    /// </summary>
    public void RequestFlash(float flashIntensity, float flashDuration)
    {
        int bgLayer = LayerMask.NameToLayer("BackGroundLight");
        var allLights = FindObjectsOfType<Light2D>();

        foreach (var light in allLights)
        {
            if (light.gameObject.layer != bgLayer)
                continue;

            var existing = _flashingLights.Find(x => x.light == light);
            if (existing != null)
            {
                // �̹� ���̵� ���� ����Ʈ�� Ÿ�̸Ӹ� ����
                existing.flashIntensity = flashIntensity;
                existing.duration = flashDuration;
                existing.elapsed = 0f;
            }
            else
            {
                // �ű� �÷��� ���
                _flashingLights.Add(new FlashLightData
                {
                    light = light,
                    originalIntensity = light.intensity,
                    flashIntensity = flashIntensity,
                    duration = flashDuration,
                    elapsed = 0f
                });
                light.intensity = flashIntensity;
            }
        }
    }
}
