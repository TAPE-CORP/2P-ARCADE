using UnityEngine;

public class BeatScaleEffect : MonoBehaviour
{
    public float beatInterval = 1f; // ���� ���� (��)
    public float scaleAmount = 0.2f; // �ִ� ũ�� ��ȭ��
    public float speed = 2f; // ź�� ���� �ӵ�

    private Vector3 originalScale;
    private float nextBeatTime;

    void Start()
    {
        originalScale = transform.localScale;
        nextBeatTime = Time.time + beatInterval;
    }

    void Update()
    {
        // ���ڿ� ���缭 ũ�� ��ȭ ����
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

        // ���� ũ��� ����
        transform.localScale = originalScale;
    }
}
