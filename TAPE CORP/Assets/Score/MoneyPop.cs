using UnityEngine;
using UnityEngine.UI;

public class CoinPopEffect : MonoBehaviour
{
    public float jumpHeight = 40f;
    public float duration = 0.3f;

    [Header("사운드")]
    public AudioClip coinSound;
    public float minPitch = 0.95f;
    public float maxPitch = 1.05f;
    public float volume = 1.0f;

    private RectTransform rect;
    private Vector2 startPos;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        startPos = rect.anchoredPosition;
    }

    public void StartPop()
    {
        PlaySound();
        StartCoroutine(JumpAndFade());
    }

    private void PlaySound()
    {
        if (coinSound == null) return;

        GameObject tempAudio = new GameObject("TempCoinAudio");
        tempAudio.transform.SetParent(transform);

        AudioSource src = tempAudio.AddComponent<AudioSource>();
        src.clip = coinSound;
        src.volume = volume;
        src.pitch = Random.Range(minPitch, maxPitch);
        src.playOnAwake = false;
        src.spatialBlend = 0f; // UI용 2D 사운드

        src.Play();
        Destroy(tempAudio, coinSound.length / src.pitch + 0.1f); // 피치 변경 고려해서 삭제
    }

    private System.Collections.IEnumerator JumpAndFade()
    {
        float t = 0f;
        Vector2 peak = startPos + Vector2.up * jumpHeight;

        Image img = GetComponent<Image>();
        Color startColor = img.color;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float progress = t / duration;

            float curve = Mathf.Sin(progress * Mathf.PI);
            rect.anchoredPosition = Vector2.Lerp(startPos, peak, curve);

            Color c = startColor;
            c.a = Mathf.Lerp(1f, 0f, progress);
            img.color = c;

            yield return null;
        }

        Destroy(gameObject);
    }
}
