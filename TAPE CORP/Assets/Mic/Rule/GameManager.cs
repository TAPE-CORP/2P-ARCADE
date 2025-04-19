using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Settlement UI")]
    public RectTransform scorePanel;
    public Text scoreText;
    public Text continueText;

    [Header("Settlement Timings")]
    public float countUpMaxDuration = 2f;
    public float promptDropDuration = 1f;

    [Header("Spring Settings")]
    public float springStiffness = 200f;
    public float springDamping = 25f;

    private float finalScore;
    private bool isGameOver = false;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            if (scorePanel != null) scorePanel.gameObject.SetActive(false);
            if (scoreText != null) scoreText.gameObject.SetActive(false);
            if (continueText != null) continueText.gameObject.SetActive(false);
        }
        else Destroy(gameObject);
    }

    void Update()
    {
        finalScore += Time.deltaTime * 7f;
        if (!isGameOver && GameObject.FindGameObjectsWithTag("Player").Length == 0)
        {
            isGameOver = true;
            GameOver();
        }
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        DisableOtherComponents();
        StartCoroutine(SettlementSequence());
    }

    void DisableOtherComponents()
    {
        foreach (var mb in FindObjectsOfType<MonoBehaviour>())
        {
            if (mb == this) continue;
            var t = mb.transform;
            if (scorePanel != null && t.IsChildOf(scorePanel)) continue;
            if (scoreText != null && t.IsChildOf(scoreText.transform)) continue;
            if (continueText != null && t.IsChildOf(continueText.transform)) continue;
            mb.enabled = false;
        }
    }

    private IEnumerator SettlementSequence()
    {
        int score = Mathf.RoundToInt(finalScore);

        // 패널 드롭
        if (scorePanel != null)
        {
            scorePanel.gameObject.SetActive(true);
            Vector2 targetPos = scorePanel.anchoredPosition;
            float startY = Screen.height / 2f + scorePanel.rect.height;
            float posY = startY, velocity = 0f;
            while (true)
            {
                float dt = Time.unscaledDeltaTime;
                velocity += (targetPos.y - posY) * springStiffness * dt;
                velocity *= Mathf.Exp(-springDamping * dt);
                posY += velocity * dt;
                scorePanel.anchoredPosition = new Vector2(targetPos.x, posY);
                if (Mathf.Abs(posY - targetPos.y) < 0.5f && Mathf.Abs(velocity) < 0.5f)
                {
                    scorePanel.anchoredPosition = targetPos;
                    break;
                }
                yield return new WaitForSecondsRealtime(0f);
            }
        }

        // 점수 카운트업
        if (scoreText != null)
        {
            scoreText.gameObject.SetActive(true);
            float duration = Mathf.Min(countUpMaxDuration, score / 1000f);
            float elapsed = 0f;
            while (elapsed < duration)
            {
                scoreText.text = Mathf.RoundToInt(score * (elapsed / duration)).ToString();
                elapsed += Time.unscaledDeltaTime;
                yield return new WaitForSecondsRealtime(0f);
            }
            scoreText.text = score.ToString();
        }

        // Continue 문구 드롭 & 페이드인
        if (continueText != null)
        {
            continueText.gameObject.SetActive(true);
            Vector2 endPos = continueText.rectTransform.anchoredPosition;
            float startOff = -Screen.height / 2f - continueText.preferredHeight;
            continueText.rectTransform.anchoredPosition = new Vector2(endPos.x, startOff);
            Color c = continueText.color;
            c.a = 0f;
            continueText.color = c;
            float elapsed2 = 0f;
            while (elapsed2 < promptDropDuration)
            {
                float t = elapsed2 / promptDropDuration;
                float y = Mathf.Lerp(startOff, endPos.y, t);
                continueText.rectTransform.anchoredPosition = new Vector2(endPos.x, y);
                c.a = Mathf.Lerp(0f, 1f, t);
                continueText.color = c;
                elapsed2 += Time.unscaledDeltaTime;
                yield return new WaitForSecondsRealtime(0f);
            }
            continueText.rectTransform.anchoredPosition = endPos;
            c.a = 1f;
            continueText.color = c;
        }

        // 입력 대기
        while (!Input.anyKeyDown && !Input.GetMouseButtonDown(0)
               && !Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2))
        {
            yield return new WaitForSecondsRealtime(0f);
        }

        Time.timeScale = 1f;
        finalScore = 0;
        SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
    }
}