using UnityEngine;

public class BeatScaleEffect : MonoBehaviour
{
    public float beatInterval = 1f; // 박자 간격 (초)
    public float scaleAmount = 0.2f; // 최대 크기 변화량
    public float speed = 2f; // 탄력 반응 속도

    private Vector3 originalScale;
    private float nextBeatTime;

    void Start()
    {
        originalScale = transform.localScale;
        nextBeatTime = Time.time + beatInterval;
    }

    void Update()
    {
        // 박자에 맞춰서 크기 변화 실행
        if (Time.time >= nextBeatTime)
        {
            nextBeatTime = Time.time + beatInterval;
            StartCoroutine(ScaleBounce());
        }
    }

    System.Collections.IEnumerator ScaleBounce()
    {
        float timer = 0f;
        while (timer < beatInterval)
        {
            float scaleFactor = 1f + Mathf.Sin(timer * speed) * scaleAmount;
            transform.localScale = originalScale * scaleFactor;

            timer += Time.deltaTime;
            yield return null;
        }

        // 원래 크기로 복귀
        transform.localScale = originalScale;
    }
}
