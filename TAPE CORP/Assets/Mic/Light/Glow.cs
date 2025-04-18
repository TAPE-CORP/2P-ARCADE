using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class Glow : MonoBehaviour
{
    private Light2D glowLight;

    [Header("Intensity (밝기) 범위")]
    [Tooltip("최소 밝기")]
    public float minIntensity = 0.5f;
    [Tooltip("최대 밝기")]
    public float maxIntensity = 1.5f;

    [Header("Radius (크기) 범위")]
    [Tooltip("최소 반경")]
    public float minRadius = 0.5f;
    [Tooltip("최대 반경")]
    public float maxRadius = 2f;

    [Header("Flicker Timing (반짝임 빈도)")]
    [Tooltip("반짝임 간 최소 대기시간")]
    public float minFlickerTime = 0.05f;
    [Tooltip("반짝임 간 최대 대기시간")]
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
            // 1) 랜덤 밝기/반경 적용
            glowLight.intensity = Random.Range(minIntensity, maxIntensity);
            glowLight.pointLightOuterRadius = Random.Range(minRadius, maxRadius);

            // 2) 다음 반짝임까지 랜덤 대기
            float wait = Random.Range(minFlickerTime, maxFlickerTime);
            yield return new WaitForSeconds(wait);
        }
    }
}
