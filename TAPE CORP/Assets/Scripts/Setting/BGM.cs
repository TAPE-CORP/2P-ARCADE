using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    [Header("BGM / SE AudioSources")]
    public List<AudioSource> bgmSources;
    public List<AudioSource> seSources;

    [Range(0f, 1f)] public float bgmVolume = 1f;
    [Range(0f, 1f)] public float seVolume = 1f;

    private const string BGM_KEY = "BGM_VOLUME";
    private const string SE_KEY = "SE_VOLUME";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        bgmVolume = PlayerPrefs.GetFloat(BGM_KEY, 1f);
        seVolume = PlayerPrefs.GetFloat(SE_KEY, 1f);
    }

    private void Start()
    {
        ApplyVolume();

        foreach (AudioSource bgm in bgmSources)
        {
            if (bgm != null)
            {
                bgm.loop = true;
                if (!bgm.isPlaying)
                    bgm.Play();
            }
        }
    }

    public void SetBGMVolume(float value)
    {
        bgmVolume = value;
        PlayerPrefs.SetFloat(BGM_KEY, bgmVolume);
        foreach (AudioSource source in bgmSources)
        {
            if (source != null)
                source.volume = bgmVolume;
        }
    }

    public void SetSEVolume(float value)
    {
        seVolume = value;
        PlayerPrefs.SetFloat(SE_KEY, seVolume);
        foreach (AudioSource source in seSources)
        {
            if (source != null)
                source.volume = seVolume;
        }
    }

    public void ApplyVolume()
    {
        SetBGMVolume(bgmVolume);
        SetSEVolume(seVolume);
    }

    public IEnumerator FadeBGMVolume(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float v = Mathf.Lerp(from, to, elapsed / duration);
            foreach (AudioSource source in bgmSources)
            {
                if (source != null)
                    source.volume = v;
            }
            yield return null;
        }

        foreach (AudioSource source in bgmSources)
        {
            if (source != null)
                source.volume = to;
        }

        bgmVolume = to;
        PlayerPrefs.SetFloat(BGM_KEY, bgmVolume);
    }
}
