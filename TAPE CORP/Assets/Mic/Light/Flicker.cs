using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class Flicker : MonoBehaviour
{
    public enum LightState { InitialFade, Idle, Flicker }

    [Header("초기 페이드 설정")]
    [Tooltip("시작 시 밝기")]
    public float initialIntensity = 1f;
    [Tooltip("시작 밝기 → 0으로 줄어드는 시간 (초)")]
    public float initialFadeDuration = 5f;

    [Header("플릭커 설정 (Space)")]
    [Tooltip("스페이스바 눌렀을 때 즉시 올라갈 밝기")]
    public float flickerIntensity = 1f;
    [Tooltip("플릭커가 0으로 돌아가는 시간 (초)")]
    public float flickerDuration = 0.2f;

    private Light2D _light;
    private LightState _state;
    private float _timer;

    void Start()
    {
        _light = GetComponent<Light2D>();
        // 초기 상태 세팅
        _state = LightState.InitialFade;
        _timer = 0f;
        _light.intensity = initialIntensity;
    }

    void Update()
    {
        switch (_state)
        {
            case LightState.InitialFade:
                _timer += Time.deltaTime;
                float t0 = Mathf.Clamp01(_timer / initialFadeDuration);
                _light.intensity = Mathf.Lerp(initialIntensity, 0f, t0);
                if (t0 >= 1f)
                {
                    _state = LightState.Idle;
                }
                break;

            case LightState.Idle:
                // 페이드 끝난 후 대기 상태
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    _state = LightState.Flicker;
                    _timer = 0f;
                    _light.intensity = flickerIntensity;
                }
                break;

            case LightState.Flicker:
                _timer += Time.deltaTime;
                float t1 = Mathf.Clamp01(_timer / flickerDuration);
                _light.intensity = Mathf.Lerp(flickerIntensity, 0f, t1);
                if (t1 >= 1f)
                {
                    _state = LightState.Idle;
                }
                break;
        }
    }
}
