using UnityEngine;
using System.Collections;

public class EscFadeToggler : MonoBehaviour
{
    [Header("���̵� ���")]
    public GameObject targetObject; // ��� ���
    private CanvasGroup canvasGroup;

    [Header("���̵� ����")]
    public float fadeDuration = 0.3f;

    private bool isVisible = false;
    private Coroutine fadeCoroutine;

    private void Start()
    {
        if (targetObject == null)
        {
            Debug.LogError("TargetObject�� �������.");
            return;
        }

        canvasGroup = targetObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = targetObject.AddComponent<CanvasGroup>();
        }

        // ó������ ���� ���·� ����
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        targetObject.SetActive(false);
        isVisible = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Toggle();
        }
    }

    public void Toggle()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(isVisible ? FadeOut() : FadeIn());
    }

    private IEnumerator FadeIn()
    {
        targetObject.SetActive(true);
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
        isVisible = true;
    }

    private IEnumerator FadeOut()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        targetObject.SetActive(false);
        isVisible = false;
    }
}
