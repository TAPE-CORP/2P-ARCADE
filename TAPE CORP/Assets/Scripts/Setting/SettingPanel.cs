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
    public Image fadeOverlay;

    [Header("페이드 설정")]
    public float fadeDuration = 0.5f;
    public float volumeFadeDuration = 0.5f;

    private bool isOpen = false;

    private void Start()
    {
        // 슬라이더 초기값은 저장된 값으로 설정
        float savedBGM = PlayerPrefs.GetFloat("BGM_VOLUME", 1f);
        float savedSE = PlayerPrefs.GetFloat("SE_VOLUME", 1f);

        if (bgmSlider != null)
        {
            bgmSlider.onValueChanged.RemoveAllListeners();
            bgmSlider.value = savedBGM;
            bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        }

        if (seSlider != null)
        {
            seSlider.onValueChanged.RemoveAllListeners();
            seSlider.value = savedSE;
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

            if (BGMManager.Instance != null)
            {
                float original = BGMManager.Instance.bgmVolume;
                StartCoroutine(BGMManager.Instance.FadeBGMVolume(original, 0.1f, volumeFadeDuration));
            }

            Time.timeScale = 0f;
        }
        else
        {
            StartCoroutine(FadeOut(() =>
            {
                settingsPanel.SetActive(false);
                Time.timeScale = 1f;
            }));

            if (BGMManager.Instance != null)
            {
                float sliderVolume = bgmSlider != null ? bgmSlider.value : 1f;
                StartCoroutine(BGMManager.Instance.FadeBGMVolume(0.1f, sliderVolume, volumeFadeDuration));
            }
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
            BGMManager.Instance.SetBGMVolume(value);

            foreach (AudioSource source in BGMManager.Instance.bgmSources)
            {
                if (source != null && !source.isPlaying)
                    source.Play();
            }
        }
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
