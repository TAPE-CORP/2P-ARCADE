using UnityEngine;
using TMPro; // 여기 추가

public class TimedPanelAndBoard : MonoBehaviour
{
    [Header("타이머 설정 (밀리초)")]
    public float timerDuration = 600000f; // 10분 = 600,000ms

    [Header("UI 참조")]
    public TextMeshProUGUI timerText; // <- TMP로 변경

    [Header("패널 설정")]
    public CanvasGroup fadePanel; // 페이드할 UI 패널
    public float fadeDuration = 2f; // 페이드에 걸리는 시간

    [Header("판 설정")]
    public Transform board; // 내려올 오브젝트
    public Vector3 targetPosition; // 최종 위치
    public float boardDropSpeed = 2f;

    private float timer;
    private bool hasTimerEnded = false;
    private Vector3 boardStartPosition;

    void Start()
    {
        timer = timerDuration;

        if (board != null)
        {
            boardStartPosition = board.position + Vector3.up * 10f; // 위에 숨겨두기
            board.position = boardStartPosition;
        }
    }

    void Update()
    {
        if (!hasTimerEnded)
        {
            timer -= Time.deltaTime * 1000f; // 밀리초 단위 감소
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
        // 패널 페이드
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

        // 판 내려오기
        while (board != null && Vector3.Distance(board.position, targetPosition) > 0.01f)
        {
            board.position = Vector3.MoveTowards(board.position, targetPosition, boardDropSpeed * Time.deltaTime);
            yield return null;
        }

        board.position = targetPosition; // 정확히 도착지점에 고정
    }
}
