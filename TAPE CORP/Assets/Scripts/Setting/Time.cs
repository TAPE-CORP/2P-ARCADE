using UnityEngine;
using TMPro; // TextMeshPro ���
using UnityEngine.SceneManagement;

public class SimpleTimerDisplay : MonoBehaviour
{
    [Header("Ÿ�̸� ���� (�и���)")]
    public float timerDuration = 600000f; // 10�� = 600,000ms

    [Header("UI ����")]
    public TextMeshProUGUI timerText; // Ÿ�̸� �ؽ�Ʈ

    [Header("���� ����")]
    public Color normalColor = Color.white;
    public Color oneMinuteColor = new Color(1f, 0.5f, 0.5f);
    public Color thirtySecColor = new Color(1f, 0.3f, 0.3f);
    public Color tenSecColor = Color.red;

    [Header("���̵� �� ���� ����")]
    public CanvasGroup fadePanel;              // ȭ���� �˰� ���̵��� �г�
    public float fadeDuration = 2f;            // ���̵忡 �ɸ��� �ð�
    public AudioSource bgmSource;              // ������� ����� �ҽ�

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
            timer -= Time.deltaTime * 1000f; // �и��� ���� ����
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

        // ���̵� �� �г� �� ���� ���̵� �ƿ�
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

        // Ȯ���� �������� ����
        if (fadePanel != null)
            fadePanel.alpha = 1f;
        if (bgmSource != null)
            bgmSource.volume = 0f;

        Debug.Log("Ÿ�̸� ����, ���� ������ ��ȯ");
        // ���� ������ ��ȯ
        SceneManager.LoadScene("ScoreScene");
    }
}
