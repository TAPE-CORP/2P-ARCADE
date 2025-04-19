using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player Objects (�ν����� �Ҵ�)")]
    [Tooltip("NavMesh �� ���� ��ġ�� �̵���ų GameObject�� �ִ� �� ������ �Ҵ��ϼ���.")]
    public List<GameObject> players = new List<GameObject>(2);

    [Header("Enemy Objects (�ν����� �Ҵ�)")]
    [Tooltip("3�� �� Ȱ��ȭ�� �� ������Ʈ�� �Ҵ��ϼ���.")]
    public List<GameObject> enemies = new List<GameObject>();

    [Tooltip("�÷��̾� �� �ּ� �Ÿ�(�浹 ������)")]
    public float minSeparation = 1f;

    [Header("Countdown UI")]
    [Tooltip("3�� ī��Ʈ�ٿ��� ǥ���� Text ������Ʈ�� �Ҵ��ϼ���.")]
    public Text countdownText;

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
            PositionPlayersOnNavMesh();
            DisableUIElements();
            DeactivateEnemies();
            if (countdownText != null)
                StartCoroutine(ActivateEnemiesWithCountdown());
            else
                StartCoroutine(ActivateEnemiesAfterDelay());
        }
        else Destroy(gameObject);
    }

    void DeactivateEnemies()
    {
        foreach (var go in enemies)
            if (go != null)
                go.SetActive(false);
    }

    private IEnumerator ActivateEnemiesAfterDelay()
    {
        yield return new WaitForSecondsRealtime(3f);
        foreach (var go in enemies)
            if (go != null)
                go.SetActive(true);
    }

    private IEnumerator ActivateEnemiesWithCountdown()
    {
        int count = 3;
        countdownText.gameObject.SetActive(true);
        Vector3 originalScale = countdownText.rectTransform.localScale;
        Color originalColor = countdownText.color;

        for (int i = count; i > 0; i--)
        {
            countdownText.text = i.ToString();
            float progress = (float)(count - i) / (count - 1);
            countdownText.rectTransform.localScale = originalScale * Mathf.Lerp(1f, 2f, progress);
            countdownText.color = Color.Lerp(originalColor, Color.red, progress);

            yield return new WaitForSecondsRealtime(1f);
        }

        countdownText.gameObject.SetActive(false);
        foreach (var go in enemies)
            if (go != null)
                go.SetActive(true);
    }

    void PositionPlayersOnNavMesh()
    {
        var triang = NavMesh.CalculateTriangulation();
        var verts = triang.vertices;
        var indices = triang.indices;

        List<Vector3> placedPositions = new List<Vector3>();
        foreach (var go in players)
        {
            if (go == null) continue;
            Vector3 pos = Vector3.zero;
            int attempts = 0;
            do
            {
                int triIndex = Random.Range(0, indices.Length / 3) * 3;
                Vector3 v0 = verts[indices[triIndex]];
                Vector3 v1 = verts[indices[triIndex + 1]];
                Vector3 v2 = verts[indices[triIndex + 2]];
                float r1 = Random.value;
                float r2 = Random.value;
                if (r1 + r2 > 1f) { r1 = 1f - r1; r2 = 1f - r2; }
                pos = v0 + r1 * (v1 - v0) + r2 * (v2 - v0);
                attempts++;
            }
            while (!IsFarFromPositions(pos, placedPositions, minSeparation) && attempts < 30);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(pos, out hit, 1f, NavMesh.AllAreas))
                pos = hit.position;

            go.transform.position = pos;
            go.SetActive(true);
            placedPositions.Add(pos);
        }
    }

    bool IsFarFromPositions(Vector3 pos, List<Vector3> positions, float minDist)
    {
        foreach (var p in positions)
            if (Vector3.Distance(pos, p) < minDist)
                return false;
        return true;
    }

    void DisableUIElements()
    {
        if (scorePanel != null) scorePanel.gameObject.SetActive(false);
        if (scoreText != null) scoreText.gameObject.SetActive(false);
        if (continueText != null) continueText.gameObject.SetActive(false);
        if (countdownText != null) countdownText.gameObject.SetActive(false);
    }

    void Update()
    {
        finalScore += Time.deltaTime * 7f;
        if (!isGameOver)
        {
            bool anyAlive = false;
            foreach (var p in players)
            {
                if (p != null && p.activeInHierarchy)
                {
                    anyAlive = true;
                    break;
                }
            }
            if (!anyAlive)
            {
                isGameOver = true;
                GameOver();
            }
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
            if (countdownText != null && t.IsChildOf(countdownText.transform)) continue;
            mb.enabled = false;
        }
    }

    private IEnumerator SettlementSequence()
    {
        int score = Mathf.RoundToInt(finalScore);

        // 1) �г� ���
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

        // 2) ���� ī��Ʈ��
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

        // 3) Continue ���� ��� & ���̵���
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

        // 4) �Է� ���
        while (!Input.anyKeyDown && !Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2))
            yield return new WaitForSecondsRealtime(0f);

        Time.timeScale = 1f;
        finalScore = 0f;
        SceneManager.LoadScene("TitleScene", LoadSceneMode.Single);
    }
}