using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class Light2DFade : MonoBehaviour
{
    [Tooltip("빛이 완전히 사라지기까지 걸리는 시간")]
    public float fadeDuration = 2f;

    private Light2D _light;
    private float _initialIntensity;

    void Awake()
    {
        _light = GetComponent<Light2D>();
        _initialIntensity = _light.intensity;
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            _light.intensity = Mathf.Lerp(_initialIntensity, 0f, t / fadeDuration);
            yield return null;
        }
        Destroy(gameObject);
    }
}
