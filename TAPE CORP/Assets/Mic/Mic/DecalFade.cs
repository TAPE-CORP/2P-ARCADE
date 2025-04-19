using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DecalFade : MonoBehaviour
{
    [Tooltip("잔상이 완전히 사라지기까지 걸리는 시간")]
    public float fadeDuration = 2f;
    [Tooltip("잔상이 퍼져나갈 최대 배율")]
    public float maxSpread = 1.5f;

    private SpriteRenderer _sr;
    private Vector3 _startScale;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _startScale = transform.localScale;   // 초기 스케일 보관
        StartCoroutine(FadeAndSpread());
    }

    private IEnumerator FadeAndSpread()
    {
        float t = 0f;
        Color startColor = _sr.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float ratio = Mathf.Clamp01(t / fadeDuration);

            // 1) 알파 페이드
            float alpha = Mathf.Lerp(1f, 0f, ratio);
            _sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            // 2) 부드럽게 퍼지듯 스케일 증가
            float scaleFactor = Mathf.Lerp(1f, maxSpread, ratio);
            transform.localScale = _startScale * scaleFactor;

            yield return null;
        }

        Destroy(gameObject);
    }
}
