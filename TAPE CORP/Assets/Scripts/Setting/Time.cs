using UnityEngine;
using TMPro; // ���� �߰�

public class TimedPanelAndBoard : MonoBehaviour
{
    [Header("Ÿ�̸� ���� (�и���)")]
    public float timerDuration = 600000f; // 10�� = 600,000ms

    [Header("UI ����")]
    public TextMeshProUGUI timerText; // <- TMP�� ����

    [Header("�г� ����")]
    public CanvasGroup fadePanel; // ���̵��� UI �г�
    public float fadeDuration = 2f; // ���̵忡 �ɸ��� �ð�

    [Header("�� ����")]
    public Transform board; // ������ ������Ʈ
    public Vector3 targetPosition; // ���� ��ġ
    public float boardDropSpeed = 2f;

    private float timer;
    private bool hasTimerEnded = false;
    private Vector3 boardStartPosition;

    void Start()
    {
        timer = timerDuration;

        if (board != null)
        {
            boardStartPosition = board.position + Vector3.up * 10f; // ���� ���ܵα�
            board.position = boardStartPosition;
        }
    }

    void Update()
    {
        if (!hasTimerEnded)
        {
            timer -= Time.deltaTime * 1000f; // �и��� ���� ����
            UpdateTimerText();

            if (timer <= 0f)
            {
                hasTimerEnded = true;
                timer = 0;
                StartCoroutine(FadeOutThenDrop());
            }
        }
    }

    void UpdateTimerText()
    {
        if (timerText != null)
        {
            int totalMs = Mathf.Max(0, (int)timer);
            int minutes = totalMs / 60000;
            int seconds = (totalMs % 60000) / 1000;
            int milliseconds = totalMs % 1000;
            timerText.text = $"{minutes:D2}:{seconds:D2}.{milliseconds:D3}";
        }
    }

    System.Collections.IEnumerator FadeOutThenDrop()
    {
        // �г� ���̵�
        if (fadePanel != null)
        {
            float elapsed = 0f;
            float startAlpha = fadePanel.alpha;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                fadePanel.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeDuration);
                yield return null;
            }

            fadePanel.alpha = 0f;
            fadePanel.interactable = false;
            fadePanel.blocksRaycasts = false;
        }

        // �� ��������
        while (board != null && Vector3.Distance(board.position, targetPosition) > 0.01f)
        {
            board.position = Vector3.MoveTowards(board.position, targetPosition, boardDropSpeed * Time.deltaTime);
            yield return null;
        }

        board.position = targetPosition; // ��Ȯ�� ���������� ����
    }
}
