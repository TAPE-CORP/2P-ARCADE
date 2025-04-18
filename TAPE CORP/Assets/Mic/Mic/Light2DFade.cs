using UnityEngine;
using UnityEngine.Rendering.Universal;
using System;
using System.Collections;

public class Light2DFade : MonoBehaviour
{
    public Light2D light2D;
    public float fadeDuration = 1f;
    public float initialIntensity = 1f;

    public Action<GameObject> onFadeComplete;

    private Coroutine fadeCoroutine;

    void Awake()
    {
        if (light2D == null)
            light2D = GetComponent<Light2D>();
    }

    void OnDisable()
    {
        // ��Ȱ��ȭ �� �ڵ� ��ȯ
        onFadeComplete?.Invoke(gameObject);
    }

    public void Initialize()
    {
        gameObject.SetActive(true);
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        light2D.intensity = initialIntensity;
        fadeCoroutine = StartCoroutine(FadeRoutine());
    }

    IEnumerator FadeRoutine()
    {
        float elapsed = 0f;
        float startIntensity = initialIntensity;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            light2D.intensity = Mathf.Lerp(startIntensity, 0f, elapsed / fadeDuration);
            yield return null;
        }

        light2D.intensity = 0f;

        // ���̵� �Ϸ� �� �ڵ� ��Ȱ��ȭ (Ǯ�� ��ȯ��)
        gameObject.SetActive(false);
    }
}
