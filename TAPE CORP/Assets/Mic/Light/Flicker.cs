using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Flicker : MonoBehaviour
{
    [Header("Light2D 할당")]
    [Tooltip("깜빡임 효과를 줄 Light2D 컴포넌트")]
    public Light2D targetLight;
    [Tooltip("테스트용 호출 최소 간격 (초)")]
    public float minTestInterval = 0.5f;
    [Tooltip("테스트용 호출 최대 간격 (초)")]
    public float maxTestInterval = 1.5f;
    [Tooltip("테스트 지속 시간 (초)")]
    public float testDuration = 10f;

    [Header("Flicker 설정")]
    [Tooltip("최소 밝기")]
    public float minIntensity = 0.02f;
    [Tooltip("최대 밝기 배율")]
    public float maxMultiplier = 4f;
    [Tooltip("한 번 깜빡임에 걸리는 시간 (초)")]
    public float flickerTime = 0.2f;
    [Tooltip("깜빡임 곡선 (0→1): 곡선에 따라 밝기가 보간됩니다")]
    public AnimationCurve flickerCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private float _baseIntensity;

    void Start()
    {
        if (targetLight == null)
        {
            Debug.LogError("Flicker: targetLight를 할당하세요.");
            enabled = false;
            return;
        }

        // 초기 밝기 설정
        _baseIntensity = minIntensity;
        targetLight.intensity = _baseIntensity;

        // 테스트용 코루틴
        StartCoroutine(TestFlicker());
    }

    /// <summary>
    /// 자연스러운 깜빡임을 한 번 실행합니다.
    /// </summary>
    public void FlickOnce()
    {
        StopCoroutine(nameof(DoFlicker));
        StartCoroutine(nameof(DoFlicker));
    }

    private IEnumerator DoFlicker()
    {
        // 깜빡임마다 최대 밝기 배율을 랜덤하게 뽑고
        float peak = Random.Range(1f, maxMultiplier);
        float half = flickerTime * 0.5f;
        float t = 0f;

        // 밝기가 base → base*peak → base 으로 곡선 보간
        while (t < flickerTime)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / flickerTime);
            // curve: 0→1, use symmetric up/down
            float curveVal = flickerCurve.Evaluate(normalized);
            // current multiplier: 1→peak→1
            float currentMul = Mathf.Lerp(1f, peak, curveVal);
            // apply intensity
            targetLight.intensity = _baseIntensity * currentMul;
            yield return null;
        }

        // 끝나면 반드시 원래 밝기로 복원
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
