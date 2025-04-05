using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class LampFlicker : MonoBehaviour
{
    public Light2D light2D;
    public float minIntensity = 0.2f;
    public float maxIntensity = 1.0f;

    public float minFlickerDuration = 0.5f;
    public float maxFlickerDuration = 2.0f;

    private bool isFlickering = false;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isFlickering && GetComponent<Rigidbody2D>() != null)
        {
            float impactForce = collision.relativeVelocity.magnitude;

            // �浹 ������ 0~10 ���̷� ����ȭ (�ʿ� �� ����)
            float normalizedImpact = Mathf.Clamp01(impactForce / 10f);

            // �浹 ������ ���� ������ �ð� ����
            float flickerDuration = Mathf.Lerp(minFlickerDuration, maxFlickerDuration, normalizedImpact);

            StartCoroutine(Flicker(flickerDuration));
        }
    }

    IEnumerator Flicker(float duration)
    {
        isFlickering = true;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            light2D.intensity = Random.Range(minIntensity, maxIntensity);
            light2D.enabled = Random.value > 0.2f;

            float waitTime = Random.Range(0.05f, 0.2f);
            yield return new WaitForSeconds(waitTime);
            elapsed += waitTime;
        }

        light2D.intensity = maxIntensity;
        light2D.enabled = true;
        isFlickering = false;
    }
}
