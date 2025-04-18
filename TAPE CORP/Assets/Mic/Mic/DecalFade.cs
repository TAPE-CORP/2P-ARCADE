using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DecalFade : MonoBehaviour
{
    [Tooltip("잔상이 완전히 사라지기까지 걸리는 시간")]
    public float fadeDuration = 2f;

    private SpriteRenderer _sr;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float t = 0f;
        Color start = _sr.color;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            _sr.color = new Color(start.r, start.g, start.b, alpha);
            yield return null;
        }
        Destroy(gameObject);
    }
}
