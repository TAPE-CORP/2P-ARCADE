// GameManager.cs
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Scan Settings")]
    [Tooltip("���� ���� ��ĵ �ð� (��)")]
    public float scanUpDuration = 2f;
    [Tooltip("���� �β� (���� ����)")]
    public float scanLineThickness = 0.5f;
    [Tooltip("���� ��ĵ ����")]
    public Color scanLightColor = Color.white;

    [Header("Scan Down Settings")]
    [Tooltip("�Ʒ��� �������� ���� ���� ���� �ð� (��)")]
    public float scanDownDuration = 1f;

    [Header("Darken Settings")]
    [Tooltip("���� ���� �� ��ο����� �ð� (��)")]
    public float darkenDuration = 1.5f;

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

    float finalScore;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // UI �ʱ� ����
            if (scorePanel != null) scorePanel.gameObject.SetActive(false);
            if (scoreText != null) scoreText.gameObject.SetActive(false);
            if (continueText != null) continueText.gameObject.SetActive(false);
        }
        else Destroy(gameObject);
    }

    void Update()
    {
        // ���� ���ھ�(�и���) ����
        finalScore = Time.time * 1000f;
    }

    public void GameOver()
    {
        // ���� ���� �� �Է� ���
        Time.timeScale = 0f;
        DisableOtherComponents();
        // ��ĵ����������������ο��������� UI
        StartCoroutine(ScanDownthenSettle());
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

    IEnumerator ScanDownthenSettle()
    {
        // ī�޶� ����
        var cam = Camera.main;
        float camH = cam.orthographicSize * 2f;
        float camW = camH * cam.aspect;
        float y0 = cam.transform.position.y - cam.orthographicSize - scanLineThickness;
        float y1 = cam.transform.position.y + cam.orthographicSize + scanLineThickness;
        float cx = cam.transform.position.x;

        // 1) ���� ���� ��ĵ
        var scanObj = new GameObject("ScanLine");
        var scanLight = scanObj.AddComponent<Light2D>();
        scanLight.lightType = Light2D.LightType.Freeform;
        scanLight.color = scanLightColor;
        scanLight.intensity = 1f;
        scanObj.transform.SetParent(transform);

        float t = 0f;
        while (t < scanUpDuration)
        {
            float n = t / scanUpDuration;
            float y = Mathf.Lerp(y0, y1, n);
            var shape = new Vector3[]
            {
                new Vector3(-camW/2f, -scanLineThickness/2f, 0f),
                new Vector3(camW/2f, -scanLineThickness/2f, 0f),
                new Vector3(camW/2f, scanLineThickness/2f, 0f),
                new Vector3(-camW/2f, scanLineThickness/2f, 0f)
            };
            scanLight.SetShapePath(shape);
            scanObj.transform.localPosition = new Vector3(cx, y, 0f);
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        // ��ĵ �ֻ�� ����
        scanObj.transform.localPosition = new Vector3(cx, y1, 0f);

        // 2) �Ʒ��� �������� ������ ���� ����
        var revealObj = new GameObject("RevealLight");
        var revealL = revealObj.AddComponent<Light2D>();
        revealL.lightType = Light2D.LightType.Freeform;
        revealL.color = scanLightColor;
        revealL.intensity = 1f;
        revealObj.transform.SetParent(transform);

        t = 0f;
        while (t < scanDownDuration)
        {
            float n = t / scanDownDuration;
            float y = Mathf.Lerp(y1, y0, n);
            float h = Mathf.Max(0.01f, y1 - y);
            var shape = new Vector3[]
            {
                new Vector3(-camW/2f, -h/2f, 0f),
                new Vector3(camW/2f, -h/2f, 0f),
                new Vector3(camW/2f, h/2f, 0f),
                new Vector3(-camW/2f, h/2f, 0f)
            };
            revealL.SetShapePath(shape);
            revealObj.transform.localPosition = new Vector3(cx, (y1 + y) / 2f, 0f);
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        // ��ü ����
        var full = new Vector3[]
        {
            new Vector3(-camW/2f, -camH/2f, 0f),
            new Vector3(camW/2f, -camH/2f, 0f),
            new Vector3(camW/2f, camH/2f, 0f),
            new Vector3(-camW/2f, camH/2f, 0f)
        };
        revealL.SetShapePath(full);
        revealObj.transform.localPosition = new Vector3(cx, cam.transform.position.y, 0f);

        Destroy(scanObj);

        // 3) ��ü ���� �� ��ο���
        float d = 0f;
        while (d < darkenDuration)
        {
            revealL.intensity = Mathf.Lerp(1f, 0f, d / darkenDuration);
            d += Time.unscaledDeltaTime;
            yield return null;
        }
        //Destroy(revealObj);

        // 4) ���� UI
        yield return StartCoroutine(SettlementSequence());
    }

    private IEnumerator SettlementSequence()
    {
        int score = Mathf.RoundToInt(finalScore);

        // �г� ���
        if (scorePanel != null)
        {
            scorePanel.gameObject.SetActive(true);
            Vector2 sp = scorePanel.anchoredPosition;
            float offY = Screen.height / 2f + scorePanel.rect.height;
            float posY = offY, vy = 0f;
            while (true)
            {
                float dt = Time.unscaledDeltaTime;
                vy += (sp.y - posY) * springStiffness * dt;
                vy *= Mathf.Exp(-springDamping * dt);
                posY += vy * dt;
                scorePanel.anchoredPosition = new Vector2(sp.x, posY);
                if (Mathf.Abs(posY - sp.y) < 0.5f && Mathf.Abs(vy) < 0.5f)
                {
                    scorePanel.anchoredPosition = sp;
                    break;
                }
                yield return null;
            }
        }

        // ���� ī��Ʈ��
        if (scoreText != null)
        {
            scoreText.gameObject.SetActive(true);
            float dur = Mathf.Min(countUpMaxDuration, score / 1000f);
            float ct = 0f;
            while (ct < dur)
            {
                scoreText.text = Mathf.RoundToInt(score * (ct / dur)).ToString();
                ct += Time.unscaledDeltaTime;
                yield return null;
            }
            scoreText.text = score.ToString();
        }

        // Continue ���� ���
        if (continueText != null)
        {
            continueText.gameObject.SetActive(true);
            Vector2 ep = continueText.rectTransform.anchoredPosition;
            float off = -Screen.height / 2f - continueText.preferredHeight;
            continueText.rectTransform.anchoredPosition = new Vector2(ep.x, off);
            float ct2 = 0f;
            while (ct2 < promptDropDuration)
            {
                float y = Mathf.Lerp(off, ep.y, ct2 / promptDropDuration);
                continueText.rectTransform.anchoredPosition = new Vector2(ep.x, y);
                ct2 += Time.unscaledDeltaTime;
                yield return null;
            }
            continueText.rectTransform.anchoredPosition = ep;
        }

        // �Է� ���
        while (!Input.anyKeyDown) yield return null;

        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScene");
    }
}
