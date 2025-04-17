using UnityEngine;
using TMPro; // TextMeshPro 사용
using UnityEngine.SceneManagement;

public class SimpleTimerDisplay : MonoBehaviour
{
    [Header("타이머 설정 (밀리초)")]
    public float timerDuration = 600000f; // 10분 = 600,000ms

    [Header("UI 참조")]
    public TextMeshProUGUI timerText; // 타이머 텍스트

    [Header("색상 설정")]
    public Color normalColor = Color.white;
    public Color oneMinuteColor = new Color(1f, 0.5f, 0.5f);
    public Color thirtySecColor = new Color(1f, 0.3f, 0.3f);
    public Color tenSecColor = Color.red;

    [Header("페이드 및 사운드 설정")]
    public CanvasGroup fadePanel;              // 화면을 검게 페이드할 패널
    public float fadeDuration = 2f;            // 페이드에 걸리는 시간
    public AudioSource bgmSource;              // 배경음악 오디오 소스

    private float timer;
    private bool hasTimerEnded = false;
    private float initialBgmVolume;

    void Start()
    {
        timer = timerDuration;
        if (timerText != null)
            timerText.color = normalColor;

        if (bgmSource != null)
            initialBgmVolume = bgmSource.volume;

        if (fadePanel != null)
        {
            fadePanel.alpha = 0f;
            fadePanel.interactable = false;
            fadePanel.blocksRaycasts = false;
        }
    }

    void Update()
    {
        if (!hasTimerEnded)
        {
            timer -= Time.deltaTime * 1000f; // 밀리초 단위 감소
            UpdateTimerText();
            UpdateTimerColor();

            if (timer <= 0f)
            {
                timer = 0f;
                hasTimerEnded = true;
                StartCoroutine(FadeOutAndLoad());
            }
        }
    }

    void UpdateTimerText()
    {
        if (timerText == null) return;

        int totalMs = Mathf.Max(0, (int)timer);
        int minutes = totalMs / 60000;
        int seconds = (totalMs % 60000) / 1000;
        int milliseconds = totalMs % 1000;

        timerText.text = $"{minutes:D2}:{seconds:D2}.{milliseconds:D3}";
    }

    void UpdateTimerColor()
    {
        if (timerText == null) return;

        if (timer <= 10000f)
            timerText.color = tenSecColor;
        else if (timer <= 30000f)
            timerText.color = thirtySecColor;
        else if (timer <= 60000f)
            timerText.color = oneMinuteColor;
        else
            timerText.color = normalColor;
    }

    System.Collections.IEnumerator FadeOutAndLoad()
    {
        float elapsed = 0f;

        // 페이드 인 패널 및 사운드 페이드 아웃
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            fadePanel.gameObject.SetActive(true);
            if (fadePanel != null)
                fadePanel.alpha = Mathf.Lerp(0f, 1f, t);

            if (bgmSource != null)
                bgmSource.volume = Mathf.Lerp(initialBgmVolume, 0f, t);

            yield return null;
        }

        // 확실히 마무리값 설정
        if (fadePanel != null)
            fadePanel.alpha = 1f;
        if (bgmSource != null)
            bgmSource.volume = 0f;

        Debug.Log("타이머 종료, 점수 씬으로 전환");
        // 점수 씬으로 전환
        SceneManager.LoadScene("ScoreScene");
    }
}
