using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class Flicker : MonoBehaviour
{
    public enum LightState { InitialFade, Idle, Flicker }

    [Header("�ʱ� ���̵� ����")]
    [Tooltip("���� �� ���")]
    public float initialIntensity = 1f;
    [Tooltip("���� ��� �� 0���� �پ��� �ð� (��)")]
    public float initialFadeDuration = 5f;

    [Header("�ø�Ŀ ���� (Space)")]
    [Tooltip("�����̽��� ������ �� ��� �ö� ���")]
    public float flickerIntensity = 1f;
    [Tooltip("�ø�Ŀ�� 0���� ���ư��� �ð� (��)")]
    public float flickerDuration = 0.2f;

    private Light2D _light;
    private LightState _state;
    private float _timer;

    void Start()
    {
        _light = GetComponent<Light2D>();
        // �ʱ� ���� ����
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
                // ���̵� ���� �� ��� ����
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
