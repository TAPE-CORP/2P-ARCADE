using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundIO : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static SoundIO Instance { get; private set; }

    private AudioSource _audioSource;

    void Awake()
    {
        // 싱글톤 초기화
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // AudioSource 가져오기
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource.clip == null)
        {
            Debug.LogWarning("SoundIO: 재생할 AudioClip이 설정되지 않았습니다.");
        }
    }

    /// <summary>
    /// 외부에서 호출할 메서드.
    /// 전달된 intensity 값에 비례해 볼륨을 설정하고 사운드를 재생합니다.
    /// </summary>
    /// <param name="intensity">0.0f에서 1.0f 사이 값을 권장</param>
    public void PlayWithIntensity(float intensity)
    {
        // 볼륨 값을 0~1로 클램프
        float vol = Mathf.Clamp01(intensity);
        _audioSource.volume = vol;

        // AudioSource가 이미 재생 중이면 중단 후 재생
        _audioSource.Stop();
        _audioSource.Play();
    }
}
