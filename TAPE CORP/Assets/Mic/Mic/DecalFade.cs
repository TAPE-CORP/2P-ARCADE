using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DecalFade : MonoBehaviour
{
    [Tooltip("�ܻ��� ������ ���������� �ɸ��� �ð�")]
    public float fadeDuration = 2f;
    [Tooltip("�ܻ��� �������� �ִ� ����")]
    public float maxSpread = 1.5f;

    private SpriteRenderer _sr;
    private Vector3 _startScale;

    void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _startScale = transform.localScale;   // �ʱ� ������ ����
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

            // 1) ���� ���̵�
            float alpha = Mathf.Lerp(1f, 0f, ratio);
            _sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            // 2) �ε巴�� ������ ������ ����
            float scaleFactor = Mathf.Lerp(1f, maxSpread, ratio);
            transform.localScale = _startScale * scaleFactor;

            yield return null;
        }

        Destroy(gameObject);
    }
}
