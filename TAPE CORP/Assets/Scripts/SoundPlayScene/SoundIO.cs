using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundIO : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static SoundIO Instance { get; private set; }

    private AudioSource _audioSource;

    void Awake()
    {
        // �̱��� �ʱ�ȭ
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // AudioSource ��������
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource.clip == null)
        {
            Debug.LogWarning("SoundIO: ����� AudioClip�� �������� �ʾҽ��ϴ�.");
        }
    }

    /// <summary>
    /// �ܺο��� ȣ���� �޼���.
    /// ���޵� intensity ���� ����� ������ �����ϰ� ���带 ����մϴ�.
    /// </summary>
    /// <param name="intensity">0.0f���� 1.0f ���� ���� ����</param>
    public void PlayWithIntensity(float intensity)
    {
        // ���� ���� 0~1�� Ŭ����
        float vol = Mathf.Clamp01(intensity);
        _audioSource.volume = vol;

        // AudioSource�� �̹� ��� ���̸� �ߴ� �� ���
        _audioSource.Stop();
        _audioSource.Play();
    }
}
