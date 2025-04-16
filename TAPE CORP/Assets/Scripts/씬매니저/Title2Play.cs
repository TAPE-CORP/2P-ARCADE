using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title2Play : MonoBehaviour
{
    public Light2D[] light2D;
    public GameObject Title;
    public int a = 5;
    public float duration = 1.5f;

    public Image fadeImage;
    public AudioSource bgmSource;

    [Header("역동적 이동 커브")]
    public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private bool isTransitioning = false;

    void Update()
    {
        if (!isTransitioning && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(TransitionToPlayScene());
        }
    }

    IEnumerator TransitionToPlayScene()
    {
        isTransitioning = true;

        float elapsed = 0f;
        Vector3 startPos = Title.transform.position;
        Vector3 endPos = startPos + new Vector3(0, a, 0);

        float[] initialIntensities = new float[light2D.Length];
        for (int i = 0; i < light2D.Length; i++)
            initialIntensities[i] = light2D[i].intensity;

        Color fadeColor = fadeImage.color;
        fadeColor.a = 0;
        fadeImage.color = fadeColor;

        float initialVolume = bgmSource != null ? bgmSource.volume : 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float curveT = moveCurve.Evaluate(t);

            // Light
            for (int i = 0; i < light2D.Length; i++)
                light2D[i].intensity = Mathf.Lerp(initialIntensities[i], 0, t);

            // Title Movement (Curve 적용!)
            Title.transform.position = Vector3.Lerp(startPos, endPos, curveT);

            // Fade
            fadeColor.a = Mathf.Lerp(0, 1, t);
            fadeImage.color = fadeColor;

            // BGM Fade
            if (bgmSource != null)
                bgmSource.volume = Mathf.Lerp(initialVolume, 0, t);

            yield return null;
        }

        // 씬 전환
        SceneCon.Instance?.LoadScene("PlayScene");
    }
}
