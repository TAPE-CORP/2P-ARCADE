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

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ApplyVolume();
    }

    public void SetBGMVolume(float value)
    {
        bgmVolume = value;
        foreach (AudioSource source in bgmSources)
        {
            if (source != null)
                source.volume = bgmVolume;
        }
    }

    public void SetSEVolume(float value)
    {
        seVolume = value;
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
    }

}
