using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ResourceCon : MonoBehaviour
{
    public static ResourceCon instance;

    [Header("Resource Settings")]
    [Tooltip("현재 자원량")]
    public float Gage = 100f;

    [Header("Minus Speed 설정")]
    [Tooltip("기본 감소 속도")]
    public float defaultMinusSpd = 1f;
    [Tooltip("움직임 1인당 곱해질 배수")]
    public float upSpd = 1.8f;

    [Header("Spacebar 감소량")]
    [Tooltip("스페이스바를 누를 때마다 감소시킬 자원량")]
    public float spaceDecreaseAmount = 10f;

    // 내부용: 플레이어별 움직임 상태 저장 (인덱스 0=P1, 1=P2)
    private bool[] moverStates = new bool[2];

    // 현재 실제로 사용할 값
    [HideInInspector]
    public float minusSpd;

    // 페이드용 데이터 구조체
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
        // 0) 스페이스바 입력으로 즉시 Gage 감소
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Gage -= spaceDecreaseAmount;
            Gage = Mathf.Max(Gage, 0f);  // 음수 방지
            Debug.Log($"스페이스바! Gage -{spaceDecreaseAmount:F1} → {Gage:F1}");
        }

        // 1) 매 프레임 자원 감소
        Gage -= minusSpd * Time.deltaTime;
        Gage = Mathf.Max(Gage, 0f);
        Debug.Log($"남은 자원 = {Gage:f1}, minusSpd = {minusSpd:f2}");
        if(Gage <= 0.1f)
        {
            GameManager.Instance.GameOver();
        }
        // 2) 번개 플래시 페이드아웃 처리
        for (int i = _flashingLights.Count - 1; i >= 0; i--)
        {
            var f = _flashingLights[i];
            f.elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(f.elapsed / f.duration);
            f.light.intensity = Mathf.Lerp(f.flashIntensity, f.originalIntensity, t);

            if (f.elapsed >= f.duration)
            {
                // 완전히 복원 후 리스트에서 제거
                f.light.intensity = f.originalIntensity;
                _flashingLights.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// ResourceTri에서 호출.
    /// BackGroundLight 레이어의 라이트를 즉시 flashIntensity로 올린 뒤,
    /// Update에서 duration만큼 서서히 원래 밝기로 페이드아웃합니다.
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
                // 이미 페이드 중인 라이트는 타이머만 리셋
                existing.flashIntensity = flashIntensity;
                existing.duration = flashDuration;
                existing.elapsed = 0f;
            }
            else
            {
                // 신규 플래시 등록
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
