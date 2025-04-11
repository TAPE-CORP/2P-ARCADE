using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;


public class SettingsPanel : MonoBehaviour
{
    [Header("UI 요소")]
    public GameObject settingsPanel;
    public Slider bgmSlider;
    public Slider seSlider;
    public UnityEngine.UI.Button quitButton;
    public Text settingsTitleText;

    [Header("페이드 배경")]
    public Image fadeOverlay; // 검정 반투명 오버레이 (알파 0~1 조절용)

    [Header("페이드 설정")]
    public float fadeDuration = 0.5f;
    public float volumeFadeDuration = 0.5f;

    private bool isOpen = false;

    private void Start()
    {
        if (bgmSlider != null)
        {
            bgmSlider.value = BGMManager.Instance != null ? BGMManager.Instance.bgmVolume : 1f;
            bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        }

        if (seSlider != null)
        {
            seSlider.value = BGMManager.Instance != null ? BGMManager.Instance.seVolume : 1f;
            seSlider.onValueChanged.AddListener(OnSEVolumeChanged);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(ReturnToTitle);
        }

        settingsPanel.SetActive(false);
        if (fadeOverlay != null)
        {
            Color c = fadeOverlay.color;
            c.a = 0f;
            fadeOverlay.color = c;
            fadeOverlay.gameObject.SetActive(false);
        }

        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettings();
        }
    }

    private void ToggleSettings()
    {
        isOpen = !isOpen;
        StopAllCoroutines();

        if (isOpen)
        {
            settingsPanel.SetActive(true);
            StartCoroutine(FadeIn());
            StartCoroutine(BGMManager.Instance.FadeBGMVolume(BGMManager.Instance.bgmVolume, 0.1f, volumeFadeDuration));
            Time.timeScale = 0f;
        }
        else
        {
            StartCoroutine(FadeOut(() =>
            {
                settingsPanel.SetActive(false);
                Time.timeScale = 1f;
            }));
            StartCoroutine(BGMManager.Instance.FadeBGMVolume(0.1f, BGMManager.Instance.bgmVolume, volumeFadeDuration));
        }
    }

    IEnumerator FadeIn()
    {
        if (fadeOverlay == null) yield break;

        fadeOverlay.gameObject.SetActive(true);
        Color color = fadeOverlay.color;
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(0f, 0.6f, t / fadeDuration);
            color.a = alpha;
            fadeOverlay.color = color;
            yield return null;
        }
        color.a = 0.6f;
        fadeOverlay.color = color;
    }

    IEnumerator FadeOut(System.Action onComplete)
    {
        if (fadeOverlay == null) yield break;

        Color color = fadeOverlay.color;
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(0.6f, 0f, t / fadeDuration);
            color.a = alpha;
            fadeOverlay.color = color;
            yield return null;
        }
        color.a = 0f;
        fadeOverlay.color = color;
        fadeOverlay.gameObject.SetActive(false);
        onComplete?.Invoke();
    }

    public void OnBGMVolumeChanged(float value)
    {
        if (BGMManager.Instance != null)
        {
            StopCoroutine("FadeInBGMVolume");
            StartCoroutine(FadeInBGMVolume(value));

            foreach (AudioSource source in BGMManager.Instance.bgmSources)
            {
                if (source != null && !source.isPlaying)
                    source.Play();
            }
        }
    }

    private IEnumerator FadeInBGMVolume(float targetVolume)
    {
        float elapsed = 0f;
        float startVolume = 0.1f;

        foreach (AudioSource source in BGMManager.Instance.bgmSources)
        {
            if (source != null)
                source.volume = startVolume;
        }

        while (elapsed < volumeFadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float v = Mathf.Lerp(startVolume, targetVolume, elapsed / volumeFadeDuration);
            foreach (AudioSource source in BGMManager.Instance.bgmSources)
            {
                if (source != null)
                    source.volume = v;
            }
            yield return null;
        }

        foreach (AudioSource source in BGMManager.Instance.bgmSources)
        {
            if (source != null)
                source.volume = targetVolume;
        }

        BGMManager.Instance.bgmVolume = targetVolume;
    }

    public void OnSEVolumeChanged(float value)
    {
        if (BGMManager.Instance != null)
            BGMManager.Instance.SetSEVolume(value);
    }

    public void ReturnToTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Title");
    }
}
